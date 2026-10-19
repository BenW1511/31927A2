using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.DTOs{
    public class RegisterDto{

        //what the frontend sends when someone logs in, password isnt stored only hash is saved and compared

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        //only accepts .edu.au emails
        [Required, EmailAddress, MaxLength(255)]
        [RegularExpression(@"^[^@\s]+@([A-Za-z0-9-]+\.)*edu\.au$",
            ErrorMessage = "Please use your university email (.edu.au).")]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
