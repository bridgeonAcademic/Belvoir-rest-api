using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Rental
{
    public class RentalCartItem
    {
        public Guid ItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public string PrimaryImageUrl { get; set; }
    }
}
