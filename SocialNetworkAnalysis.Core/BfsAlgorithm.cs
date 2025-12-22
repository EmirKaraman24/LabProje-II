using System.Collections.Generic;

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

                    foreach (var neighborId in node.Neighbors)
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
