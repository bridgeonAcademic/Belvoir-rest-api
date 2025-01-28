using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }

    }
}
