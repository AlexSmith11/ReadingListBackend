using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadingListBackend.Requests
{
    public class UserCreateRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Username == "admin")
            {
                yield return new ValidationResult("Username 'admin' is not allowed", new[] { nameof(Username) });
            }
        }
    }
}