using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Rental
{
    public class AddToCartDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
