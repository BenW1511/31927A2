#nullable disable
using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Enums;
using CampusHaps.Api.Extensions;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Models;
using CampusHaps.Api.Services;
using Microsoft.EntityFrameworkCore;
//Tests EventService and event status calculations, checks creating, getting, filtering updated and deleting events as well as invalid input
namespace CampusHaps.Tests.Tests
{

    [TestFixture]
    public class EventServiceTests
    {
        private AppDbContext _database;
        private EventService _service;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _database = new AppDbContext(options);

            Club club = new Club
            {
                Id = 1,
                Name = "test club",
                Description = "testing club",
                Category = "Social",
                ContactEmail = "club@example.com"

            };

            _database.Clubs.Add(club);
            _database.SaveChanges();

            _service = new EventService(_database);

        }

        [TearDown]
        public void TearDown()
        {
            _database.Dispose();
        }
        //Helper method to make a valid event for the club made above (id:1)
        private CreateEventDto MakeEventDto(DateTime start, DateTime end)
        {
            CreateEventDto dto = new CreateEventDto();
            dto.Title = "test event";
            dto.Description = "event for testing";
            dto.Location = "Building 1";
            dto.StartTime = start;
            dto.EndTime = end;
            dto.Type = EventType.Social;
            dto.ClubId = 1;
            return dto;
        }

        [Test]
        public void CreateValidEventSavesEvent()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026, 11, 14, 12, 0, 0), new DateTime(2026, 11, 14, 14, 0, 0));

            EventDto created = _service.Create(dto);

            Assert.IsTrue(created.Id > 0, "Event must have an Id");
            Assert.AreEqual("test event", created.Title);
            Assert.AreEqual("test club", created.ClubName);
            Assert.AreEqual(1, _database.Events.Count(), "There must be one event in the database");


        }

        [Test]
        public void CreateEndBeforeStartFails()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026,11,14, 12, 0, 0), new DateTime(2026,11,14,10,0, 0));

            Assert.Throws<ArgumentException>(() => _service.Create(dto));
            Assert.AreEqual(0, _database.Events.Count(), "The invalid event mustnt be saved");
        }

        [Test]
        public void CreateClubDoesNotExistFails()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026,11,14, 10, 0, 0), new DateTime(2026,11,14, 12, 0, 0));
            dto.ClubId = 99;

            Assert.Throws<ArgumentException>(() => _service.Create(dto));
        }

        [Test]
        public void GetByIDExistingEventReturnsEvent()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026, 11, 14, 12, 0, 0), new DateTime(2026, 11, 14, 14, 0, 0));
            EventDto created = _service.Create(dto);

            EventDto found = _service.GetById(created.Id);

            Assert.NotNull(found);
            Assert.AreEqual("test event", found.Title);
        }

        [Test]
        public void GetByIDMissingEventReturnsNull()
        {
            EventDto found = _service.GetById(999);

            Assert.Null(found);
        }
        //test to ensure filtering by type works
        [Test]
        public void GetAllFilterByType()
        {
            CreateEventDto socialEvent = MakeEventDto(new DateTime(2026, 11, 14, 10, 0, 0), new DateTime(2026, 11, 14, 12, 0, 0));
            socialEvent.Title = "Social Event";
            socialEvent.Type = EventType.Social;
            _service.Create(socialEvent);

            CreateEventDto sportEvent = MakeEventDto(new DateTime(2026, 11, 15, 10, 0, 0), new DateTime(2026, 11, 15, 12, 0, 0));
            sportEvent.Title = "Sport Event";
            sportEvent.Type = EventType.Sport;
            _service.Create(sportEvent);

            List<EventDto> result = _service.GetAll(EventType.Sport, null);

            Assert.AreEqual(1, result.Count, "Only sport event must be returned");
            Assert.AreEqual("Sport Event", result[0].Title);
        }
        //test to make sure the get by date range only returns overlapping events
        [Test]
        public void GetByDateRangeOverlap()
        {
            CreateEventDto inside = MakeEventDto(new DateTime(2026, 11, 14, 12, 0, 0), new DateTime(2026, 11, 14, 14, 0, 0));
            inside.Title = "Inside";
            _service.Create(inside);

            CreateEventDto overlapping = MakeEventDto(new DateTime(2026, 11, 13, 22, 0, 0), new DateTime(2026, 11, 14, 2, 0, 0));
            overlapping.Title = "Overlapping";
            _service.Create(overlapping);

            CreateEventDto before = MakeEventDto(new DateTime(2026, 11, 13, 10, 0, 0), new DateTime(2026, 11, 13, 12, 0, 0));
            before.Title = "Before";
            _service.Create(before);

            CreateEventDto after = MakeEventDto(new DateTime(2026, 11, 15, 10, 0, 0), new DateTime(2026, 11, 15, 12, 0, 0));
            after.Title = "After";
            _service.Create(after);

            List<EventDto> result = _service.GetByDateRange(new DateTime(2026, 11, 14), new DateTime(2026, 11, 15));

            Assert.AreEqual(2, result.Count, "Only two events on 14 November must be returned");
            Assert.AreEqual("Overlapping", result[0].Title);
            Assert.AreEqual("Inside", result[1].Title);

        }

        [Test]
        public void GetByDateRangeEndBeforeStart()
        {
            DateTime from = new DateTime(2026, 12, 1);
            DateTime to = new DateTime(2026, 11, 1);
            Assert.Throws<ArgumentException>(() => _service.GetByDateRange(from,to));
        }
        //tests if updating changes titles
        [Test]
        public void UpdateEventTitleChange()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 12, 0, 0));
            EventDto created = _service.Create(dto);
            dto.Title = "Updated Title";

            EventDto updated = _service.Update(created.Id, dto);
            Assert.NotNull(updated);
            Assert.AreEqual("Updated Title", updated.Title);
        }
        //test to see if updating a non existing event returns null
        [Test]
        public void UpdateMissingEvent()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 12, 0, 0));
            EventDto updated = _service.Update(999,dto);

            Assert.Null(updated);
        }

        [Test]
        public void DeleteEvent()
        {
            CreateEventDto dto = MakeEventDto(new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 12, 0, 0));
            EventDto created = _service.Create(dto);

            bool deleted = _service.Delete(created.Id);
            Assert.IsTrue(deleted);
            Assert.Null(_service.GetById(created.Id), "The event must no longer exist");
        }
        //tests to see if deleting amissing event returns false
        [Test]
        public void DeleteMissingEvent()
        {
            bool deleted = _service.Delete(99);
            Assert.IsFalse(deleted);
        }

        [Test]
        public void GetStatusUpcomingEvent()
        {
            Event e  = new Event();
            e.StartTime = DateTime.Now.AddDays(1);
            e.EndTime = DateTime.Now.AddDays(3);    
            EventStatus status = e.GetStatus();
            Assert.AreEqual(EventStatus.Upcoming, status);
        }

        [Test]
        public void GetStatusCurrentEventIsOngoing()
        {
            Event e = new Event();
            e.StartTime = DateTime.Now.AddHours(-1);
            e.EndTime= DateTime.Now.AddHours(1);

            EventStatus status = e.GetStatus();
            Assert.AreEqual(EventStatus.Ongoing, status);
        }
        //test if a past event's status is completed
        [Test]
        public void GetStatusEventCompleted()
        {
            Event e = new Event();
            e.StartTime = DateTime.Now.AddDays(-2);
            e.EndTime = DateTime.Now.AddDays(-1);
            EventStatus status = e.GetStatus();
            Assert.AreEqual(EventStatus.Completed, status);
        }


    }
}
