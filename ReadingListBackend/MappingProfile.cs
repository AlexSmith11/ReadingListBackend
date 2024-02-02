using AutoMapper;
using ReadingListBackend.Models;
using ReadingListBackend.Responses;

namespace ReadingListBackend
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<Author, AuthorResponse>();
            CreateMap<Genre, GenreResponse>();
            CreateMap<Book, BookResponse>();
            CreateMap<List, ListResponse>();
        }
    }
}