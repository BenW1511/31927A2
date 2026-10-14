using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHaps.Api.Controllers
{
    //Handles requests from the frontend related to comments
    //Used to add get update and delete comments

    [ApiController]
    [Route("api/events/{eventId}/comments")] //sets the base URL path for all endpoints in this controller
    public class CommentController :ControllerBase
    {
        private readonly ICommentService _comments; //holds a reference to the comment service so we can call its methods

        public CommentController(ICommentService comments) //injects the comment service when the controller is created
        {
            _comments = comments;
        }

        [HttpGet]
        public IActionResult GetForEvent(int eventId)
        {
            var comments = _comments.GetForEvent(eventId); //call the service to get all comments for this event

            if (comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }

        [HttpPost]
        public IActionResult Add(int eventId, CreateCommentDto dto)
        {
            var comment = _comments.Add(eventId, dto); //call the service to add a new comment

            if (comment == null)
            {
                return BadRequest("Could not create the comment. Check that the event and user exist and the content is not empty.");
            }

            return Ok(comment);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int eventId, int id, CreateCommentDto dto)
        {
            var updated = _comments.Update(id, dto); //call the service to update the comment

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int eventId, int id, [FromQuery] int userId)
        {
            var deleted = _comments.Delete(id, userId); //call the service to delete the comment

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}

