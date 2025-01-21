using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Rental
{
    public class RentalCart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<RentalCartItem> Items { get; set; } = new List<RentalCartItem>();
    }
}
