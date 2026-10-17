using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Enums;
using CampusHaps.Api.Extensions;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusHaps.Api.Services
{
    //Handles the main event related logic and is used for getting,creating, updating, deleting and filtering events

    public class EventService : IEventService {

        private readonly AppDbContext _database;

        public EventService(AppDbContext db) { _database = db; }

        //Starting query for events with the associated club, votes and comments loaded so ClubName, VoteCount and CommentCount are correct in the DTO
        private IQueryable<Event> EventsWithDetails() {  return _database.Events.Include(e=>e.Club).Include(e=>e.Votes).Include(e=>e.Comments); }

        public List<EventDto> GetAll(EventType? type, int? clubId)
        {
            var query = EventsWithDetails();

            if (type.HasValue)
            {
                query = query.Where(e=>e.Type == type.Value);
            }
            if (clubId.HasValue)
            {
                query = query.Where(e=>e.ClubId == clubId.Value);
            }
            var events = query
                .OrderBy(e =>e.StartTime)
                .ToList();

            var result = new List<EventDto>();

            foreach(var ev in events)
            {
                result.Add(ev.ToDto());
            }
            return result;
        }

        public EventDto? GetById(int id)
        {
            var ev = EventsWithDetails().FirstOrDefault(e=> e.Id == id);

            if(ev == null)
            {
                return null;
            }
            return ev.ToDto();

        }

        public List<EventDto> GetByDateRange(DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new ArgumentException("The end of the date range must be after the start");
            }
            //only returns events that overlap by selecting ones that start before the range ends and ends before the range starts
            var events = EventsWithDetails()
                .Where(e=>e.StartTime < to && e.EndTime > from)
                .OrderBy(e=>e.StartTime)
                .ToList();

            var result = new List<EventDto>(); 
            
            foreach(var e in events)
            {
                result.Add(e.ToDto());
            
            }
            return result;
        }

        public EventDto Create(CreateEventDto dto)
        {
            ValidateEvent(dto);

            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                StartTime = dto.StartTime!.Value,
                EndTime = dto.EndTime!.Value,
                TicketUrl = dto.TicketUrl,
                Type = dto.Type,
                ClubId = dto.ClubId,
            };

            _database.Events.Add(ev);
            _database.SaveChanges();

            return GetById(ev.Id)!;
        }

        public EventDto? Update(int id, CreateEventDto dto)
        {
            var ev = _database.Events.Find(id);

            if(ev == null)
            {
                return null;
            }

            ValidateEvent(dto);

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Location = dto.Location;
            ev.StartTime = dto.StartTime!.Value;
            ev.EndTime = dto.EndTime!.Value;
            ev.TicketUrl = dto.TicketUrl;
            ev.Type = dto.Type;
            ev.ClubId = dto.ClubId;

            _database.SaveChanges();
            return GetById(id);
        }

        public bool Delete(int id)
        {
            var ev = _database.Events.Find(id);

            if(ev == null)
            {
                return false;

            }

            _database.Events.Remove(ev);
            _database.SaveChanges();
            return true;
        }

        private void ValidateEvent(CreateEventDto dto)
        {
            if(!dto.StartTime.HasValue || !dto.EndTime.HasValue)
            {
                throw new ArgumentException("Start and end times must be entered");
            }
            if(dto.EndTime.Value <= dto.StartTime.Value)
            {
                throw new ArgumentException("The end must be after the start time");
            }

            if(dto.ClubId.HasValue && !_database.Clubs.Any(c => c.Id == dto.ClubId.Value))
            {
                throw new ArgumentException("Entered club doesnt exist");
            }
        }
    }

}
