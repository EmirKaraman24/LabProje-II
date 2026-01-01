using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// A* algoritması ile en kısa yolu bulur.
    /// </summary>
    public class AStarAlgorithm : IShortestPathAlgorithm
    {
        public string Name => "A* (A Star)";

        /// <summary>
        /// A* algoritması ile iki düğüm arasındaki en kısa yolu bulur.
        /// </summary>
        public List<Node> FindPath(Graph graph, string startId, string endId)
        {
            // Varsayılan heuristic: Özellik farkına dayalı
            Func<Node, Node, double> defaultHeuristic = (n1, n2) =>
            {
                double diffActivity = Math.Abs(n1.Activity - n2.Activity);
                double diffInteraction = Math.Abs(n1.Interaction - n2.Interaction);
                double diffConnection = Math.Abs(n1.ConnectionCount - n2.ConnectionCount);
                return Math.Sqrt(diffActivity * diffActivity + diffInteraction * diffInteraction + diffConnection * diffConnection);
            };

            return AStar(graph, startId, endId, defaultHeuristic);
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
            var closedSet = new HashSet<string>();

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
                openSet.Remove(current);

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

                closedSet.Add(current);

                if (graph.Nodes.TryGetValue(current, out var u))
                {
                    foreach (var edge in graph.Edges.Where(e => e.SourceId == current || e.TargetId == current))
                    {
                        var neighborId = edge.SourceId == current ? edge.TargetId : edge.SourceId;
                        
                        if (closedSet.Contains(neighborId))
                            continue;

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

