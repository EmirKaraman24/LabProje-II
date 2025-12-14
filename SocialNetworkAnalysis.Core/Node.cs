namespace SocialNetworkAnalysis.Core
{
    public class Node
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Activity { get; set; }
        public double Interaction { get; set; }
        public double ConnectionCount { get; set; }
        public List<string> Neighbors { get; set; } = new List<string>();

        public Node()
        {
        }

        public Node(string id, string name, double activity, double interaction, double connectionCount)
        {
            Id = id;
            Name = name;
            Activity = activity;
            Interaction = interaction;
            ConnectionCount = connectionCount;
        }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
