using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class AuthorCreateRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
    }
}