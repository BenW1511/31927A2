using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHaps.Api.Controllers
{
    //handles requests from the frontend related to clubs
    //used to get create update and delete clubs

    [ApiController] //marks this class as a an api controller
    [Route("api/clubs")] //sets the base url path for all endpoints onto this controller
    public class ClubsController : ControllerBase
    {

        private readonly IClubService _clubs; //holds a reference to the club service to call its methods

        public ClubsController(IClubService clubs) //framework gives controller an instance of clubservice interface when created
        {
            _clubs = clubs;
        }

        [HttpGet] //this endpoint responds to GET requests
        public IActionResult GetAll() 
        {
            var clubs = _clubs.GetAll(); //call the service to get all clubs

            return Ok(clubs); //return the list of clubs with a 200 ok response
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) //call the service to get the club by ID
        {
            var club = _clubs.GetById(id); 

            if (club == null)
            {
                return NotFound();
            }

            return Ok(club);
        }

        [HttpPost]
        public IActionResult Create(CreateClubDto dto)
        {
            var club = _clubs.Create(dto); //call the service to create a new club

            return CreatedAtAction(
                nameof(GetById),
                new { id = club.Id },
                club
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CreateClubDto dto)
        {
            var updated = _clubs.Update(id, dto); //call the service to update the club

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _clubs.Delete(id); //call the service to delete the club

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/posts")]
        public IActionResult GetPosts(int id)
        {
            var posts = _clubs.GetPosts(id); //call the service to get the posts for this club

            if (posts == null)
            {
                return NotFound();
            }

            return Ok(posts);
        }

        [HttpPost("{id}/posts")]
        public IActionResult AddPost(int id, CreateClubPostDto dto)
        {
            var post = _clubs.AddPost(id, dto); //call the service to add a new post

            if (post == null)
            {
                return BadRequest("Could not create the post. Check that the club and author exist and the title and content are not empty.");
            }

            return Ok(post); 
        }
    }

} 
