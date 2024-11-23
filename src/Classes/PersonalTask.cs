using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementApp.Classes
{
    public class PersonalTask
    {
        public required string Task { get; set; }
        public string Id { get; set; }
    }
    public class SharedTask
    {
        public required string Task { get; set; }
        public required string Username {  get; set; }
    }
    public class SharedEvent
    {
        public required string Event { get; set; }
        public required string Date { get; set; }
    }
    public class ShoppingList
    {
        public required string ShoppingItems { get; set; }
        public required string Username { get; set; }
    }
}
