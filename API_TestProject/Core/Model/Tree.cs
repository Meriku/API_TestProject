using System.ComponentModel.DataAnnotations;
using API_TestProject.DataBase.Model;

namespace API_TestProject.Core.Model
{
    public class TreeExtended: Tree
    {
        public TreeExtended(Tree tree) : base(tree) { }

        public Dictionary<int, Node> NodesMap => base.Nodes != null ? base.Nodes.ToDictionary(x => x.NodeId, x => x) : new Dictionary<int, Node>();
    }
    // 

    public class NodeExtended : Node
    {

    }
}
