using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementApp.Classes
{
    public class RegisteredUser
    {
        public required string Username { get; set; }
        public required string UserID { get; set; }
        public required string Email { get; set; }
    }
    public class PersonalTask
    {
        public string Username { get; set; }
        public required string Task { get; set; }
        public string TaskID { get; set; }
    }
    public class SharedTask
    {
        public string Username { get; set; }
        public string Task { get; set; }
        public string TaskId { get; set; }
    }
    public class SharedEvent
    {
        public string Username { get; set; }
        public required string Event { get; set; }
        public required string Date { get; set; }
        public string EventId { get; set; }
    }
    public class ShoppingList
    {
        public string Username { get; set; }
        public string ShoppingItems { get; set; }
        public string ListId { get; set; }
    }
}
