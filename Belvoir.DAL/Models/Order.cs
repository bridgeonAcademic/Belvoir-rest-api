using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class Order
    {
        public Guid customerId { get; set; }
        public DateTime orderDate { get; set; }
        public decimal totalAmount { get; set; }
        public string paymentMethod { get; set; }
        public string shippingAddress { get; set; }
        public string shippingMethod { get; set; }
        public decimal shippingCost { get; set; }
        public string trackingNumber { get; set; }
        public Guid updatedBy { get; set; }
        public string productType { get; set; }
        public Guid? tailorProductId { get; set; }
        public Guid? rentalProductId { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }

    }
}
