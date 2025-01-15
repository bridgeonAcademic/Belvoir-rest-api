namespace Belvoir.DAL.Models
{
    public class Rating
    {
        public int Id { get; set; }            // The primary key of the rating record
        public Guid TailorId { get; set; }     // The ID of the tailor being rated
        public int RatingValue { get; set; }   // The rating value given by the user (e.g., 1 to 5)
        public Guid UserId { get; set; }       // The ID of the user who is giving the rating
        public DateTime RatedAt { get; set; }  // The timestamp for when the rating was given

    }

}
