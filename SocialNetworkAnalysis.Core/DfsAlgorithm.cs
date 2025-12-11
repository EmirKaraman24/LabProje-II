using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    public class DfsAlgorithm
    {
        public List<Node> Execute(Graph graph, string startNodeId)
        {
            var visited = new List<Node>();
            if (!graph.Nodes.ContainsKey(startNodeId)) return visited;

            var stack = new Stack<string>();
            var visitedIds = new HashSet<string>();

            stack.Push(startNodeId);

            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                
                if (!visitedIds.Contains(currentId))
                {
                    visitedIds.Add(currentId);
                    if (graph.Nodes.TryGetValue(currentId, out var node))
                    {
                        visited.Add(node);
                    }

                    // For Stack to process neighbors in specific order (e.g. alphabetical like BFS),
                    // we should push them in reverse order.
                    var neighbors = graph.Edges
                        .Where(e => e.SourceId == currentId)
                        .Select(e => e.TargetId)
                        .Concat(graph.Edges.Where(e => e.TargetId == currentId).Select(e => e.SourceId))
                        .Distinct()
                        .OrderByDescending(id => id); // Reverse for Stack

                    foreach (var neighborId in neighbors)
                    {
                        if (!visitedIds.Contains(neighborId))
                        {
                            stack.Push(neighborId);
                        }
                    }
                }
            }

            return visited;
        }
    }
}
