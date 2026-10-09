using System.ComponentModel.DataAnnotations;
namespace CampusHaps.Api.Models
{
    // Represents a post created by a club or society
    // Used by clubs to make announcements and share information
    // Should have ID title content date/time club ID and author ID
    public class ClubPost 
    {
        int Id { get; set; } //unique id of the club's post
        [Required]
        string Title { get; set; } //title of the post
        string Content { get; set; } //what is inside the post
        DateTime CreatedAt { get; set; } //time and date the post was created at
        
        int ClubId { get; set; } 
        Club Club  { get; set; } //club

        int AuthorId { get; set; }
        User Author { get; set; } //author




    }
}
