using CampusHaps.Api.DTOs;

namespace CampusHaps.Api.Interfaces
{
    //defines what the ClubService should be able to do
    //contains the method definitions for working with clubs
    public interface IClubService
    {
        IEnumerable<ClubDto> GetAll(); //gets all clubs
        ClubDto? GetById(int id); //gets one club and null if not found
        ClubDto Create(CreateClubDto dto); //creates a new club
        bool Update(int id, CreateClubDto dto); //false if club not found
        bool Delete(int id); //false if club not found

        IEnumerable<ClubPostDto>? GetPosts(int clubId); //null if club not found
        ClubPostDto? AddPost(int clubId, CreateClubPostDto dto); //null if club or author not found
    }
}
