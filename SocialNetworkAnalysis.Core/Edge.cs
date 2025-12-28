namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// İki düğüm arasındaki bağlantıyı temsil eder.
    /// </summary>
    public class Edge
    {
        public string SourceId { get; set; } // Başlangıç düğümü
        public string TargetId { get; set; } // Bitiş düğümü
        public double Weight { get; set; } // Kenar ağırlığı

        public Edge(string sourceId, string targetId, double weight = 0)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Weight = weight;
        }
    }
}
