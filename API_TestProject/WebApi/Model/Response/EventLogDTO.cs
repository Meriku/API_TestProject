namespace API_TestProject.WebApi.Model.Response
{
    public class EventLogListDTO
    {
        public int skip { get; set; }
        public int count { get; set; }
        public ICollection<EventLogItemDTO> items { get; set; }
    }

    public class EventLogItemDTO
    {
        public int id { get; set; }
        public string eventId { get; set; }
        public DateTime createdAt { get; set; }
    }
    public class EventLogExtendedItemDTO
    {
        public int id { get; set; }
        public string eventId { get; set; }
        public DateTime createdAt { get; set; }
        public string text { get; set; }
    }
}
