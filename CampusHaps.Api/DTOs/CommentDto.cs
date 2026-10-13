namespace CampusHaps.Api.DTOs
{
    public class CommentDto //defines the information the API sends to the frontend when displaying a comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int EventId { get; set; }
    }
}
