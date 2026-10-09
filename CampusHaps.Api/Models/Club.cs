using System.ComponentModel.DataAnnotations;
namespace CampusHaps.Api.Models
{
    // Represents a club or society at the university
    // Used for club pages and connecting clubs to their events and posts
    // Should have ID name description category and contact email
    public class Club
    {

        //user info lists
        public int Id { get; set; } //clubs unique ID
        [Required]
        public string Name { get; set; } //the club's name
        public string Description { get; set; } //description of the clubs
        public string Category { get; set; } //the category of club
        [EmailAddress]
        public string ContactEmail { get; set; } //the club's email to contact them 

        //posts and events lists
        public List<Event> Events { get; set; } = new List<Event>(); //list of events belonging to the club
        public List<ClubPost> Posts { get; set; } = new List<ClubPost>(); //list of posts belonging to the club





    }
}
