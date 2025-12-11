using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Core
{
    public class ShortestPathAlgorithm
    {
        // Dijkstra's Algorithm
        public List<Node> Dijkstra(Graph graph, string startId, string endId)
        {
            // Dijkstra is just A* with heuristic = 0
            return AStar(graph, startId, endId, (n1, n2) => 0);
        }

        // A* Algorithm
        // Heuristic function: estimates cost from current node to goal
        public List<Node> AStar(Graph graph, string startId, string endId, Func<Node, Node, double> heuristic)
        {
            var path = new List<Node>();
            if (!graph.Nodes.ContainsKey(startId) || !graph.Nodes.ContainsKey(endId))
                return path;

            var startNode = graph.Nodes[startId];
            var endNode = graph.Nodes[endId];

            var openSet = new HashSet<string> { startId };
            var cameFrom = new Dictionary<string, string>();

            var gScore = new Dictionary<string, double>();
            foreach (var node in graph.Nodes.Keys) gScore[node] = double.MaxValue;
            gScore[startId] = 0;

            var fScore = new Dictionary<string, double>();
            foreach (var node in graph.Nodes.Keys) fScore[node] = double.MaxValue;
            fScore[startId] = heuristic(startNode, endNode);

            while (openSet.Count > 0)
            {
                // Get node with lowest fScore in openSet
                var currentId = openSet.OrderBy(id => fScore[id]).First();

                if (currentId == endId)
                {
                    return ReconstructPath(cameFrom, currentId, graph);
                }

                openSet.Remove(currentId);
                var currentNode = graph.Nodes[currentId];

                // Find neighbors
                var neighbors = GetNeighbors(graph, currentId);

                foreach (var neighborId in neighbors)
                {
                    var neighbor = graph.Nodes[neighborId];
                    var edgeWeigth = GetEdgeWeight(graph, currentId, neighborId);
                    
                    var tentativeGScore = gScore[currentId] + edgeWeigth;

                    if (tentativeGScore < gScore[neighborId])
                    {
                        cameFrom[neighborId] = currentId;
                        gScore[neighborId] = tentativeGScore;
                        fScore[neighborId] = gScore[neighborId] + heuristic(neighbor, endNode);

                        if (!openSet.Contains(neighborId))
                        {
                            openSet.Add(neighborId);
                        }
                    }
                }
            }

            // No path found
            return path;
        }

        private List<Node> ReconstructPath(Dictionary<string, string> cameFrom, string currentId, Graph graph)
        {
            var totalPath = new List<Node> { graph.Nodes[currentId] };
            while (cameFrom.ContainsKey(currentId))
            {
                currentId = cameFrom[currentId];
                totalPath.Insert(0, graph.Nodes[currentId]);
            }
            return totalPath;
        }

        private List<string> GetNeighbors(Graph graph, string nodeId)
        {
            // We can rely on graph.Edges or Node.Neighbors. 
            // Since we built graph ensuring Edges are populated, let's look at Edges for accurate weights
            return graph.Edges
                .Where(e => e.SourceId == nodeId)
                .Select(e => e.TargetId)
                .Concat(graph.Edges.Where(e => e.TargetId == nodeId).Select(e => e.SourceId))
                .Distinct()
                .ToList();
        }

        private double GetEdgeWeight(Graph graph, string sourceId, string targetId)
        {
            var edge = graph.Edges.FirstOrDefault(e => 
                (e.SourceId == sourceId && e.TargetId == targetId) || 
                (e.SourceId == targetId && e.TargetId == sourceId));
            
            return edge != null ? edge.Weight : double.MaxValue;
        }
    }
}
