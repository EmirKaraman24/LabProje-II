using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    public class ShortestPathAlgorithm
    {
        public List<Node> Dijkstra(Graph graph, string startId, string endId)
        {
            var distances = new Dictionary<string, double>();
            var previous = new Dictionary<string, string>();
            var nodes = new List<string>();

            foreach (var node in graph.Nodes.Values)
            {
                if (node.Id == startId) distances[node.Id] = 0;
                else distances[node.Id] = double.MaxValue;

                nodes.Add(node.Id);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x].CompareTo(distances[y]));
                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == endId)
                {
                    var path = new List<Node>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(graph.Nodes[smallest]);
                        smallest = previous[smallest];
                    }
                    path.Add(graph.Nodes[startId]);
                    path.Reverse();
                    return path;
                }

                if (distances[smallest] == double.MaxValue) break;

                if (graph.Nodes.TryGetValue(smallest, out var u))
                {
                    foreach (var edge in graph.Edges.Where(e => e.SourceId == smallest || e.TargetId == smallest))
                    {
                        var neighborId = edge.SourceId == smallest ? edge.TargetId : edge.SourceId;
                        var alt = distances[smallest] + edge.Weight;
                        if (alt < distances[neighborId])
                        {
                            distances[neighborId] = alt;
                            previous[neighborId] = smallest;
                        }
                    }
                }
            }

            return new List<Node>();
        }
    }
}
