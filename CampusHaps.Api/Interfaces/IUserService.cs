using CampusHaps.Api.DTOs;

namespace CampusHaps.Api.Interfaces
{
    //Defines what the UserService should be able to do, implemented by UserService
    public interface IUserService
    {
        //Gets all users
        List<UserDto> GetAll();

        //Get one user, or null if they don't exist
        UserDto? GetById(int id);

        //Creates an account with a hashed password
        //Returns null if the email is already registered
        UserDto? Register(RegisterDto dto);

        //Returns the user if the email and password are correct, otherwise null
        UserDto? Login(LoginDto dto);
    }
}
