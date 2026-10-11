using System.ComponentModel.DataAnnotations;
using CampusHaps.Api.Enums;
//Holds information needed when creating an event
//What the frontend sends when creating or updating an event
namespace CampusHaps.Api.DTOs
{
   public class CreateEventDto : IValidatableObject {

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

        //Checks that the end time is not before the start time to validate input, it is also validated by the attributes above each property
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext){
            if (StartTime.HasValue && EndTime.HasValue && EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "End time must be after start time.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}