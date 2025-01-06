using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Belvoir.Models
{


    public class User
    {
        public string Email { get; set; }                 // 'laundry1@example.com'
        public Guid Id { get; set; }                  // 'abc12345-6789-4def-0123-456789abcdef'
        public bool IsActive { get; set; }                // FALSE
        public string Role { get; set; }                  // 'Laundry Manager'
        public string PasswordHash { get; set; }          // 'hash5'
        public string Phone { get; set; }           // '6677889900'
        public string Name { get; set; }            // 'Laundry'
    }

}
