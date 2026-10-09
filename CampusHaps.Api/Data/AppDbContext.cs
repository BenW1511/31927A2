using CampusHaps.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusHaps.Api.Data
{
    // Connects the app to the database
    // Used to read and save our data
    // Should contain DbSets for Users Events Clubs Comments EventVotes and ClubPosts
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<ClubPost> ClubPosts => Set<ClubPost>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<EventVote> EventVotes => Set<EventVote>();
    }
}