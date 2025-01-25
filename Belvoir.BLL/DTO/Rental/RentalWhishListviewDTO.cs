using Belvoir.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Rental
{
    public class RentalWhishListviewDTO
    {
        public int WhishlistId { get; set; }
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal offerPrice { get; set; }
        public decimal price { get; set; }

        public Guid fabrictype { get; set; }
        public string gender { get; set; }
        public string garmenttype { get; set; }
        public List<RentalImage> images { get; set; }
    }
}
