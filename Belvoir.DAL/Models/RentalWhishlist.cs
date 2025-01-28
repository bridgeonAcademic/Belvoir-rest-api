using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class RentalWhishlist
    {
        public int WhishlistId { get; set; }
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public decimal OfferPrice { get; set; }

        public decimal Price { get; set; }

        public Guid FabricType { get; set; }

        public string Gender { get; set; }

        public string GarmentType { get; set; }

    }
}
