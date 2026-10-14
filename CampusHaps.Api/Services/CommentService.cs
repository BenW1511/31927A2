using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Models;

namespace CampusHaps.Api.Services
{
    //handles the main comment related logic
    //used for adding getting updating and deleting comments
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context; //holds the database context allowing to talk to the database

        public CommentService(AppDbContext context) //injects the database context when the service is created
        {
            _context = context;
        }

        public IEnumerable<CommentDto>? GetForEvent(int eventId) //gets all comments for an event
        {
            var eventExists = _context.Events.Any(e => e.Id == eventId); //event exists check

            if (!eventExists)
            {
                return null;
            }

            return _context.Comments //gets all comments belonging to the event and map them to comment dto
                .Where(c => c.EventId == eventId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    EventId = c.EventId
                })
                .ToList(); //execute the query and return a list
        }

        
        public CommentDto? Add(int eventId, CreateCommentDto dto) //adds a comment to an event
        {
            var eventExists = _context.Events.Any(e => e.Id == eventId); //event and user exist check
            var userExists = _context.Users.Any(u => u.Id == dto.UserId);

            if (!eventExists || !userExists ||
                string.IsNullOrWhiteSpace(dto.Content))
            {
                return null;
            }

            var comment = new Comment //creates a new comment entity
            {
                EventId = eventId,
                UserId = dto.UserId,
                Content = dto.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment); //adds the new comment to the database
            _context.SaveChanges(); 

            return _context.Comments //return the newly created comment as a dto
                .Where(c => c.Id == comment.Id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    EventId = c.EventId
                })
                .First();
        }


        public bool Update(int id, CreateCommentDto dto) //updates a comment if it exists and belongs to the specified user
        {
            var comment = _context.Comments.Find(id); //finds comment first by id

            if (comment == null ||
                comment.UserId != dto.UserId ||
                string.IsNullOrWhiteSpace(dto.Content))
            {
                return false;
            }

            comment.Content = dto.Content.Trim(); //update the comment content 

            _context.SaveChanges();

            return true;
        }

      
        public bool Delete(int id, int userId)   //deletes a comment if it exists and belongs to the specified user
        {
            var comment = _context.Comments.Find(id);

            if (comment == null || comment.UserId != userId)
            {
                return false;
            }

            _context.Comments.Remove(comment); //remove the comment from the database
            _context.SaveChanges();

            return true;
        }
    }
}
