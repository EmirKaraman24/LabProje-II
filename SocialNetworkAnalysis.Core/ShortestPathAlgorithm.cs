using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// En kısa yol algoritmalarını (Dijkstra, A*) barındırır.
    /// </summary>
    public class ShortestPathAlgorithm
    {
        /// <summary>
        /// Dijkstra algoritması ile iki düğüm arasındaki en kısa yolu bulur.
        /// </summary>
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

        /// <summary>
        /// A* algoritması ile iki düğüm arasındaki en kısa yolu (sezgisel yöntemle) bulur.
        /// </summary>
        public List<Node> AStar(Graph graph, string startId, string endId, Func<Node, Node, double> heuristic)
        {
            var distances = new Dictionary<string, double>(); // g(n)
            var fScores = new Dictionary<string, double>(); // f(n) = g(n) + h(n)
            var previous = new Dictionary<string, string>();
            var openSet = new List<string>();

            foreach (var node in graph.Nodes.Values)
            {
                distances[node.Id] = double.MaxValue;
                fScores[node.Id] = double.MaxValue;
            }

            distances[startId] = 0;
            // h(start, end)
            if (graph.Nodes.ContainsKey(startId) && graph.Nodes.ContainsKey(endId))
            {
                fScores[startId] = heuristic(graph.Nodes[startId], graph.Nodes[endId]);
            }
            
            openSet.Add(startId);

            while (openSet.Count > 0)
            {
                openSet.Sort((x, y) => fScores[x].CompareTo(fScores[y]));
                var current = openSet[0];

                if (current == endId)
                {
                    var path = new List<Node>();
                    while (previous.ContainsKey(current))
                    {
                        path.Add(graph.Nodes[current]);
                        current = previous[current];
                    }
                    path.Add(graph.Nodes[startId]);
                    path.Reverse();
                    return path;
                }

                openSet.Remove(current);

                if (graph.Nodes.TryGetValue(current, out var u))
                {
                    foreach (var edge in graph.Edges.Where(e => e.SourceId == current || e.TargetId == current))
                    {
                        var neighborId = edge.SourceId == current ? edge.TargetId : edge.SourceId;
                        
                        // Tentative gScore
                        var tentativeGScore = distances[current] + edge.Weight;

                        if (tentativeGScore < distances[neighborId])
                        {
                            previous[neighborId] = current;
                            distances[neighborId] = tentativeGScore;
                            fScores[neighborId] = distances[neighborId] + heuristic(graph.Nodes[neighborId], graph.Nodes[endId]);

                            if (!openSet.Contains(neighborId))
                            {
                                openSet.Add(neighborId);
                            }
                        }
                    }
                }
            }

            return new List<Node>();
        }
    }
}
