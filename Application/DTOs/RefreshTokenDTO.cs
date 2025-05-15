using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RefreshTokenDTO
    {
        public Guid Id { get; set; }
        public string? Token { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }
}
