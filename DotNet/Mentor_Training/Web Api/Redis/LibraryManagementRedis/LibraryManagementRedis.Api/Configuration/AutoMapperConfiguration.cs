namespace LibraryManagementRedis.Api.Configuration;
public static class AutoMapperConfiguration
{
    public static IMapper InitializeMapper()
    {
        var mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<Author, AuthorVm>().ReverseMap();
            config.CreateMap<AuthorDto, Author>();

            config.CreateMap<Book, BookVm>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author!.Name));
            config.CreateMap<BookDto, Book>()
                .ForMember(dest => dest.Author, opt => opt.Ignore());

            config.CreateMap<Borrower, BorrowerVm>()
                .ForMember(dest => dest.BorrowedBooks,
                    opt => opt.MapFrom(src => src.BorrowedBooks.Select(b => b.Title).ToList()));
            config.CreateMap<BorrowerDto, Borrower>();
        });

        return mapper.CreateMapper();
    }
}
