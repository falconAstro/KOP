using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementApp.Classes
{
    public record ShoppingList
    {
        public string Username { get; set; }
        public List<string> ShoppingItems { get; set; }
        public string ListId { get; set; }
        public string Date { get; set; }
    }
}
