using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class GenreCreateRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}