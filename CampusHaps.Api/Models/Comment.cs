using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.Models
{
    //represents a comment made by a user on an event
    //used so users can discuss events
    //should have ID content date/time user ID and event ID
    public class Comment
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;


    }
}
