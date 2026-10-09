using System.ComponentModel.DataAnnotations;
using CampusHaps.Api.Enums;

namespace CampusHaps.Api.Models
{
    //Represents a campus event that students can discover and interact with
    //Used to display events in the feed and calendar
    public class Event
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [MaxLength(500)]
        public string? TicketUrl { get; set; }
        
        public EventType Type { get; set; }

        public int? ClubId { get; set; }
        public Club? Club { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<EventVote> Votes { get; set; } = new();

    }
}
