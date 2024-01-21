namespace ReadingListBackend.Models
{
    public class UserBook
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int ListId { get; set; }
        public List List { get; set; }

        public bool IsRead { get; set; }
    }
}