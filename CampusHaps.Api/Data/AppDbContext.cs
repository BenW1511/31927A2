using CampusHaps.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusHaps.Api.Data
{
    //connects the app to the database
    //used to read and save our data
    //should contain DbSets for Users Events Clubs Comments EventVotes and ClubPosts
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } //constructer which recieves db options and then passess them to the base class


        //represents the tables in the database
        public DbSet<User> Users => Set<User>(); //users table
        public DbSet<Event> Events => Set<Event>(); //events table
        public DbSet<Club> Clubs => Set<Club>(); //clubs table
        public DbSet<ClubPost> ClubPosts => Set<ClubPost>(); //club posts table
        public DbSet<Comment> Comments => Set<Comment>(); //comments table
        public DbSet<EventVote> EventVotes => Set<EventVote>(); //event votes table

        protected override void OnModelCreating(ModelBuilder modelBuilder) //tells entity framework how the tables in the database relations to each other and the rules
        {
            base.OnModelCreating(modelBuilder);

            //configures the event entity
            modelBuilder.Entity<Event>() 
                .HasOne(e => e.Club)  
                .WithMany(c => c.Events) 
                .HasForeignKey(e => e.ClubId) 
                .OnDelete(DeleteBehavior.SetNull);
           
            //configures the comment entity
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //configures the eventvote entity
            modelBuilder.Entity<EventVote>()
                .HasOne(v => v.Event)
                .WithMany()
                .HasForeignKey(v => v.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //configures the clubpost entity
            modelBuilder.Entity<ClubPost>()
                .HasOne(p => p.Club)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubPost>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //configures the eventvote entity
            modelBuilder.Entity<EventVote>()
                .HasIndex(v => new { v.UserId, v.EventId }) //creates a database index using the specified fields.
                .IsUnique();

            //seed some test users into the database
            modelBuilder.Entity<User>().HasData( //HasData tells ef core to insert these records when the database is created or migrated 
                new User //first test user
                {
                    Id = 1,
                    Name = "Test Student 1",
                    Email = "student1@example.com",
                    IsVerified = true,
                    PasswordHash = "PLACEHOLDER_HASH_1"
                },
                new User //second test user
                {
                    Id = 2,
                    Name = "Test Studen 2",
                    Email = "student2@exmaple.com",
                    IsVerified = true,
                    PasswordHash = "PLACEHOLDER_HASH_2"
                }
             );

            //add starter/test data for the Club table
            modelBuilder.Entity<Club>().HasData(
                //first test club
                new Club
                {
                    Id = 1,
                    Name = "Ethan Society",
                    Description = "A club for students who enjoy Ethan",
                    Category = "Social",
                    ContactEmail = "ethan@example.com"
                },
                //second test club
                new Club
                {
                    Id = 2,
                    Name = "Nero Society",
                    Description = "A club for students who enjoy Nero",
                    Category = "Social",
                    ContactEmail = "nero@example.com"
                }
            );
            //add starter/test data for the Event table
            modelBuilder.Entity<Event>().HasData(
                //first test event linked to club 1
                new Event
                {
                    Id = 1,
                    Title = "Get to know Ethan Night",
                    Description = "A night to just understand and explore who Ethan is",
                    Location = "Building 2",
                    StartTime = new DateTime(2027, 3, 10, 14, 0, 0),
                    EndTime = new DateTime(2027, 3, 10, 17, 0, 0),
                    TicketUrl = null,
                    Type = Enums.EventType.Social,
                    ClubId = 1,
                },
                //second test event linked to club 2
                new Event
                {
                    Id = 2,
                    Title = "Nero's Gundam building day",
                    Description = "Session to build some Gunpla/Gundam with Nero",
                    Location = "Building 11",
                    StartTime = new DateTime(2027, 3, 12, 10, 0, 0),
                    EndTime = new DateTime(2027, 3, 12, 12, 0, 0),
                    TicketUrl = null,
                    Type = Enums.EventType.Sport,
                    ClubId = 2,
                },
                //third test event with no club
                new Event
                {
                    Id = 3,
                    Title = "Pool Tournament",
                    Description = "Bracket Tournament for 8 ball Pool",
                    Location = "The Underground",
                    StartTime = new DateTime(2027, 3, 15, 18, 0, 0),
                    EndTime = new DateTime(2027, 3, 15, 21, 0, 0),
                    TicketUrl = null,
                    Type = Enums.EventType.Social,
                    ClubId = null,
                }
            );
        }






    }
}