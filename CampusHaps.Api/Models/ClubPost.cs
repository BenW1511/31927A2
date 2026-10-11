using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.Models
{
    // Represents a post created by a club or society
    // Used by clubs to make announcements and share information
    // Should have ID title content date/time club ID and author ID
    public class ClubPost 
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;


    }
}
