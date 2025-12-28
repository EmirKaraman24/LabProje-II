namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Sosyal ağdaki bir kişiyi temsil eder.
    /// </summary>
    public class Node
    {
        public string Id { get; set; } = string.Empty; // Benzersiz kimlik
        public string Name { get; set; } = string.Empty; // Görünen isim
        public double Activity { get; set; } // Etkinlik puanı
        public double Interaction { get; set; } // Etkileşim puanı
        public double ConnectionCount { get; set; } // Bağlantı sayısı
        public List<string> Neighbors { get; set; } = new List<string>(); // Komşu ID listesi

        public Node() { }

        public Node(string id, string name, double activity, double interaction, double connectionCount)
        {
            Id = id;
            Name = name;
            Activity = activity;
            Interaction = interaction;
            ConnectionCount = connectionCount;
        }

        public override string ToString() => $"{Name} ({Id})";
    }
}
