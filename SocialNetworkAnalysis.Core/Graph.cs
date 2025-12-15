using System.Collections.Generic;

namespace SocialNetworkAnalysis.Core
{
    public class Graph
    {
        public Dictionary<string, Node> Nodes { get; private set; } = new Dictionary<string, Node>();
        public List<Edge> Edges { get; private set; } = new List<Edge>();

        public void AddNode(Node node)
        {
            if (!Nodes.ContainsKey(node.Id))
            {
                Nodes[node.Id] = node;
            }
        }
    }
}
