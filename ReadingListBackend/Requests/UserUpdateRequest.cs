using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class UserUpdateRequest
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
    }
}