﻿using System.Linq;
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
            CreateMap<Book, BookResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            CreateMap<List, ListResponse>()
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.ListBooks.Select(lb => lb.Book).ToList()));
        }
    }
}