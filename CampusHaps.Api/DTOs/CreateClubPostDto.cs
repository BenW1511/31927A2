using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.DTOs
{
    public class CreateClubPostDto //defines the information the frontend sends to the API when creating a club post
    {
        [Required]
        public string Title { get; set; } = "";
        [Required]
        public string Content { get; set; } = "";
        public int AuthorId { get; set; }
    }
}
