using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.DTO.Delivery
{
    public class DeliveryDTO
    {
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LicenceNo { get; set; }
        public string VehicleNo { get; set; }
    }
}
