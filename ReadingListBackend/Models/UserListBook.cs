namespace ReadingListBackend.Models
{
    public class UserListBook
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int ListId { get; set; }
        public List List { get; set; }
        public int Order { get; set; }
        public bool IsRead { get; set; }
    }
}