using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.Models
{
    //Represents a club or society at the university
    //Used for club pages and connecting clubs to their events and posts
    //Should have ID name description category and contact email
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
