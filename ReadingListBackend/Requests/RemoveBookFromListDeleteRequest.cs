using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class RemoveBookFromListDeleteRequest
    {
        [Required(ErrorMessage = "ListId is required.")]
        public int ListId { get; set; }

        [Required(ErrorMessage = "BookId is required.")]
        public int BookId { get; set; }
    }
}