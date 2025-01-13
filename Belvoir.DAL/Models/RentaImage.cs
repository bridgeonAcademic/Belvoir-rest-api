namespace Belvoir.DAL.Models
{
    public class RentalImage
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public Guid ProductId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
