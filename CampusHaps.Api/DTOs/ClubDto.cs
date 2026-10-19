namespace CampusHaps.Api.DTOs
{
    //holds the club information we want to send to the frontend
    //used to control what club information gets sent
    public class ClubDto //defines the information the API sends to the frontend when displaying a club
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public int EventCount { get; set; }
        public int PostCount { get; set; }
    }
}
