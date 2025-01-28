using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Design
{
    public class AddDesignDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public Guid CreatedBy { get; set; }
        public List<IFormFile> ImageFiles { get; set; }
    }
}
