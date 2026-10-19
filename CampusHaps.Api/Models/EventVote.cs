using Microsoft.EntityFrameworkCore;

namespace CampusHaps.Api.Models
{
    //represents a users vote on an event
    //the unique index below means the database rejects a second vote from the same user on the same event
   
    [Index(nameof(UserId), nameof(EventId), IsUnique = true)]
    public class EventVote
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
