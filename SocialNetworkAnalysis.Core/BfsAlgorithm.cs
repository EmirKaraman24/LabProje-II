using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    public class BfsAlgorithm
    {
        public List<Node> Execute(Graph graph, string startNodeId)
        {
            var visited = new List<Node>();
            if (!graph.Nodes.ContainsKey(startNodeId)) return visited;

            var queue = new Queue<string>();
            var visitedIds = new HashSet<string>();

            queue.Enqueue(startNodeId);
            visitedIds.Add(startNodeId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (graph.Nodes.TryGetValue(currentId, out var node))
                {
                    visited.Add(node);

                    // Sort neighbors to have deterministic order (optional, by Id)
                   var neighbors = graph.Edges
                        .Where(e => e.SourceId == currentId)
                        .Select(e => e.TargetId)
                        .Concat(graph.Edges.Where(e => e.TargetId == currentId).Select(e => e.SourceId))
                        .Distinct()
                        .OrderBy(id => id); // Alphabetical order for consistency

                    foreach (var neighborId in neighbors)
                    {
                        if (!visitedIds.Contains(neighborId))
                        {
                            visitedIds.Add(neighborId);
                            queue.Enqueue(neighborId);
                        }
                    }
                }
            }

            return visited;
        }
    }
}
