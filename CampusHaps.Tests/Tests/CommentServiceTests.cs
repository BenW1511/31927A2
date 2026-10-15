using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Enums;
using CampusHaps.Api.Models;
using CampusHaps.Api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace CampusHaps.Tests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private AppDbContext _context = null!; //holds the in-memory database
        private CommentService _service = null!; //holds the service we are testing

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) //use a unique database name each time
                .Options;

            _context = new AppDbContext(options); //create the database context
            _service = new CommentService(_context); //create the service with the test database
        }

        [TearDown] //runs after every test
        public void TearDown()
        {
            _context.Dispose(); //clean up the database
        }


        [Test]
        public void Add_Valid_ReturnsCommentWithUserName() //tests that Add creates a comment and includes the users name
        {
            //arrange
            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            //arrange
            var eventItem = new Event
            {
                Title = "Campus Meetup",
                Description = "Meet other students",
                Location = "UTS",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Type = EventType.Social
            };

            _context.Events.Add(eventItem);
            _context.SaveChanges();

            var commentDto = new CreateCommentDto
            {
                UserId = user.Id,
                Content = "Looking forward to this!"
            };

            //act
            var result = _service.Add(eventItem.Id, commentDto);

            //assert
            Assert.That(result, Is.Not.Null); //should not return null
            Assert.That(result!.Content, Is.EqualTo("Looking forward to this!"));
            Assert.That(result.UserName, Is.EqualTo("John Smith")); //should include the users name
        }


        [Test]
        public void Add_EventDoesNotExist_ReturnsNull() //tests that Add returns null when the event does not exist
        {
            //arrange
            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var commentDto = new CreateCommentDto
            {
                UserId = user.Id,
                Content = "Is this event still happening?"
            };

            int missingEventId = 999; //an event ID that does not exist

            //act
            var result = _service.Add(missingEventId, commentDto);

            //assert
            Assert.That(result, Is.Null);
        }


        [Test]
        public void Add_EmptyContent_ReturnsNull() //tests that Add returns null when the content is empty or only whitespace
        {
            //arrange
            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            //arrange
            var eventItem = new Event
            {
                Title = "Campus Meetup",
                Description = "Meet other students",
                Location = "UTS",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Type = EventType.Social
            };

            _context.Events.Add(eventItem);
            _context.SaveChanges();

            var commentDto = new CreateCommentDto
            {
                UserId = user.Id,
                Content = "   " //content is only whitespace
            };

            //act
            var result = _service.Add(eventItem.Id, commentDto);

            //assert
            Assert.That(result, Is.Null);
        }


        [Test]
        public void Update_OwnComment_ReturnsTrue() //tests that Update succeeds when the user owns the comment
        {
            // Arrange: create a user
            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            //arrange
            var eventItem = new Event
            {
                Title = "Campus Meetup",
                Description = "Meet other students",
                Location = "UTS",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Type = EventType.Social
            };

            _context.Events.Add(eventItem);
            _context.SaveChanges();

            //arrange
            var comment = new Comment
            {
                UserId = user.Id,
                EventId = eventItem.Id,
                Content = "The original comment",
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            var updateDto = new CreateCommentDto
            {
                UserId = user.Id,
                Content = "My updated comment"
            };

            //act
            var result = _service.Update(comment.Id, updateDto);

            //assert
            Assert.That(result, Is.True); //should return true

            var updatedComment = _context.Comments.Find(comment.Id);
            Assert.That(updatedComment!.Content, Is.EqualTo("My updated comment"));
        }


        [Test]
        public void Update_SomeoneElsesComment_ReturnsFalse() //tests that Update returns false when trying to edit someone elses comment
        {
            //arrange
            var owner = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            var otherUser = new User
            {
                Name = "Jane Smith",
                Email = "jane@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.AddRange(owner, otherUser);
            _context.SaveChanges();

            //arrange
            var eventItem = new Event
            {
                Title = "Campus Meetup",
                Description = "Meet other students",
                Location = "UTS",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Type = EventType.Social
            };

            _context.Events.Add(eventItem);
            _context.SaveChanges();

            //arrange
            var comment = new Comment
            {
                UserId = owner.Id,
                EventId = eventItem.Id,
                Content = "John's original comment",
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            var updateDto = new CreateCommentDto
            {
                UserId = otherUser.Id, //user trying to edit another users comment
                Content = "Trying to change John's comment"
            };

            //act
            var result = _service.Update(comment.Id, updateDto);

            //assert
            Assert.That(result, Is.False);

            var unchangedComment = _context.Comments.Find(comment.Id);
            Assert.That(
                unchangedComment!.Content,
                Is.EqualTo("John's original comment"));
        }


        [Test]
        public void Delete_OwnComment_RemovesIt() //tests that Delete removes a comment when the user owns it
        {
            //arrange
            var user = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            //arrange
            var eventItem = new Event
            {
                Title = "Campus Meetup",
                Description = "Meet other students",
                Location = "UTS",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Type = EventType.Social
            };

            _context.Events.Add(eventItem);
            _context.SaveChanges();

            //arrange
            var comment = new Comment
            {
                UserId = user.Id,
                EventId = eventItem.Id,
                Content = "This is my comment",
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            //act
            var result = _service.Delete(comment.Id, user.Id);

            //assert
            Assert.That(result, Is.True); //should return true
            Assert.That(_context.Comments.Find(comment.Id), Is.Null); //comment should no longer exist
        }


        [Test]
        public void Delete_SomeoneElsesComment_ReturnsFalse() //tests that Delete returns false when trying to delete someone elses comment
        {
            //arrange
            var owner = new User
            {
                Name = "John Smith",
                Email = "john@example.com",
                PasswordHash = "test-hash"
            };

            var otherUser = new User
            {
                Name = "Jane Smith",
                Email = "jane@example.com",
                PasswordHash = "test-hash"
            };

            _context.Users.AddRange(owner, otherUser);
            _context.SaveChanges();

            //arrange
            var eventItem = new Event
            {
                Title = "Campus Meetup",
                Description = "Meet other students",
                Location = "UTS",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Type = EventType.Social
            };

            _context.Events.Add(eventItem);
            _context.SaveChanges();

            //arrange
            var comment = new Comment
            {
                UserId = owner.Id,
                EventId = eventItem.Id,
                Content = "John's original comment",
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            //act
            var result = _service.Delete(comment.Id, otherUser.Id);

            //assert
            Assert.That(result, Is.False);

            var unchangedComment = _context.Comments.Find(comment.Id);
            Assert.That(unchangedComment, Is.Not.Null);
            Assert.That(
                unchangedComment!.Content,
                Is.EqualTo("John's original comment"));
        }









    }
}