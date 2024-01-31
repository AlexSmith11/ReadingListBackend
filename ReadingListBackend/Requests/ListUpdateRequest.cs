using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class ListUpdateRequest
    {
        [Required(ErrorMessage = "ListId is required.")]
        public int ListId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
    }
}