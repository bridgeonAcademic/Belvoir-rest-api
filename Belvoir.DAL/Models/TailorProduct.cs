using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class TailorProduct
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DesignId { get; set; }
        public Guid ClothId { get; set; }
        public string product_name { get; set; }
        public decimal price { get; set; }
    }
}
