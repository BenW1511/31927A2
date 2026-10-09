using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.Models
{
    // Represents a comment made by a user on an event
    // Used so users can discuss events
    // Should have ID content date/time user ID and event ID
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
