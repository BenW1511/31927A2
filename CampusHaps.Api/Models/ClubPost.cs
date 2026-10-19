using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.Models
{
    //represents a post created by a club or society
    //used by clubs to make announcements and share information
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
