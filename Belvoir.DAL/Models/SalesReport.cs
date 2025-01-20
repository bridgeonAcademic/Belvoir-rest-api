using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class SalesReport
    {
        public DateTime Date { get; set; }
        public decimal Tailoring { get; set; }
        public decimal Laundry { get; set; }
        public decimal Rentals { get; set; }
        public decimal TotalSales { get; set; }

        public decimal TargetSales { get; set; }
        public decimal TotalRevenue { get; set; }

    }
}
