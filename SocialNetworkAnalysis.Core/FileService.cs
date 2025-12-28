using System;
using System.IO;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    public class FileService
    {
        /// <summary>
        /// CSV dosyasından grafiği yükler ve kenarları oluşturur.
        /// </summary>
        public Graph LoadGraphFromCsv(string filePath)
        {
            var graph = new Graph();
            var lines = File.ReadAllLines(filePath);

            // 1. Düğümleri oluştur
            foreach (var line in lines.Skip(1)) // Başlık satırını atla
            {
                var parts = line.Split(',');
                if (parts.Length >= 5)
                {
                    var node = new Node(
                        id: parts[0].Trim(),
                        name: parts[1].Trim(),
                        activity: double.Parse(parts[2].Trim()),
                        interaction: double.Parse(parts[3].Trim()),
                        connectionCount: double.Parse(parts[4].Trim())
                    );
                    
                    // Komşu ID'lerini geçici olarak sakla
                    if (parts.Length > 5)
                    {
                         var neighborIds = parts[5].Split(';').Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n));
                         node.Neighbors.AddRange(neighborIds);
                    }

                    graph.AddNode(node);
                }
            }

            // 2. Kenarları ve Ağırlıkları hesapla
            foreach (var node in graph.Nodes.Values)
            {
                foreach (var neighborId in node.Neighbors)
                {
                    if (graph.Nodes.ContainsKey(neighborId))
                    {
                        var neighbor = graph.Nodes[neighborId];
                        double weight = WeightCalculator.CalculateWeight(node, neighbor);
                        graph.AddEdge(node.Id, neighborId, weight);
                    }
                }
            }

            return graph;
        }

        /// <summary>
        /// Grafiği CSV formatında kaydeder.
        /// </summary>
        public void SaveGraphToCsv(Graph graph, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Id,Name,Activity,Interaction,ConnectionCount,Neighbors");
                foreach (var node in graph.Nodes.Values)
                {
                    string neighbors = string.Join(";", node.Neighbors);
                    writer.WriteLine($"{node.Id},{node.Name},{node.Activity},{node.Interaction},{node.ConnectionCount},{neighbors}");
                }
            }
        }
    }
}
