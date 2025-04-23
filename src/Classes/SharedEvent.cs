namespace TimeManagementApp.Classes
{
    public record SharedEvent
    {
        public string Event { get; set; }
        public DateTime Date { get; set; }
        public string EventId { get; set; }
    }
}
