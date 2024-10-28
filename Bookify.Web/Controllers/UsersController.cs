using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);

        return View(viewModel);
    }

    [AjaxOnly]
    public async Task<IActionResult> Create()
    {

        return PartialView("_Form");
    }
}
