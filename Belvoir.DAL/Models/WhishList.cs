using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class WhishList
    {
        public int WhishlistId { get; set; }
        public Guid Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
