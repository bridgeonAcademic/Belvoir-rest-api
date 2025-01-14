using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class ProductQuery
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } 
        public bool IsDescending { get; set; } = false; 
        public string? Category { get; set; } 
        public decimal? MinPrice { get; set; } 
        public decimal? MaxPrice { get; set; } 
    }
}
