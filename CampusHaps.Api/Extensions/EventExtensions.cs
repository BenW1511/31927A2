using CampusHaps.Api.DTOs;
using CampusHaps.Api.Enums;
using CampusHaps.Api.Models;

namespace CampusHaps.Api.Extensions{
    //contains useful extra methods for Events
    public static class EventExtensions{
        //retrieves event status by comparing start and end times to the current time
        public static EventStatus GetStatus (this Event e)
        {
            var now = DateTime.Now;

            if(now< e.StartTime)
            {
                return EventStatus.Upcoming;
            }

            if (now<= e.EndTime)
            {
                return EventStatus.Ongoing;
            }

            return EventStatus.Completed;
            
            
                
           }

        //converts an Event into an eventdto the event must be loaded with .Include() for club votes and comments otherwise the clubname will be null and counts will be 0
        public static EventDto ToDto(this Event e) {
            string? clubName;
            if(e.Club != null)
            {
                clubName  = e.Club.Name;
            }
            else
            {
                clubName = null;
            }

                return new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Location = e.Location,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    TicketUrl = e.TicketUrl,
                    Type = e.Type,
                    ClubId = e.ClubId,
                    Status = e.GetStatus(),
                    ClubName = clubName,
                    VoteCount = e.Votes.Count,
                    CommentCount = e.Comments.Count

                };


        }

    }
}
