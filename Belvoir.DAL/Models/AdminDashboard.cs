using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class AdminDashboard
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int SoldProducts { get; set; }
        public int NewUsers { get; set; }


        public IEnumerable<TargetSalesAndActual> SalesReports { get; set; }
    }
}
