using System.Collections.Generic;

namespace ReadingListBackend.Responses
{
    public class ListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookResponse> Books { get; set; } = new();
    }
}