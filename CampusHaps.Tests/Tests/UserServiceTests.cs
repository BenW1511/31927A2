#nullable disable
using CampusHaps.Api.Data;
using CampusHaps.Api.DTOs;
using CampusHaps.Api.Models;
using CampusHaps.Api.Services;
using Microsoft.EntityFrameworkCore;
//Tests for UserService, checks registering, loggin in, duplcate emails and ensures that passwords are saved as hashes
namespace CampusHaps.Tests.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private AppDbContext _database;
        private UserService _service;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _database = new AppDbContext(options);
            _service = new UserService(_database);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Dispose();
        }

        //helper method to make a valid registration
        private RegisterDto MakeRegisterDto()
        {
            RegisterDto dto = new RegisterDto();
            dto.Name = "test user";
            dto.Email = "test@student.uts.edu.au";
            dto.Password = "Password412";
            return dto;
        }


        [Test]
        public void RegisterNewUser()
        {
            RegisterDto dto = MakeRegisterDto();
            UserDto user = _service.Register(dto);

            Assert.NotNull(user);
            Assert.AreEqual("test@student.uts.edu.au", user.Email);
            Assert.IsFalse(user.IsVerified, "New users must begin as unverified");
        }
        //tests that the password is saved as hash
        [Test]
        public void RegisterPasswordSavedHash()
        {
            RegisterDto dto = MakeRegisterDto();
            _service.Register(dto);
            User saved = _database.Users.FirstOrDefault(u => u.Email == "test@student.uts.edu.au");

            Assert.NotNull(saved);
            Assert.AreNotEqual("Password412", saved.PasswordHash, "Plain password must not be saved");
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("Password412", saved.PasswordHash));
        }

        [Test]
        public void RegisterDuplicateEmail()
        {
            _service.Register(MakeRegisterDto());
            RegisterDto dup = MakeRegisterDto();
            dup.Email = "TEST@student.uts.edu.au";

            UserDto result = _service.Register(dup);

            Assert.Null(result);
            Assert.AreEqual(1, _database.Users.Count(), "Only one user must be saved");
        }

        //test to make sure the correct password returns the user
        [Test]
        public void LoginCorrectPassword()
        {
            _service.Register(MakeRegisterDto());
            LoginDto login = new LoginDto();
            login.Email = "test@student.uts.edu.au";
            login.Password = "Password412";

            UserDto user = _service.Login(login);
            Assert.NotNull(user);
            Assert.AreEqual("test user", user.Name);


        }

        [Test]
        public void LoginWrongPassword()
        {
            _service.Register(MakeRegisterDto());

            LoginDto login = new LoginDto();
            login.Email = "test@student.uts.edu.au";
            login.Password = "Wrongpassword";

            UserDto user = _service.Login(login);
            Assert.Null(user);
        }

        [Test]
        public void LoginUnknownEmail()
        {
            LoginDto login = new LoginDto();
            login.Email = "missing@student.uts.edu.au";
            login.Password = "Password412";
            UserDto user = _service.Login(login); 
            Assert.Null(user);
        }

        [Test]
        public void GetByIDMissingUser()
        {
            UserDto user = _service.GetById(9999);
            Assert.Null(user);
        }
        //test to see if get all returns 2 users
        [Test]
        public void GetAllReturnsUsers()
        {
            _service.Register(MakeRegisterDto());

            RegisterDto second = MakeRegisterDto();
            second.Email = "second@student.uts.edu.au";
            _service.Register(second);

            List<UserDto> users = _service.GetAll();
            Assert.AreEqual(2, users.Count);
        }


    }
}
