using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// JSON formatında veri yükleme ve kaydetme işlemlerini yapar.
    /// </summary>
    public class JsonFileService
    {
        /// <summary>
        /// JSON dosyasından grafiği yükler.
        /// </summary>
        public Graph LoadGraphFromJson(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            var graphData = JsonSerializer.Deserialize<GraphData>(jsonString);

            if (graphData == null)
                throw new InvalidOperationException("JSON dosyası geçersiz format.");

            var graph = new Graph();

            // Düğümleri ekle
            foreach (var nodeData in graphData.Nodes)
            {
                var node = new Node(
                    id: nodeData.Id,
                    name: nodeData.Name,
                    activity: nodeData.Activity,
                    interaction: nodeData.Interaction,
                    connectionCount: nodeData.ConnectionCount
                );
                graph.AddNode(node);
            }

            // Kenarları ekle ve ağırlıkları hesapla
            foreach (var edgeData in graphData.Edges)
            {
                if (graph.Nodes.ContainsKey(edgeData.SourceId) && graph.Nodes.ContainsKey(edgeData.TargetId))
                {
                    var sourceNode = graph.Nodes[edgeData.SourceId];
                    var targetNode = graph.Nodes[edgeData.TargetId];
                    double weight = edgeData.Weight > 0 
                        ? edgeData.Weight 
                        : WeightCalculator.CalculateWeight(sourceNode, targetNode);
                    
                    graph.AddEdge(edgeData.SourceId, edgeData.TargetId, weight);
                }
            }

            return graph;
        }

        /// <summary>
        /// Grafiği JSON formatında kaydeder.
        /// </summary>
        public void SaveGraphToJson(Graph graph, string filePath)
        {
            var graphData = new GraphData
            {
                Nodes = graph.Nodes.Values.Select(n => new NodeData
                {
                    Id = n.Id,
                    Name = n.Name,
                    Activity = n.Activity,
                    Interaction = n.Interaction,
                    ConnectionCount = n.ConnectionCount
                }).ToList(),
                Edges = graph.Edges.Select(e => new EdgeData
                {
                    SourceId = e.SourceId,
                    TargetId = e.TargetId,
                    Weight = e.Weight
                }).ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonString = JsonSerializer.Serialize(graphData, options);
            File.WriteAllText(filePath, jsonString);
        }

        // JSON serileştirme için yardımcı sınıflar
        private class GraphData
        {
            public List<NodeData> Nodes { get; set; } = new List<NodeData>();
            public List<EdgeData> Edges { get; set; } = new List<EdgeData>();
        }

        private class NodeData
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public double Activity { get; set; }
            public double Interaction { get; set; }
            public double ConnectionCount { get; set; }
        }

        private class EdgeData
        {
            public string SourceId { get; set; } = string.Empty;
            public string TargetId { get; set; } = string.Empty;
            public double Weight { get; set; }
        }
    }
}

