using CampusHaps.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        }






    }
}