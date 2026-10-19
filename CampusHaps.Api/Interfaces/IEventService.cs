using CampusHaps.Api.DTOs;
using CampusHaps.Api.Enums;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace CampusHaps.Api.Interfaces
{
    //defines what the EventService should be able to do, implemented by EventService
    
    public interface IEventService
    {
        //gets all events, can be filtered by type or club
        List<EventDto> GetAll(EventType? type, int? clubId);

        //one event or null if it doesnt exist
        EventDto? GetById(int id);

        //events that overlap the date range for the calender view
        List<EventDto> GetByDateRange(DateTime from, DateTime to);

        //creates an event and throws ArgumentException if the club doesnt exist
        EventDto Create(CreateEventDto dto);

        //updates the event or returns null if it doesnt exist
        EventDto? Update(int id, CreateEventDto dto);

        //deletes event, returns false if it doesnt exist
        bool Delete(int id);
    }
}
