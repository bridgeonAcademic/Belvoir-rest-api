using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Belvoir.Models
{


    public class User
    {

        public Guid Id { get; set;}
        public string Name { get; set; }
        public string PasswordHash { get; set;}
        public string Email { get; set;}
        public string Phone { get; set;}
        public string Role { get; set;}
        public bool IsBlocked { get; set;}
    }

}
