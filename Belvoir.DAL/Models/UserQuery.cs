using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Models
{
    public class UserQuery
    {
        public string? SearchTerm { get; set; } 
       
        public bool? IsBlocked { get; set; } 
        public DateTime? MinCreatedDate { get; set; } 
        public DateTime? MaxCreatedDate { get; set; } 
        public string? SortBy { get; set; } 
        public bool IsDescending { get; set; } = false; 
        public int? PageSize { get; set; }
        public int? pageNo { get; set; }
    }
}
