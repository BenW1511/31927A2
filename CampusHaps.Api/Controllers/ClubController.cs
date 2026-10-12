using Microsoft.AspNetCore.Mvc;

namespace CampusHaps.Api.Controllers
{
    // Handles requests from the frontend related to clubs
    // Used to get create update and delete clubs

    [ApiController] //identifies the class as an API controller
    [Route("api/[controller]")] //sets the URL to /api/clubs
    public class ClubsController : ControllerBase
    {
        [HttpGet] //means the endpoint responds to HTTP GET requests
        public IActionResult GetClubs()
        {
            return Ok("Clubs endpoint is working"); //returns a successful HTTP 200 response with a test message
        }
    }
} 
