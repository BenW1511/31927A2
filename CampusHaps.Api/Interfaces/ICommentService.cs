using CampusHaps.Api.DTOs;

namespace CampusHaps.Api.Interfaces
{
    //defines what the CommentService should be able to do
    //contains the method definitions for working with comments
    public interface ICommentService
    {
        IEnumerable<CommentDto>? GetForEvent(int eventId); //null if event not found
        CommentDto? Add(int eventId, CreateCommentDto dto); //null if event/user not found or empty content
        bool Update(int id, CreateCommentDto dto); //false if not found or not the owner
        bool Delete(int id, int userId); //false if not found or not the owner
    }
}
