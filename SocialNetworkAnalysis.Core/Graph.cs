using System.Collections.Generic;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Graf veri yapısını yönetir.
    /// </summary>
    public class Graph
    {
        public Dictionary<string, Node> Nodes { get; private set; } = new Dictionary<string, Node>(); // Düğüm listesi
        public List<Edge> Edges { get; private set; } = new List<Edge>(); // Kenar listesi

        /// <summary>
        /// Grafa yeni bir düğüm ekler.
        /// </summary>
        public void AddNode(Node node)
        {
            if (!Nodes.ContainsKey(node.Id))
            {
                Nodes[node.Id] = node;
            }
        }

        /// <summary>
        /// İki düğüm arasına kenar ekler.
        /// </summary>
        public void AddEdge(string sourceId, string targetId, double weight)
        {
            if (sourceId == targetId) return; // Kendine döngü engelle

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
