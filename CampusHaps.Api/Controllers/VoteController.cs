using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace CampusHaps.Api.Controllers
{
    // Handles requests from the frontend related to votes, used to add remove and check votes

    [ApiController]
    [Route("api/events/{eventId}/votes")]
    public class VoteController : ControllerBase
    {

        private readonly IVoteService _votes;

        public VoteController(IVoteService votes)
        {
            _votes = votes;
        }

        //Get /api/events/5/votes and returns the vote count and if the user voted
        [HttpGet]
        public IActionResult GetVotes(int eventId, int? userId)
        {
            var count = _votes.GetCount(eventId);

            var hasVoted = false;
            if (userId.HasValue)
            {
                hasVoted = _votes.HasVoted(eventId, userId.Value); 
            }
            //returns JSON object with count and hasVoted boolean
            return Ok(new {count, hasVoted});

        }

        [HttpPost]
        public IActionResult AddVote(int eventId, VoteDto dto)
        {
            var added = _votes.AddVote(eventId, dto.UserId);

            if (!added)
            {
                return BadRequest("Vote not added. Event/User does not exist or user has voted");
            }

            var count = _votes.GetCount(eventId);
            return Ok(new { count, hasVoted = true });
        } 

        [HttpDelete]
        public IActionResult RemoveVote (int eventId, int userId)
        {
            var removed = _votes.RemoveVote(eventId, userId);

            if (!removed)
            {
                return NotFound();
            }

            var count = _votes.GetCount(eventId);
            return Ok(new { count, hasVoted = false });
        }




    }
}
