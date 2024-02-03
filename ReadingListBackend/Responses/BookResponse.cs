namespace ReadingListBackend.Responses
{
    public class BookResponse
    {
        public string Title { get; set; }
        public int PageCount { get; set; }
        public string AuthorName { get; set; }
        public string GenreName { get; set; }
    }
}