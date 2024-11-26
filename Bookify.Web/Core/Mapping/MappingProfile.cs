﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Areas
        CreateMap<Area, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        // Authors
        CreateMap<Author, AuthorViewModel>();
        CreateMap<AuthorFormViewModel, Author>().ReverseMap();
        CreateMap<Author, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        // Books
        CreateMap<BookFormViewModel, Book>()
            .ReverseMap()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<Book, BookViewModel>()
			.ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
			.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Category!.Name).ToList()));

        // BookCopies
        CreateMap<BookCopy, BookCopyViewModel>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title));
        CreateMap<BookCopy, BookCopyFormViewModel>();

        // Categories
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryFormViewModel, Category>().ReverseMap();
        CreateMap<Category, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        // Users
        CreateMap<ApplicationUser, UserViewModel>();
        CreateMap<UserFormViewModel, ApplicationUser>()
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ReverseMap();

        // Governorates
        CreateMap<Governorate, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        // Subscribers
        CreateMap<Subscriber, SubscriberFormViewModel>().ReverseMap();
        CreateMap<Subscriber, SubscriberSearchResultViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<Subscriber, SubscriberViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate!.Name))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name));
    }
}
