using System.ComponentModel.DataAnnotations;
using CampusHaps.Api.Enums;
//stores the information needed when creating an event
//what the frontend sends when creating or updating an event
namespace CampusHaps.Api.DTOs
{
   public class CreateEventDto  {

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime? StartTime { get; set; }

        [Required]
        public DateTime? EndTime { get; set; }

        [Url, MaxLength(500)]
        public string? TicketUrl { get; set; }

        [EnumDataType(typeof(EventType))]
        public EventType Type { get; set; }

        public int? ClubId { get; set; }

  
    }
}