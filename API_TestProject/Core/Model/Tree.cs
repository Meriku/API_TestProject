using System.ComponentModel.DataAnnotations;
using API_TestProject.DataBase.Model;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace API_TestProject.Core.Model
{
    public class TreeExtended: Tree
    {
        public TreeExtended(Tree tree, List<Node> nodes) : base(tree) { FillNodesChildren(nodes); }

        public ICollection<Node> RootNodes { get { return Nodes.Where(n => n.ParentNodeId == null).ToList(); } }

        public Dictionary<int, Node> AllNodesMap;

        private void FillNodesChildren(List<Node> nodes)
        {
            AllNodesMap = nodes.ToDictionary(x => x.NodeId, x => x);
            foreach (var node in nodes)
            {
                AddNodeToParent(node);
            }
        }

        private void AddNodeToParent(Node node)
        {
            if (node.ParentNodeId == null)
                return;

            var parentNode = AllNodesMap[(int)node.ParentNodeId];

            parentNode.ChildrenNodes ??= new List<Node>();
            parentNode.ChildrenNodes.Add(node);
        }

    }

    public class NodeExtended : Node
    {

    }
}
