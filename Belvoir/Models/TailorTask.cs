namespace Belvoir.Models
{
    public class TailorTask
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; }

        public DateTime Orderdate { get; set; }

        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime createdat { get; set; }

        public DateTime updatedat { get; set; }

        public Guid assaigned {  get; set; }
        public bool isdeleted { get; set; }
    }
}
