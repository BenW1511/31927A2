using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.DTOs
{
    public class CreateCommentDto //defines the information the frontend sends to the API when creating a comment
    {
        public int UserId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = "";
    }
}
