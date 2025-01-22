using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class Design
    {
        public string DesignId { get; set; }
        public string DesignName { get; set; }
        public string DesignDescription { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public List<string> ImageUrls { get; set; } 
        public DateTime DesignCreatedAt { get; set; }
    }
}
