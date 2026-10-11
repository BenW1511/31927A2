using System.ComponentModel.DataAnnotations;

namespace CampusHaps.Api.DTOs{
    public class VoteDto{
        //Rejects 0 and negative numbers
        [Range(1, int.MaxValue, ErrorMessage = "A valid user is required.")]
        public int UserId { get; set; }

    }
}
