using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Models;
using CampusHaps.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using NUnit.Framework;

namespace CampusHaps.Tests.Tests
{
    [TestFixture] //marks this class as a test class
    public class ClubServiceTests
    {
        private AppDbContext _context = null!; //holds the in-memory database
        private ClubService _service = null!; //holds the service we are testing

        [SetUp] //runs before every test
        public void Setup() //create a new in-memory database for each test
        {
            var options = new DbContextOptionsBuilder<AppDbContext>() //use a unique database name each time
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options); //create the database context
            _service = new ClubService(_context); //create the service with the test database
        }


        [TearDown] //runs after every test
        public void TearDown()
        {
            _context.Dispose(); //clean up the database
        }

        [Test] //marks this method as a test
        public void Create_ValidClub_ReturnsClub()
        {
            //arrange
            var dto = new CreateClubDto //set up the data we need for the test
            {
                Name = "Chess Club",
                Description = "A club for chess players",
                Category = "Games",
                ContactEmail = "chess@example.com"
            };

            //act
            var result = _service.Create(dto); //call the method we are testing

            //assert
            Assert.That(result.Name, Is.EqualTo("Chess Club")); //check the name is correct
            Assert.That(result.Id, Is.Not.EqualTo(0)); //check that an ID was generated
        }

        [Test]
        public void GetById_MissingId_ReturnsNull() //tests that GetById returns null when the club does not exist
        {
            //arrange
            int missingId = 999; //an ID that does not exist

            //act
            var result = _service.GetById(missingId);

            //assert
            Assert.That(result, Is.Null); //should return null when the club is not found
        }

        [Test]
        public void Update_ExistingClub_ChangesFields() //tests that Update changes the fields of an existing club
        {
            //arrange
            var club = _service.Create(new CreateClubDto
            {
                Name = "Chess Club",
                Description = "Original description",
                Category = "Games",
                ContactEmail = "chess@example.com"
            });

            var updateDto = new CreateClubDto
            {
                Name = "University Chess Club",
                Description = "Updated description",
                Category = "Academic",
                ContactEmail = "unichess@example.com"
            };

            //act
            var result = _service.Update(club.Id, updateDto);

            //assert
            Assert.That(result, Is.True); //update should return true
            var updatedClub = _service.GetById(club.Id); //get the club again to check the new values

            Assert.That(updatedClub!.Name, Is.EqualTo("University Chess Club"));
            Assert.That(updatedClub.Description, Is.EqualTo("Updated description"));
            Assert.That(updatedClub.Category, Is.EqualTo("Academic"));
            Assert.That(updatedClub.ContactEmail, Is.EqualTo("unichess@example.com"));
        }

        [Test]
        public void Update_MissingClub_ReturnsFalse() //tests that Update returns false when the club does not exist
        {
            //arrange
            int missingId = 999;

            var updateDto = new CreateClubDto
            {
                Name = "Updated Club",
                Description = "Updated description",
                Category = "Academic",
                ContactEmail = "updated@example.com"
            };

            //act
            var result = _service.Update(missingId, updateDto);

            //assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void Delete_ExistingClub_RemovesIt() //tests that Delete removes an existing club
        {
            //arrange
            var club = _service.Create(new CreateClubDto
            {
                Name = "Chess Club",
                Description = "A club for chess players",
                Category = "Games",
                ContactEmail = "chess@example.com"
            });

            //act
            var result = _service.Delete(club.Id);

            //assert
            Assert.That(result, Is.True); //delete should return true
            var deletedClub = _service.GetById(club.Id); //try to get it again
            Assert.That(deletedClub, Is.Null); //should now return null
        }


        [Test]
        public void AddPost_ValidClubAndAuthor_AddsPost() //tests that AddPost successfully creates a post when the club and author exist
        {
            //arrange
            var club = _service.Create(new CreateClubDto
            {
                Name = "Chess Club",
                Description = "A club for chess players",
                Category = "Games",
                ContactEmail = "chess@example.com"
            });

            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user); //add the user to the database
            _context.SaveChanges();

            var postDto = new CreateClubPostDto
            {
                Title = "Welcome!",
                Content = "Welcome to the Chess Club.",
                AuthorId = user.Id
            };

            //act
            var result = _service.AddPost(club.Id, postDto);

            //assert
            Assert.That(result, Is.Not.Null); //should not return null
            Assert.That(result!.ClubId, Is.EqualTo(club.Id)); //should belong to the correct club
            Assert.That(result.AuthorName, Is.EqualTo("John Smith")); //should have the correct author name
        }

        [Test]
        public void AddPost_ClubDoesNotExist_ReturnsNull() //tests that AddPost returns null when the club does not exist
        {
            //arrange
            int missingClubId = 999;

            var postDto = new CreateClubPostDto
            {
                Title = "Welcome!",
                Content = "This club does not exist.",
                AuthorId = 1
            };

            //act
            var result = _service.AddPost(missingClubId, postDto);

            //assert
            Assert.That(result, Is.Null);
        }


        [Test]
        public void GetPosts_ReturnsNewestFirst() //tests that GetPosts returns the newest posts first
        {
            //arrange
            var club = _service.Create(new CreateClubDto
            {
                Name = "Chess Club",
                Description = "A club for chess players",
                Category = "Games",
                ContactEmail = "chess@example.com"
            });

            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var olderPost = new ClubPost
            {
                ClubId = club.Id,
                AuthorId = user.Id,
                Title = "Older Post",
                Content = "This was posted first.",
                CreatedAt = DateTime.UtcNow.AddHours(-2) //posted 2 hours ago
            };

            var newerPost = new ClubPost
            {
                ClubId = club.Id,
                AuthorId = user.Id,
                Title = "Newer Post",
                Content = "This was posted later.",
                CreatedAt = DateTime.UtcNow //posted just now
            };

            _context.ClubPosts.AddRange(olderPost, newerPost); //add both posts to the database
            _context.SaveChanges();

            //act
            var posts = _service.GetPosts(club.Id)?.ToList();

            //asssert
            Assert.That(posts, Is.Not.Null);
            Assert.That(posts!.Count, Is.EqualTo(2)); //should have 2 posts
            Assert.That(posts[0].Title, Is.EqualTo("Newer Post")); //newest post should be first
            Assert.That(posts[1].Title, Is.EqualTo("Older Post")); //older post should be second
        }

        [Test]
        public void GetPosts_ClubDoesNotExist_ReturnsNull() //tests that GetPosts returns null when the club does not exist
        {
            //arrange
            int missingClubId = 999;

            //act
            var result = _service.GetPosts(missingClubId);

            //assert
            Assert.That(result, Is.Null);
        }
    }
}
