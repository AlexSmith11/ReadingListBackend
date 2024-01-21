using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class AuthorUpdateRequest
    {
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Age")]
        public int? Age { get; set; }

        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string Country { get; set; }
    }
}