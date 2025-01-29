using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO
{
    public class ClothDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MaterialType { get; set; }
        public string Color { get; set; }
        public string DesignType { get; set; }
        public Decimal Price { get; set; }
    }
}
