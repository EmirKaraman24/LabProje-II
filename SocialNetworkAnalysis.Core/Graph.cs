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

        public void AddEdge(string sourceId, string targetId, double weight)
        {
            if (sourceId == targetId) return;

            if (Nodes.ContainsKey(sourceId) && Nodes.ContainsKey(targetId))
            {
                bool exists = false;
                foreach(var e in Edges)
                {
                    if ((e.SourceId == sourceId && e.TargetId == targetId) ||
                        (e.SourceId == targetId && e.TargetId == sourceId))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    Edges.Add(new Edge(sourceId, targetId, weight));
                    Nodes[sourceId].Neighbors.Add(targetId);
                    Nodes[targetId].Neighbors.Add(sourceId);
                }
            }
        }
    }
}
