namespace CampusHaps.Api.DTOs{
    //holds the user information that can be safely sent to the frontend

    public class UserDto{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }
}
