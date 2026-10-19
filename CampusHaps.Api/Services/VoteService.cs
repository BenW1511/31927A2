using CampusHaps.Api.Data;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Models;

namespace CampusHaps.Api.Services
{
    //handles the main voting logic this is used for adding and removing votes and checking if someone has already voted
    public class VoteService : IVoteService
    {
        private readonly AppDbContext _database;

        public VoteService(AppDbContext database)
        {

            _database = database;
        }

        public bool AddVote(int eventID, int userID)
        {
            //events and user must exist and the user can only vote once for each event
            if(!_database.Events.Any(e=>e.Id == eventID))
            {
                return false;
            }
            if(!_database.Users.Any(u=>u.Id == userID))
            {
                return false;
            }
            if(HasVoted(eventID, userID))
            {
                return false;
            }

            var vote = new EventVote
            {
                EventId = eventID,
                UserId = userID
            };

            _database.EventVotes.Add(vote);
            _database.SaveChanges();
            return true;
        }

        public bool RemoveVote(int eventId, int userId)
        {
            var vote = _database.EventVotes.FirstOrDefault(v=>v.EventId == eventId &&  v.UserId == userId);

            if(vote == null)
            {
                return false;
            }

            _database.EventVotes.Remove(vote);
            _database.SaveChanges();
            return true;
        }

        public bool HasVoted (int eventID, int userID)
        {
            return _database.EventVotes.Any(v => v.EventId == eventID && v.UserId == userID);
        }

        public int GetCount(int eventID)
        {
            return _database.EventVotes.Count(v => v.EventId == eventID); 
        }

    }

}
