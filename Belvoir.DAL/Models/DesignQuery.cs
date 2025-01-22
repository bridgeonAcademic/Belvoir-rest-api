using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class DesignQuery
    {
        public string Name { get; set; }          // Filter by Design Name
        public string Category { get; set; }      // Filter by Category
        public decimal? MinPrice { get; set; }    // Minimum price filter
        public decimal? MaxPrice { get; set; }    // Maximum price filter
        public bool? Available { get; set; }      // Filter by Availability
        public string SortBy { get; set; }        // Column to sort by (CreatedAt, Price, etc.)
        public bool? IsDescending { get; set; }   // Sort order (TRUE for descending, FALSE for ascending)
        public int? PageNo { get; set; }          // Page number for pagination
        public int? PageSize { get; set; }
    }
}
