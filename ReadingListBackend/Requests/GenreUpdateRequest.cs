using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class GenreUpdateRequest
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
    }
}