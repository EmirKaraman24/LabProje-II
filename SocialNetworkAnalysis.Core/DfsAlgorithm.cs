using System.Collections.Generic;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Derinlik Öncelikli Arama (DFS) algoritmasını uygular.
    /// </summary>
    public class DfsAlgorithm : IGraphTraversalAlgorithm
    {
        public string Name => "DFS (Depth-First Search)";

        /// <summary>
        /// Başlangıç düğümünden itibaren grafiği derinlemesine gezer.
        /// </summary>
        public List<Node> Traverse(Graph graph, string startNodeId)
        {
            return Execute(graph, startNodeId);
        }

        /// <summary>
        /// Başlangıç düğümünden itibaren grafiği derinlemesine gezer.
        /// </summary>
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

                        // Push neighbors in reverse order so they are processed in original order
                        for (int i = node.Neighbors.Count - 1; i >= 0; i--)
                        {
                            var neighborId = node.Neighbors[i];
                            if (!visitedIds.Contains(neighborId))
                            {
                                stack.Push(neighborId);
                            }
                        }
                    }
                }
            }

            return visited;
        }
    }
}
