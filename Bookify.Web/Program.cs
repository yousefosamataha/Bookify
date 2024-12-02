using Bookify.Web.Core.Mapping;
using Bookify.Web.Helpers;
using Bookify.Web.Seeds;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Identity Configuration
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders();
    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    });

    // SecurityStamp Configuration
    builder.Services.Configure<SecurityStampValidatorOptions>(opt => opt.ValidationInterval = TimeSpan.Zero);

    // AutoMapper Configuration
    var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
    IMapper mapper = mapperConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    // ExpressiveAnnotations
    builder.Services.AddExpressiveAnnotations();

    // Cloudinary Configuration
    builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));

    // Add User Claims
    builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

    // Add Image Service
    builder.Services.AddTransient<IImageService, ImageService>();

    // Email Configuration & Add Email Service & Add Email Body Builder Service
    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
    builder.Services.AddTransient<IEmailSender, EmailSender>();
    builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();

    // Add Data Protection Service
    builder.Services.AddDataProtection().SetApplicationName(nameof(Bookify));

    builder.Services.AddControllersWithViews();
}


WebApplication? app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
    using var scope = scopeFactory.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await DefaultRoles.SeedRolesAsync(roleManager);
    await DefaultUsers.SeedAdminUserAsync(userManager);

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Run();
}

