using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}