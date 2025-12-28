using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using SocialNetworkAnalysis.Core;

namespace SocialNetworkAnalysis.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Graph? _graph;
    private FileService _fileService;
    private Dictionary<string, Point> _nodePositions = new Dictionary<string, Point>();

    public MainWindow()
    {
        InitializeComponent();
        _fileService = new FileService();
    }

    private void DrawGraph()
    {
        // Graph visualization logic to be added
    }

    private void BtnLoadCsv_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                _graph = _fileService.LoadGraphFromCsv(openFileDialog.FileName);
                MessageBox.Show($"Grafik yüklendi! {_graph.Nodes.Count} düğüm, {_graph.Edges.Count} kenar.");
                DrawGraph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

        private void BtnSaveGraph_Click(object sender, RoutedEventArgs e)
        {
             MessageBox.Show("Kaydetme Henüz Hazır Değil");
        }

        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Placeholder for node selection logic
        }

        private void BtnBfs_Click(object sender, RoutedEventArgs e)
        {
            if (_graph == null) return;
            string startId = TxtSourceId.Text;

            // Default to first node if simple test needed and no source selected
            if (string.IsNullOrEmpty(startId))
            {
                 if (_graph.Nodes.Count > 0)
                 {
                     startId = _graph.Nodes.Keys.First();
                 }
                 else
                 {
                     MessageBox.Show("Grafik boş!");
                     return;
                 }
            }

            var bfs = new BfsAlgorithm();
            var visited = bfs.Execute(_graph, startId);
            
            var resultStr = string.Join(" -> ", visited.Select(n => n.Id));
            TxtInfo.Text = $"BFS Sonucu ({startId} başlangıçlı):\n{resultStr}";
        }
        private void BtnDfs_Click(object sender, RoutedEventArgs e)
        {
            if (_graph == null) return;
            string startId = TxtSourceId.Text;

             if (string.IsNullOrEmpty(startId))
            {
                 if (_graph.Nodes.Count > 0)
                 {
                     startId = _graph.Nodes.Keys.First();
                 }
                 else
                 {
                     MessageBox.Show("Grafik boş!");
                     return;
                 }
            }

            var dfs = new DfsAlgorithm();
            var visited = dfs.Execute(_graph, startId);

            var resultStr = string.Join(" -> ", visited.Select(n => n.Id));
            TxtInfo.Text = $"DFS Sonucu ({startId} başlangıçlı):\n{resultStr}";
        }
        private void BtnDijkstra_Click(object sender, RoutedEventArgs e) 
        { 
            RunShortestPath("Dijkstra");
        }

        private void BtnAStar_Click(object sender, RoutedEventArgs e) 
        { 
            RunShortestPath("AStar"); 
        }
        
        private void RunShortestPath(string algorithmType)
        {
            if (_graph == null) return;
            string src = TxtSourceId.Text;
            string tgt = TxtTargetId.Text;

            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(tgt))
            {
                MessageBox.Show("Lütfen Kaynak ve Hedef düğümleri seçin.");
                return;
            }

            var sp = new ShortestPathAlgorithm();
            List<Node> path;
            
            // Heuristic for A* (Simple placeholder for now, usually Euclidean)
            Func<Node, Node, double> heuristic = (n1, n2) => 0; 
            
            if (algorithmType == "AStar")
            {
                // Note: For real A*, we need positions. Assuming UI positions map to node logic or using dummy
                // For now returning 0 so it behaves like Dijkstra until we have coordinates in Node or use _nodePositions
                 heuristic = (n1, n2) => 
                {
                    if (_nodePositions.ContainsKey(n1.Id) && _nodePositions.ContainsKey(n2.Id))
                    {
                        Point p1 = _nodePositions[n1.Id];
                        Point p2 = _nodePositions[n2.Id];
                        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                    }
                    return 0;
                };
                path = sp.AStar(_graph, src, tgt, heuristic);
            }
            else
            {
                path = sp.Dijkstra(_graph, src, tgt);
            }

            if (path.Count > 0)
            {
                var pathStr = string.Join(" -> ", path.Select(n => n.Id));
                TxtInfo.Text = $"{algorithmType} Yolu:\n{pathStr}";
            }
            else
            {
                TxtInfo.Text = $"{algorithmType}: Yol bulunamadı.";
            }
        }
        private void BtnCentrality_Click(object sender, RoutedEventArgs e)
        {
            if (_graph == null) return;

            var centrality = new CentralityAlgorithm();
            var results = centrality.CalculateDegreeCentrality(_graph, 5); // Top 5

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Derece Merkeziliği (Top 5):");
            foreach (var item in results)
            {
                sb.AppendLine($"{item.Key.Name} ({item.Key.Id}): {item.Value}");
            }

            TxtInfo.Text = sb.ToString();
        }
}