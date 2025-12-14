namespace SocialNetworkAnalysis.Core
{
    public class Edge
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public double Weight { get; set; }

        public Edge(string sourceId, string targetId, double weight = 0)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Weight = weight;
        }
    }
}
