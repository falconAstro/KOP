using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementApp.Classes
{
    public record SharedEvent
    {
        public string Event { get; set; }
        public DateTime Date { get; set; }
        public string EventId { get; set; }
    }
}
