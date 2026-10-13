using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Models;

namespace CampusHaps.Api.Services
{
    //handles the main club related logic
    //used for getting creating updating and deleting clubs
    public class ClubService : IClubService
    {
        private readonly AppDbContext _context; //holds the database context allowing to talk to the database

        public ClubService(AppDbContext context) //injects the database context when the service is created
        {
            _context = context;
        }

        public IEnumerable<ClubDto> GetAll() //gets all clubs and maps them to club dto
        {
            return _context.Clubs //query all clubs from the database
                .Select(club => new ClubDto //convert each club entity into a club dto
                {
                    Id = club.Id,
                    Name = club.Name,
                    Description = club.Description,
                    Category = club.Category,
                    ContactEmail = club.ContactEmail,
                    EventCount = club.Events.Count,
                    PostCount = club.Posts.Count
                })
                .ToList(); //execute the query and return a list
        }

        public ClubDto? GetById(int id) //gets a single club by id and maps it to club dto
        {
            var club = _context.Clubs //find the club with the matching id and convert it to a dto
                .Where(c => c.Id == id) //filter by id
                .Select(c => new ClubDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Category = c.Category,
                    ContactEmail = c.ContactEmail,
                    EventCount = c.Events.Count,
                    PostCount = c.Posts.Count
                })
                .FirstOrDefault(); //this gets the first match or else its null

            return club;
        }

        public ClubDto Create(CreateClubDto dto) //creates a new club from the dto and saves
        {
            var club = new Club //new Club entity created from data in the dto
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                ContactEmail = dto.ContactEmail
            };

            _context.Clubs.Add(club); //adds the club to the db
            _context.SaveChanges();

            return GetById(club.Id)!; //returns the new club as a dto
        }

        public bool Update(int id, CreateClubDto dto) //updates existing club with the new data
        {
            var club = _context.Clubs.Find(id);

            if (club == null)
            {
                return false;
            }

            //updates the club with new values
            club.Name = dto.Name;
            club.Description = dto.Description;
            club.Category = dto.Category;
            club.ContactEmail = dto.ContactEmail;

            _context.SaveChanges();

            return true;
        }

        public bool Delete(int id) //deletes a club from its id
        {
            var club = _context.Clubs.Find(id); //find club from id

            if (club == null)
            {
                return false;
            }

            _context.Clubs.Remove(club); //remove club from db
            _context.SaveChanges(); 

            return true;
        }

        public IEnumerable<ClubPostDto>? GetPosts(int clubId) //gets all posts for a specific club
        {
            var clubExists = _context.Clubs.Any(c => c.Id == clubId);

            if (!clubExists) //if club doesnt exist then null
            {
                return null;
            }

            return _context.ClubPosts //gets all posts from this club and map them to the dto
                .Where(p => p.ClubId == clubId) //filters post by the club id
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ClubPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    ClubId = p.ClubId,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name //get users name
                })
                .ToList(); //execute query and return list
        }

        public ClubPostDto? AddPost(int clubId, CreateClubPostDto dto) //adds a new post to a club
        {
            var clubExists = _context.Clubs.Any(c => c.Id == clubId); //check club exists
            var authorExists = _context.Users.Any(u => u.Id == dto.AuthorId); //check if user exists

            if (!clubExists || !authorExists || string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content)) //if either the club or author does not exist it returns null
            {
                return null;
            }

            var post = new ClubPost //create a new club post entity
            {
                ClubId = clubId,
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow //set to time right now
            };

            _context.ClubPosts.Add(post); //add the post to db
            _context.SaveChanges(); //save

            return _context.ClubPosts //rturn the newly created post as a DTO
                .Where(p => p.Id == post.Id)
                .Select(p => new ClubPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    ClubId = p.ClubId,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name
                })
                .First();
        }

    }


}
