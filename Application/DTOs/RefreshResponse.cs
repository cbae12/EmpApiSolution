using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record RefreshResponse(bool Flag, string Message = null!, string RefreshToken = null!);
}
