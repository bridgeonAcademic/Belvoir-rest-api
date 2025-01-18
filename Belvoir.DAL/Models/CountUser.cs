using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class CountUser
    {
        public int usercount { get; set; }
        public int activeusercount { get; set; }
        public int blockedusercount { get; set; }
    }
}
