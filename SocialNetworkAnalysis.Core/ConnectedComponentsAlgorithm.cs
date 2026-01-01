using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Bağlı bileşenleri (connected components) tespit eden algoritma.
    /// </summary>
    public class ConnectedComponentsAlgorithm : IGraphAnalysisAlgorithm
    {
        public string Name => "Connected Components";

        /// <summary>
        /// Grafiğin tüm bağlı bileşenlerini bulur.
        /// </summary>
        /// <returns>Her bileşen bir liste olarak döndürülür.</returns>
        public List<List<Node>> FindComponents(Graph graph)
        {
            var components = new List<List<Node>>();
            var visited = new HashSet<string>();

            foreach (var node in graph.Nodes.Values)
            {
                if (!visited.Contains(node.Id))
                {
                    var component = new List<Node>();
                    DFSComponent(graph, node.Id, visited, component);
                    components.Add(component);
                }
            }

            return components;
        }

        /// <summary>
        /// DFS kullanarak bir bileşendeki tüm düğümleri bulur.
        /// </summary>
        private void DFSComponent(Graph graph, string nodeId, HashSet<string> visited, List<Node> component)
        {
            visited.Add(nodeId);
            if (graph.Nodes.TryGetValue(nodeId, out var node))
            {
                component.Add(node);

                foreach (var neighborId in node.Neighbors)
                {
                    if (!visited.Contains(neighborId))
                    {
                        DFSComponent(graph, neighborId, visited, component);
                    }
                }
            }
        }

        /// <summary>
        /// Bağlı bileşen sayısını döndürür.
        /// </summary>
        public int GetComponentCount(Graph graph)
        {
            return FindComponents(graph).Count;
        }
    }
}

