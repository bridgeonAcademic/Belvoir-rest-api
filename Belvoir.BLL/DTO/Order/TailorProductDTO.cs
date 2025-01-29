using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Order
{
    public class TailorProductDTO
    {
        public Guid DesignId { get; set; }
        public Guid ClothId { get; set; }
        public string product_name { get; set; }
        public decimal price { get; set; }
    }
}
