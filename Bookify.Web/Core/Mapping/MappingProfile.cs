namespace Bookify.Web.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Author
        CreateMap<Author, AuthorViewModel>();
        CreateMap<AuthorFormViewModel, Author>().ReverseMap();

        // Category
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryFormViewModel, Category>().ReverseMap();
    }
}
