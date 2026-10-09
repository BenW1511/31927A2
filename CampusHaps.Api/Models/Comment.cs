using System.ComponentModel.DataAnnotations;
namespace CampusHaps.Api.Models
{
    // Represents a comment made by a user on an event
    // Used so users can discuss events
    // Should have ID content date/time user ID and event ID
    public class Comment
    {
        int Id { get; set; } //unique id of the comment

        [Required]
        [MaxLength(500)]
        string Content { get; set; } //the text inside the comment, can only be 500 characters long
        DateTime CreatedAt { get; set; } //time and date the comment it was created at
      
        int UserId { get; set; }
        User User { get; set; } //User assigned to the comment

        int EventId { get; set; }
        Event Event { get; set; } //Event the comment is on 

    }
}
