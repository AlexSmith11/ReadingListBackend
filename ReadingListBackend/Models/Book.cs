﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<ListBook> ListBooks { get; set; } = new();
    }
}