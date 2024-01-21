namespace ReadingListBackend.Models
{
    public class UserListBook
    {
        // Composite key
        public int BookId { get; set; }
        public int ListId { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public Book Book { get; set; }
        public List List { get; set; }
        public User User { get; set; }
    }
}