using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class UpdateBookPositionOnListRequest
    {
        [Required(ErrorMessage = "ListId is required.")]
        public int ListId { get; set; }

        [Required(ErrorMessage = "BookId is required.")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "NewPosition is required.")]
        public int NewPosition { get; set; }
    }
}