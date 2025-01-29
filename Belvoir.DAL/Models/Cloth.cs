namespace Belvoir.DAL.Models
{
    public class Cloth
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; }
        public string Color { get; set; }
        public string DesignType { get; set; }
        public Decimal Price { get; set; }
        public string ImageUrl {  get; set; }

    }
}
