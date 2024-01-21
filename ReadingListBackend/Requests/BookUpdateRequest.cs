using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class BookUpdateRequest
    {
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid AuthorId")]
        public int? AuthorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid GenreId")]
        public int? GenreId { get; set; }
    }
}