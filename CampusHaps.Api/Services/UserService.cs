using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Models;

namespace CampusHaps.Api.Services
{
    //Handles the main user related logic, used for creating finding and managing users
    public class UserService : IUserService
    {
        private readonly AppDbContext _database;

        public UserService(AppDbContext database) {

            _database = database;
        }

        public List<UserDto> GetAll()
        {
            return _database.Users.OrderBy(u => u.Name).ToList().Select(u => ToDto(u)).ToList();
        }

        public UserDto? GetById(int id)
        {
            var user = _database.Users.Find(id);

            if (user == null)
            {
                return null;

            }
            return ToDto(user);
        }

        public UserDto? Register(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            //duplicate emails are rejected
            if (_database.Users.Any(u => u.Email == email))
            {
                return null;
            }

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                IsVerified = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)

            };

            _database.Users.Add(user);
            _database.SaveChanges();

            return ToDto(user);
        }

        public UserDto? Login(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = _database.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {

                return null;
            }
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    return null;
                }
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return null;
            }

            return ToDto(user);
        }
            //method to convert user into a userDto
            private static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsVerified = user.IsVerified,
            };
        
        
        }

    }
}
