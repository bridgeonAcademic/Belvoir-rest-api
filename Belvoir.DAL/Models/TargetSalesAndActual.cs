using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class TargetSalesAndActual
    {
        public decimal TotalSales { get; set; }

        public decimal TargetSales { get; set; }

        public DateTime Date {  get; set; }

    }
}
