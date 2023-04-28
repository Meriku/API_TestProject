using System.ComponentModel.DataAnnotations;

namespace API_TestProject.DataBase.Model
{
    public class Tree
    {
        [Key]
        public int TreeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Node> Nodes { get; set; }

        public Tree() { }

        public Tree(Tree tree)
        {
            TreeId = tree.TreeId;
            Name = tree.Name;
            Nodes = tree.Nodes;
        }
    }

    public class Node
    {
        [Key]
        public int NodeId { get; set; }
        public string Name { get; set; }
        public int TreeId { get; set; }
        public int? ParentNodeId { get; set; }

        public virtual Tree Tree { get; set; }
        public virtual Node ParentNode { get; set; }
        public virtual ICollection<Node> ChildrenNodes { get; set; }
    }
}
