using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.DTOs
{
    public class CreateClubDto //defines the information the frontend sends to the API when creating a club
    {
        [Required]
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        [EmailAddress]
        public string ContactEmail { get; set; } = "";

        
    }
}
