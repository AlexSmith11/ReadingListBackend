using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<List>? Lists { get; set; } = new();
    }
}