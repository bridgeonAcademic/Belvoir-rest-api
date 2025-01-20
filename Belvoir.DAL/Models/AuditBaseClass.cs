namespace Belvoir.DAL.Models
{
    public class AuditBaseClass
    {
        public bool IsDeleted { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ?UpdatedBy { get; set; }
        public DateTime ?CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
