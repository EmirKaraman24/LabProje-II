using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Ağdaki düğümlerin önem derecelerini (merkezilik) hesaplar.
    /// </summary>
    public class CentralityAlgorithm
    {
        /// <summary>
        /// Derece Merkeziliği (Degree Centrality) hesaplar.
        /// (Bir düğümün kaç bağlantısı olduğu).
        /// </summary>
        /// <param name="graph">Analiz edilecek graf</param>
        /// <param name="topN">Döndürülecek en önemli N düğüm sayısı</param>
        /// <returns>Düğüm ve skor listesi</returns>
        public List<KeyValuePair<Node, double>> CalculateDegreeCentrality(Graph graph, int topN = 5)
        {
            var scores = new Dictionary<Node, double>();

            foreach (var node in graph.Nodes.Values)
            {
                // Derece = Komşu sayısı
                scores[node] = node.Neighbors.Count;
            }

            // Puanına göre azalan sırada sırala ve ilk N tanesini al
            var sorted = scores.OrderByDescending(x => x.Value).Take(topN).ToList();
            return sorted;
        }
    }
}
