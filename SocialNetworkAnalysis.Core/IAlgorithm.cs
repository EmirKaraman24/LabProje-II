using System.Collections.Generic;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Tüm algoritmalar için temel arayüz.
    /// </summary>
    public interface IAlgorithm
    {
        string Name { get; }
    }

    /// <summary>
    /// Graf gezinti algoritmaları için arayüz (BFS, DFS).
    /// </summary>
    public interface IGraphTraversalAlgorithm : IAlgorithm
    {
        List<Node> Traverse(Graph graph, string startNodeId);
    }

    /// <summary>
    /// En kısa yol algoritmaları için arayüz (Dijkstra, A*).
    /// </summary>
    public interface IShortestPathAlgorithm : IAlgorithm
    {
        List<Node> FindPath(Graph graph, string startId, string endId);
    }

    /// <summary>
    /// Graf analiz algoritmaları için arayüz (Centrality, Connected Components).
    /// </summary>
    public interface IGraphAnalysisAlgorithm : IAlgorithm
    {
    }
}

