using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Models
{
    public class ListBook
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListId { get; set; }
        public List List { get; set; }


        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }
        
        public bool IsRead { get; set; }
        public int Position { get; set; }
    }
}