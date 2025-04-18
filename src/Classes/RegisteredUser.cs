using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementApp.Classes
{
    public record RegisteredUser
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
