using System.Collections.Generic;

namespace SocialNetworkAnalysis.Core
{
    public class Node
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        // Ozellik_I (Aktiflik)
        public double Activity { get; set; }
        
        // Ozellik_II (Etkileşim)
        public double Interaction { get; set; }
        
        // Ozellik_III (Bağlantı Sayısı - can be calculated or loaded)
        public double ConnectionCountProperty { get; set; }

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
            ConnectionCountProperty = connectionCount;
        }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
