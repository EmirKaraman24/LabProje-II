using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Welsh-Powell graf renklendirme algoritması.
    /// Komşu düğümlerin farklı renklerde boyanmasını sağlar.
    /// </summary>
    public class WelshPowellColoringAlgorithm : IColoringAlgorithm
    {
        public string Name => "Welsh-Powell Coloring";

        /// <summary>
        /// Grafiği renklendirir ve her düğüme bir renk numarası atar.
        /// </summary>
        /// <returns>Düğüm ID'si -> Renk numarası (0'dan başlar)</returns>
        public Dictionary<string, int> ColorGraph(Graph graph)
        {
            var coloring = new Dictionary<string, int>();

            // 1. Düğümleri derecelerine göre azalan sırada sırala
            var sortedNodes = graph.Nodes.Values
                .OrderByDescending(n => n.Neighbors.Count)
                .ToList();

            // 2. Her düğüm için renklendirme
            foreach (var node in sortedNodes)
            {
                // Bu düğümün komşularının renklerini bul
                var neighborColors = new HashSet<int>();
                foreach (var neighborId in node.Neighbors)
                {
                    if (coloring.ContainsKey(neighborId))
                    {
                        neighborColors.Add(coloring[neighborId]);
                    }
                }

                // En küçük uygun rengi bul (0'dan başla)
                int color = 0;
                while (neighborColors.Contains(color))
                {
                    color++;
                }

                coloring[node.Id] = color;
            }

            return coloring;
        }

        /// <summary>
        /// Renklendirme sonucunu renk gruplarına göre döndürür.
        /// </summary>
        /// <returns>Renk numarası -> O renkteki düğümlerin listesi</returns>
        public Dictionary<int, List<Node>> GetColorGroups(Graph graph)
        {
            var coloring = ColorGraph(graph);
            var colorGroups = new Dictionary<int, List<Node>>();

            foreach (var kvp in coloring)
            {
                var color = kvp.Value;
                var nodeId = kvp.Key;

                if (!colorGroups.ContainsKey(color))
                {
                    colorGroups[color] = new List<Node>();
                }

                if (graph.Nodes.TryGetValue(nodeId, out var node))
                {
                    colorGroups[color].Add(node);
                }
            }

            return colorGroups;
        }

        /// <summary>
        /// Her bağlı bileşen için ayrı ayrı renklendirme yapar.
        /// </summary>
        public Dictionary<string, int> ColorGraphByComponents(Graph graph)
        {
            var coloring = new Dictionary<string, int>();
            var componentsAlgo = new ConnectedComponentsAlgorithm();
            var components = componentsAlgo.FindComponents(graph);

            int colorOffset = 0;

            foreach (var component in components)
            {
                // Her bileşen için alt graf oluştur
                var subGraph = CreateSubGraph(graph, component.Select(n => n.Id).ToList());
                
                // Alt grafi renklendir
                var componentColoring = ColorGraph(subGraph);

                // Renk numaralarını offset ile ayarla
                foreach (var kvp in componentColoring)
                {
                    coloring[kvp.Key] = kvp.Value + colorOffset;
                }

                // Sonraki bileşen için renk offset'ini güncelle
                if (componentColoring.Count > 0)
                {
                    colorOffset += componentColoring.Values.Max() + 1;
                }
            }

            return coloring;
        }

        /// <summary>
        /// Belirli düğümlerden oluşan alt graf oluşturur.
        /// </summary>
        private Graph CreateSubGraph(Graph originalGraph, List<string> nodeIds)
        {
            var subGraph = new Graph();

            foreach (var nodeId in nodeIds)
            {
                if (originalGraph.Nodes.TryGetValue(nodeId, out var node))
                {
                    subGraph.AddNode(node);
                }
            }

            // Alt grafa kenarları ekle
            foreach (var edge in originalGraph.Edges)
            {
                if (nodeIds.Contains(edge.SourceId) && nodeIds.Contains(edge.TargetId))
                {
                    subGraph.AddEdge(edge.SourceId, edge.TargetId, edge.Weight);
                }
            }

            return subGraph;
        }
    }
}

