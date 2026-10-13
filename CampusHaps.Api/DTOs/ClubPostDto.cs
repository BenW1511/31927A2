namespace CampusHaps.Api.DTOs
{
    public class ClubPostDto //defines the information the API sends to the frontend when displaying a club post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int ClubId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = "";
    }
}
