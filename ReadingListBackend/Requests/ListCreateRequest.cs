using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class ListCreateRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }
    }
}