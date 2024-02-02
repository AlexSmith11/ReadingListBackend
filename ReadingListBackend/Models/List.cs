using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Models
{
    public class List
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public List<ListBook> ListBooks { get; set; } = new();
    }
}