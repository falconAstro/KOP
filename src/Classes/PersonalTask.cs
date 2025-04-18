using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementApp.Classes
{
    public record PersonalTask
    {
        public string Task { get; set; }
        public string TaskId { get; set; }
    }
}
