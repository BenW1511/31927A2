using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.Models
{
    //represents a club or society at the university
    //used for club pages and connecting clubs to their events and posts
    public class Club
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";
        public string Category { get; set; } = "";

        [EmailAddress]
        public string ContactEmail { get; set; } = "";

        public List<Event> Events { get; set; } = new List<Event>();
        public List<ClubPost> Posts { get; set; } = new List<ClubPost>();

    }
}
