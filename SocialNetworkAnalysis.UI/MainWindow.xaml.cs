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

        private void BtnBfs_Click(object sender, RoutedEventArgs e) { MessageBox.Show("BFS Henüz Hazır Değil"); }
        private void BtnDfs_Click(object sender, RoutedEventArgs e) { MessageBox.Show("DFS Henüz Hazır Değil"); }
        private void BtnDijkstra_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Dijkstra Henüz Hazır Değil"); }
        private void BtnAStar_Click(object sender, RoutedEventArgs e) { MessageBox.Show("A* Henüz Hazır Değil"); }
        private void BtnCentrality_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Merkezilik Henüz Hazır Değil"); }
}