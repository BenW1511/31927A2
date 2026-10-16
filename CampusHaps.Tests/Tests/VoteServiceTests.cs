#nullable disable
using CampusHaps.Api.Data;
using CampusHaps.Api.Models;
using CampusHaps.Api.Services;
using Microsoft.EntityFrameworkCore;
//Tests for VoteService, checks adding and removing votes, duplicate votes, missing event or user votes
namespace CampusHaps.Tests.Tests
{
    [TestFixture]
    public class VoteServiceTests
    {
        private AppDbContext _database;
        private VoteService _service;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _database = new AppDbContext(options);

            User user = new User
            {
                Id = 1,
                Name = "test user",
                Email = "test@student.uts.edu.au",
                PasswordHash = "not used in the tests"
            };

            Event e = new Event
            {
                Id = 1,
                Title = "test event",
                Location = "Building 1",
                StartTime = new DateTime(2026, 11, 14, 12, 0, 0),
                EndTime = new DateTime(2026, 11, 14, 14, 0, 0)
            };

            _database.Users.Add(user);
            _database.Events.Add(e);
            _database.SaveChanges();
            _service = new VoteService(_database);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Dispose();
        }

        //tests that valid vote is added and counted
        [Test]
        public void AddValidVote()
        {
            bool added = _service.AddVote(1, 1);
            Assert.IsTrue(added);
            Assert.AreEqual(1, _service.GetCount(1), "The event must have one vote");
        }

        [Test]
        public void AddVoteTwice()
        {
            _service.AddVote(1, 1);
            bool secondVote = _service.AddVote(1, 1);

            Assert.IsFalse(secondVote, "User cant vote twice");
            Assert.AreEqual(1, _service.GetCount(1));
        }

        [Test]
        public void AddVoteMissingEventFails()
        {
            bool added = _service.AddVote(9999, 1);
            Assert.IsFalse(added);
        }

        //test to make sure that missing user cant vote
        [Test]
        public void AddVoteMissingUser()
        {
            bool added = _service.AddVote(1, 9999);
            Assert.IsFalse(added);


        }

        [Test]
        public void RemoveVoteCountZero()
        {
            _service.AddVote(1, 1);
            bool removed = _service.RemoveVote(1, 1);
            Assert.IsTrue(removed);
            Assert.AreEqual(0, _service.GetCount(1), "The vote must be removed");
        }

        //tests that removing a vote which was never made returns false
        [Test]
        public void RemoveVoteNotMade()
        {
            bool removed = _service.RemoveVote(1, 1);
            Assert.IsFalse(removed);
        }

        [Test]
        public void HasVotedAfterVoting()
        {
            _service.AddVote(1, 1);

            bool hasVoted = _service.HasVoted(1, 1);

            Assert.IsTrue(hasVoted);
        }

        [Test]
        public void HasVotedWithoutVoting()
        {
            bool hasVoted = _service.HasVoted(1, 1);
            Assert.IsFalse(hasVoted);
        }


    }
}
