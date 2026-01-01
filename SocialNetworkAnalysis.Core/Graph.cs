using System.Collections.Generic;
using System.Linq;

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
        public bool AddNode(Node node)
        {
            // Hatalı veri kontrolü
            if (string.IsNullOrWhiteSpace(node.Id))
                return false;
            
            if (Nodes.ContainsKey(node.Id))
                return false; // Aynı ID'li düğüm zaten var
            
            Nodes[node.Id] = node;
            return true;
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

        /// <summary>
        /// Graftan bir düğümü siler ve ona bağlı tüm kenarları kaldırır.
        /// </summary>
        public bool RemoveNode(string nodeId)
        {
            if (!Nodes.ContainsKey(nodeId))
                return false;

            // Düğüme bağlı tüm kenarları kaldır
            var edgesToRemove = Edges.Where(e => e.SourceId == nodeId || e.TargetId == nodeId).ToList();
            foreach (var edge in edgesToRemove)
            {
                RemoveEdge(edge.SourceId, edge.TargetId);
            }

            // Komşu listelerinden bu düğümü kaldır
            foreach (var neighborId in Nodes[nodeId].Neighbors.ToList())
            {
                if (Nodes.ContainsKey(neighborId))
                {
                    Nodes[neighborId].Neighbors.Remove(nodeId);
                }
            }

            // Düğümü kaldır
            Nodes.Remove(nodeId);
            return true;
        }

        /// <summary>
        /// İki düğüm arasındaki kenarı siler.
        /// </summary>
        public bool RemoveEdge(string sourceId, string targetId)
        {
            var edgeToRemove = Edges.FirstOrDefault(e => 
                (e.SourceId == sourceId && e.TargetId == targetId) ||
                (e.SourceId == targetId && e.TargetId == sourceId));

            if (edgeToRemove != null)
            {
                Edges.Remove(edgeToRemove);
                
                // Komşu listelerinden kaldır
                if (Nodes.ContainsKey(sourceId))
                    Nodes[sourceId].Neighbors.Remove(targetId);
                if (Nodes.ContainsKey(targetId))
                    Nodes[targetId].Neighbors.Remove(sourceId);
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Bir düğümün bilgilerini günceller.
        /// </summary>
        public bool UpdateNode(string nodeId, string? name = null, double? activity = null, 
            double? interaction = null, double? connectionCount = null)
        {
            if (!Nodes.ContainsKey(nodeId))
                return false;

            var node = Nodes[nodeId];
            if (name != null) node.Name = name;
            if (activity.HasValue) node.Activity = activity.Value;
            if (interaction.HasValue) node.Interaction = interaction.Value;
            if (connectionCount.HasValue) node.ConnectionCount = connectionCount.Value;

            // Ağırlıkları yeniden hesapla
            RecalculateWeights(nodeId);
            return true;
        }

        /// <summary>
        /// Bir düğümün kenar ağırlıklarını yeniden hesaplar.
        /// </summary>
        private void RecalculateWeights(string nodeId)
        {
            if (!Nodes.ContainsKey(nodeId))
                return;

            var node = Nodes[nodeId];
            foreach (var edge in Edges.Where(e => e.SourceId == nodeId || e.TargetId == nodeId))
            {
                var neighborId = edge.SourceId == nodeId ? edge.TargetId : edge.SourceId;
                if (Nodes.ContainsKey(neighborId))
                {
                    edge.Weight = WeightCalculator.CalculateWeight(node, Nodes[neighborId]);
                }
            }
        }

        /// <summary>
        /// Komşuluk matrisini döndürür.
        /// </summary>
        public Dictionary<string, Dictionary<string, double>> GetAdjacencyMatrix()
        {
            var matrix = new Dictionary<string, Dictionary<string, double>>();

            foreach (var nodeId in Nodes.Keys)
            {
                matrix[nodeId] = new Dictionary<string, double>();
                foreach (var otherId in Nodes.Keys)
                {
                    matrix[nodeId][otherId] = 0;
                }
            }

            foreach (var edge in Edges)
            {
                matrix[edge.SourceId][edge.TargetId] = edge.Weight;
                matrix[edge.TargetId][edge.SourceId] = edge.Weight; // Yönsüz graf
            }

            return matrix;
        }

        /// <summary>
        /// Komşuluk listesini döndürür.
        /// </summary>
        public Dictionary<string, List<string>> GetAdjacencyList()
        {
            var adjacencyList = new Dictionary<string, List<string>>();
            foreach (var node in Nodes.Values)
            {
                adjacencyList[node.Id] = new List<string>(node.Neighbors);
            }
            return adjacencyList;
        }
    }
}
