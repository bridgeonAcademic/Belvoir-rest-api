namespace Belvoir.Models
{
    public class RentalProduct:AuditBaseClass
    {
        public Guid Id { get; set; }    
        public string Title { get; set; }
        public string Description { get; set; }

        public decimal OfferPrice { get; set; }

        public decimal Price { get; set; }

        public Guid FabricType { get; set; }

        public string Gender { get; set; }

        public string GarmentType { get; set; }

    }
}
