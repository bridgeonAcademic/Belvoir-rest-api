namespace Belvoir.DAL.Models
{
    public class TailorTask
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime Deadline { get; set; }
        public Guid assaigned { get; set; }
        
    }
}
