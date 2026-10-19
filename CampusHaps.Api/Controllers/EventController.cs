using CampusHaps.Api.DTOs;
using CampusHaps.Api.Enums;
using CampusHaps.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHaps.Api.Controllers
{
    //handles requests from the frontend related to events
    //used to get create update delete and filter events
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _events;

        public EventController(IEventService events)
        {
            _events = events;
        }

        [HttpGet]
        public IActionResult GetAll(EventType? type, int? ClubId)
        {
            return Ok(_events.GetAll(type, ClubId));
        }

        //get /api/events/5, id should only match numbers so caledndar isnt accidentally taken as an ID
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var e = _events.GetById(id);

            if(e == null)
            {
                return NotFound();
            }
            return Ok(e);
        }
        //get /api/events/calendar
        [HttpGet("calendar")]
        public IActionResult GetByDateRange(DateTime from, DateTime to)
        {
            try
            {
                return Ok(_events.GetByDateRange(from, to));
            }
            catch(ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }
        //post /api/events/5
        [HttpPost]
        public IActionResult Create(CreateEventDto dto)
        {
            try
            {
                var created = _events.Create(dto);
                return Ok(created);
            }
            catch(ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }
        //Put /api/events/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, CreateEventDto dto)
        {
            try
            {
                var updated = _events.Update(id, dto);

                if(updated == null)
                {
                    return NotFound();

                }
                return Ok(updated); 
            }

            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);  
            }
        }

        //Delete /api/events/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _events.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }




    }
}
