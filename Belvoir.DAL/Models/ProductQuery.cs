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
        public string? Material { get; set; } 
        public string? DesignPattern { get; set; } 
        public decimal? MinPrice { get; set; } 
        public decimal? MaxPrice { get; set; } 
        public int? PageNo { get; set; } 
        public int? PageSize { get; set; } 
    }
}
