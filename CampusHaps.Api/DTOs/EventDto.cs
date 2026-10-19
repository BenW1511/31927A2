using CampusHaps.Api.Enums;

namespace CampusHaps.Api.DTOs{
    //what the API sends back whenever events are displayed
    //includes calculated values so the frontend doesnt need to do it
    public class EventDto{
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? TicketUrl { get; set; }
        public EventType Type { get; set; }
        public int? ClubId { get; set; }

        //calculated values, not stored in database
        public EventStatus Status { get; set; }
        public string? ClubName { get; set; }
        public int VoteCount { get; set; }
        public int CommentCount { get; set; }
    }
}
