namespace API_TestProject.Model.Response
{
    public class TreeDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public ICollection<NodeDTO> children { get; set; }
    }
    public class NodeDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public virtual ICollection<NodeDTO> children { get; set; }
    }
}
