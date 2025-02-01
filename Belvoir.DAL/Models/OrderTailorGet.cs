using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class OrderTailorGet
    {
        public Guid order_id { get; set; }
        public string customerName { get; set; }
        public DateTime orderDate { get; set; }
        public Guid tailorProductId { get; set; }
        public string status { get; set; }
        public DateTime deadline { get; set; }
    }
}
