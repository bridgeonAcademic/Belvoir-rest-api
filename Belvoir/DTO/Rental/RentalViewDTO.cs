using Belvoir.Models;

namespace Belvoir.DTO.Rental
{
    public class RentalViewDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal offerPrice { get; set; }
        public decimal price { get; set; }

        public Guid fabrictype { get; set; }
        public string gender { get; set; }
        public string garmenttype { get; set; }
        public List<RentalImage> images { get; set; }
    }
}
