using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        public int? Age { get; set; }
        public double? Salary { get; set; }
        public string? Title { get; set; }
        public double? Exp { get; set; }
        public string? Department { get; set; }
    }
}
