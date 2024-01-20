using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    /// <summary>
    /// this is reliant on an author and genre pre-existing
    /// </summary>
    public class BookCreateRequest
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "AuthorId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid AuthorId")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "GenreId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid GenreId")]
        public int GenreId { get; set; }
    }
}