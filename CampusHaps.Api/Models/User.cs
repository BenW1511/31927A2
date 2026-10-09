using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CampusHaps.Api.Models

{
    //Represents a student/user of the application
    //Used for things like logging in and identifying who made comments or votes
    [Index(nameof(Email), IsUnique = true)]
    public class User 
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public bool IsVerified { get; set; }
        //Hashed version of the user's password, the real password is never stored
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
