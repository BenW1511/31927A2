using CampusHaps.Api.DTOs;

namespace CampusHaps.Api.Interfaces
{
    //defines what the UserService should be able to do, implemented by UserService
    public interface IUserService
    {
        //gets all users
        List<UserDto> GetAll();

        //get one user, or null if they don't exist
        UserDto? GetById(int id);

        //creates an account with a hashed password
        //returns null if the email is already registered
        UserDto? Register(RegisterDto dto);

        //returns the user if the email and password are correct, otherwise null
        UserDto? Login(LoginDto dto);
    }
}
