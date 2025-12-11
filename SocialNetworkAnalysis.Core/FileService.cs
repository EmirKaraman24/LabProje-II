using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    public class FileService
    {
        public Graph LoadGraphFromCsv(string filePath)
        {
            var graph = new Graph();
            var lines = File.ReadAllLines(filePath);

            // Skip header if exists
            // Assuming header: DugumId,Activity,Interaction,ConnectionCount,Neighbors
            var startIdx = 0;
            if (lines.Length > 0 && lines[0].StartsWith("DugumId"))
            {
                startIdx = 1;
            }

            // First pass: Create Nodes
            for (int i = startIdx; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                var id = parts[0].Trim();
                var activity = double.Parse(parts[1].Trim());
                var interaction = double.Parse(parts[2].Trim());
                var connectionCount = double.Parse(parts[3].Trim());
                
                // Name defaults to ID if not separate, user requirement didn't specify Name column but Node has Name.
                // Let's use ID as Name for now.
                var node = new Node(id, id, activity, interaction, connectionCount);
                
                // Add neighbors temporarily to process later or store in Node property
                if (parts.Length > 4)
                {
                    var neighborsStr = parts[4].Trim();
                    if (!string.IsNullOrEmpty(neighborsStr))
                    {
                        var neighborIds = neighborsStr.Split(';');
                        node.Neighbors.AddRange(neighborIds.Select(n => n.Trim()));
                    }
                }

                graph.AddNode(node);
            }

            // Second pass: Create Edges based on Neighbors list
            // We need to calculate weights dynamically
            foreach (var node in graph.Nodes.Values)
            {
                foreach (var neighborId in node.Neighbors)
                {
                    if (graph.Nodes.ContainsKey(neighborId))
                    {
                        var neighbor = graph.Nodes[neighborId];
                        var weight = WeightCalculator.CalculateWeight(node, neighbor);
                        
                        // Graph.AddEdge handles duplication checks
                        graph.AddEdge(node.Id, neighborId, weight);
                    }
                }
            }

            return graph;
        }

        public void SaveGraphToCsv(Graph graph, string filePath)
        {
            var lines = new List<string>();
            lines.Add("DugumId,Activity,Interaction,ConnectionCount,Neighbors");

            foreach (var node in graph.Nodes.Values)
            {
                // Neighbors are stored in the Node object, or we can look at edges.
                // The Node object has a Neighbors list which we populated.
                // Ideally, we should sync this if edges change dynamically in the app.
                // But for the requirements "Nodes stored with Neighbors", we update Neighbors list from Edges before save?
                // Or just assume Node.Neighbors is kept up to date.
                // Let's reconstruct neighbors from Edges to be safe if the graph changed.
                
                // Find all edges connected to this node
                var connectedIds = graph.Edges
                    .Where(e => e.SourceId == node.Id)
                    .Select(e => e.TargetId)
                    .Concat(graph.Edges.Where(e => e.TargetId == node.Id).Select(e => e.SourceId))
                    .Distinct()
                    .ToList();
                
                var neighborsStr = string.Join(";", connectedIds);

                var line = $"{node.Id},{node.Activity},{node.Interaction},{node.ConnectionCountProperty},{neighborsStr}";
                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
