using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers;

[Authorize(Roles = AppRoles.Reception)]
public class SubscribersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    public SubscribersController(ApplicationDbContext context, IMapper mapper, IImageService imageService)
    {
        _context = context;
        _mapper = mapper;
        _imageService = imageService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Search(SearchFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subscriber = _context.Subscribers.SingleOrDefault(s => 
                                                s.MobileNumber == model.value ||
                                                s.NationalId == model.value ||
                                                s.Email == model.value);

        var viewModel = _mapper.Map<SubscriberSearchResultViewModel>(subscriber);

        return PartialView("_Result", viewModel);
    }

    public IActionResult Details(int id)
    {
        var subscriber = _context.Subscribers
            .Include(s => s.Governorate)
            .Include(s => s.Area)
            .SingleOrDefault(s => s.Id == id);

        if (subscriber == null)
            return NotFound();

        var viewModel = _mapper.Map<SubscriberViewModel>(subscriber);

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View("Form", PopulateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubscriberFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel());

        var subscriber = _mapper.Map<Subscriber>(model);
        var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
        var imagePath = "/images/subscribers";
        var (isUploaded, errorMassage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, true);

        if (!isUploaded) {
            ModelState.AddModelError("Image", errorMassage!);
            return View("Form", PopulateViewModel());
        }

        subscriber.ImageUrl = $"{imagePath}/{imageName}";
        subscriber.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
        subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        _context.Add(subscriber);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index), new { id = subscriber.Id});
    }

    public IActionResult Edit(int id)
    {
        var subscriber = _context.Subscribers.Find(id);

        if (subscriber is null)
            return NotFound();

        var model = _mapper.Map<SubscriberFormViewModel>(subscriber);
        var viewModel = PopulateViewModel(model);

        return View("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubscriberFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel());

        var subscriber = _context.Subscribers.Find(model.Id);

        if (subscriber is null)
            return NotFound();

        if (model.Image is not null)
        {
            if (!string.IsNullOrEmpty(subscriber.ImageUrl))
                _imageService.Delete(subscriber.ImageUrl, subscriber.ImageThumbnailUrl);

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var imagePath = "/images/subscribers";
            var (isUploaded, errorMassage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, true);

            if (!isUploaded)
            {
                ModelState.AddModelError("Image", errorMassage!);
                return View("Form", PopulateViewModel());
            }

            model.ImageUrl = $"{imagePath}/{imageName}";
            model.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
        }
        else if (!string.IsNullOrEmpty(subscriber.ImageUrl))
        {
            model.ImageUrl = subscriber.ImageUrl;
            model.ImageThumbnailUrl = subscriber.ImageThumbnailUrl;
        }

        subscriber = _mapper.Map(model, subscriber);
        subscriber.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        subscriber.LastUpdatedOn = DateTime.Now;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index), new { id = subscriber.Id });
    }

    [AjaxOnly]
    public IActionResult GetAreas(int governorateId)
    {
        var areas = _context.Areas
                            .Where(a => a.GovernorateId == governorateId && !a.IsDeleted)
                            .OrderBy(a => a.Name)
                            .ToList();

        return Ok(_mapper.Map<IEnumerable<SelectListItem>>(areas));
    }

    public IActionResult AllowNationalId(SubscriberFormViewModel model)
    {
        var subscriber = _context.Subscribers.FirstOrDefault(s => s.NationalId == model.NationalId);
        var isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);

        return Json(isAllowed);
    }

    public IActionResult AllowMobileNumber(SubscriberFormViewModel model)
    {
        var subscriber = _context.Subscribers.FirstOrDefault(s => s.MobileNumber == model.MobileNumber);
        var isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);

        return Json(isAllowed);
    }

    public IActionResult AllowEmail(SubscriberFormViewModel model)
    {
        var subscriber = _context.Subscribers.FirstOrDefault(s => s.Email == model.Email);
        var isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);

        return Json(isAllowed);
    }

    private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model = null)
    {
        SubscriberFormViewModel viewModel = model is null ? new SubscriberFormViewModel() : model;

        var governorates = _context.Governorates.Where(g => !g.IsDeleted).OrderBy(g => g.Name).ToList();
        viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorates);

        if (model?.GovernorateId > 0)
        {
            var areas = _context.Areas
                                .Where(a => a.GovernorateId == model.GovernorateId && !a.IsDeleted)
                                .OrderBy(a => a.Name)
                                .ToList();

            viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
        }

        return viewModel;
    }
}
