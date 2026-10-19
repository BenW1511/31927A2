namespace CampusHaps.Api.Interfaces
{
    //defines what the VoteService should be able to do, implemented by VoteService
    
    public interface IVoteService
    {
        //adds a vote returns false if the event or user doesnt exist or if the user already voted
        bool AddVote(int eventId, int userId);

        //removes a vote, returns false if the user hasnt voted
        bool RemoveVote(int eventId, int userId);

        //true if the user has already voted on this event
        bool HasVoted(int eventId, int userId);

        //number of votes on an event
        int GetCount(int eventId);
    }
}
