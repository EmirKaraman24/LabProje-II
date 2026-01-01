using System.Collections.Generic;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Graf renklendirme algoritmaları için arayüz.
    /// </summary>
    public interface IColoringAlgorithm : IAlgorithm
    {
        Dictionary<string, int> ColorGraph(Graph graph);
        Dictionary<int, List<Node>> GetColorGroups(Graph graph);
    }
}

