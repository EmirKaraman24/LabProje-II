using System.Collections.Generic;
using System.Linq;

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
            // Self-loop check
            if (sourceId == targetId) return;

            // Check if nodes exist
            if (Nodes.ContainsKey(sourceId) && Nodes.ContainsKey(targetId))
            {
                // Check if edge already exists (undirected, so check both directions or assume canonical)
                // For this project, let's store effective edges.
                // Assuming undirected, we might want to ensure we don't add duplicate
                
                var exists = Edges.Any(e => 
                    (e.SourceId == sourceId && e.TargetId == targetId) || 
                    (e.SourceId == targetId && e.TargetId == sourceId));

                if (!exists)
                {
                    Edges.Add(new Edge(sourceId, targetId, weight));
                    
                    // Update neighbor lists in nodes
                    Nodes[sourceId].Neighbors.Add(targetId);
                    Nodes[targetId].Neighbors.Add(sourceId);
                }
            }
        }
        
        public void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
        }
    }
}
