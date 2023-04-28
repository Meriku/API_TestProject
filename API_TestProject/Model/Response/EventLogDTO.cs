namespace API_TestProject.Model.Response
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
        public int eventId { get; set; }
        public DateTime createdAt { get; set; }
    }
}
