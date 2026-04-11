using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Requests
{
    public class LeaveServerRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string ServerId { get; set; } = string.Empty;
    }
}
