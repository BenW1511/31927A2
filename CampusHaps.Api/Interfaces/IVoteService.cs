namespace CampusHaps.Api.Interfaces
{
    //Defines what the VoteService should be able to do, implemented by VoteService
    
    public interface IVoteService
    {
        //Adds a vote, returns false if the event or user doesnt exist or if the user already voted
        bool AddVote(int eventId, int userId);

        //Removes a vote, returns false if the user hasnt voted
        bool RemoveVote(int eventId, int userId);

        //True if the user has already voted on this event
        bool HasVoted(int eventId, int userId);

        //Number of votes on an event
        int GetCount(int eventId);
    }
}
