using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    private JsonFileService _jsonFileService;
    private Dictionary<string, Point> _nodePositions = new Dictionary<string, Point>();
    private Dictionary<string, Ellipse> _nodeShapes = new Dictionary<string, Ellipse>();
    private Dictionary<string, Line> _edgeShapes = new Dictionary<string, Line>();
    private Dictionary<string, int> _nodeColors = new Dictionary<string, int>();
    private string? _selectedNodeId = null;

    // Renk paleti
    private static readonly Brush[] ColorPalette = new Brush[]
    {
        Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Orange, Brushes.Purple,
        Brushes.Brown, Brushes.Pink, Brushes.Cyan, Brushes.Yellow, Brushes.Magenta,
        Brushes.Lime, Brushes.Teal, Brushes.Navy, Brushes.Maroon, Brushes.Olive
    };

    public MainWindow()
    {
        InitializeComponent();
        _fileService = new FileService();
        _jsonFileService = new JsonFileService();
    }

    /// <summary>
    /// Grafiği canvas üzerinde görselleştirir.
    /// </summary>
    private void DrawGraph()
    {
        if (_graph == null || _graph.Nodes.Count == 0) return;

        GraphCanvas.Children.Clear();
        _nodeShapes.Clear();
        _edgeShapes.Clear();

        // Düğüm pozisyonlarını hesapla (dairesel yerleşim)
        CalculateNodePositions();

        // Kenarları çiz
        DrawEdges();

        // Düğümleri çiz
        DrawNodes();
    }

    /// <summary>
    /// Düğüm pozisyonlarını dairesel yerleşim algoritması ile hesaplar.
    /// </summary>
    private void CalculateNodePositions()
    {
        if (_graph == null) return;

        var nodes = _graph.Nodes.Values.ToList();
        int count = nodes.Count;
        if (count == 0) return;

        double centerX = GraphCanvas.ActualWidth / 2;
        double centerY = GraphCanvas.ActualHeight / 2;
        double radius = Math.Min(centerX, centerY) * 0.7;

        for (int i = 0; i < count; i++)
        {
            double angle = 2 * Math.PI * i / count;
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY + radius * Math.Sin(angle);
            _nodePositions[nodes[i].Id] = new Point(x, y);
        }
    }

    /// <summary>
    /// Kenarları çizer.
    /// </summary>
    private void DrawEdges()
    {
        if (_graph == null) return;

        foreach (var edge in _graph.Edges)
        {
            if (!_nodePositions.ContainsKey(edge.SourceId) || !_nodePositions.ContainsKey(edge.TargetId))
                continue;

            var p1 = _nodePositions[edge.SourceId];
            var p2 = _nodePositions[edge.TargetId];

            var line = new Line
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = Brushes.Gray,
                StrokeThickness = 2
            };

            string edgeKey = $"{edge.SourceId}-{edge.TargetId}";
            _edgeShapes[edgeKey] = line;
            GraphCanvas.Children.Add(line);
        }
    }

    /// <summary>
    /// Düğümleri çizer.
    /// </summary>
    private void DrawNodes()
    {
        if (_graph == null) return;

        foreach (var node in _graph.Nodes.Values)
        {
            if (!_nodePositions.ContainsKey(node.Id)) continue;

            var pos = _nodePositions[node.Id];
            Brush fillColor = GetNodeColor(node.Id);

            var ellipse = new Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = fillColor,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(ellipse, pos.X - 20);
            Canvas.SetTop(ellipse, pos.Y - 20);

            // Düğüm ID'sini göster
            var textBlock = new TextBlock
            {
                Text = node.Id,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(textBlock, pos.X - 15);
            Canvas.SetTop(textBlock, pos.Y - 10);

            _nodeShapes[node.Id] = ellipse;
            GraphCanvas.Children.Add(ellipse);
            GraphCanvas.Children.Add(textBlock);
        }
    }

    /// <summary>
    /// Düğüm rengini döndürür.
    /// </summary>
    private Brush GetNodeColor(string nodeId)
    {
        if (_nodeColors.ContainsKey(nodeId))
        {
            int colorIndex = _nodeColors[nodeId] % ColorPalette.Length;
            return ColorPalette[colorIndex];
        }
        return Brushes.LightGray;
    }

    /// <summary>
    /// Renklendirmeyi uygular.
    /// </summary>
    private void ApplyColoring(Dictionary<string, int> coloring)
    {
        _nodeColors = coloring;
        DrawGraph();
    }

    #region File Operations

    private void BtnLoadCsv_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _graph = _fileService.LoadGraphFromCsv(openFileDialog.FileName);
                stopwatch.Stop();

                MessageBox.Show($"Grafik yüklendi! {_graph.Nodes.Count} düğüm, {_graph.Edges.Count} kenar.\nYükleme süresi: {stopwatch.ElapsedMilliseconds} ms");
                DrawGraph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    private void BtnLoadJson_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "JSON Dosyaları (*.json)|*.json|Tüm Dosyalar (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _graph = _jsonFileService.LoadGraphFromJson(openFileDialog.FileName);
                stopwatch.Stop();

                MessageBox.Show($"Grafik yüklendi! {_graph.Nodes.Count} düğüm, {_graph.Edges.Count} kenar.\nYükleme süresi: {stopwatch.ElapsedMilliseconds} ms");
                DrawGraph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    private void BtnSaveCsv_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv";
        saveFileDialog.DefaultExt = "csv";
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                _fileService.SaveGraphToCsv(_graph, saveFileDialog.FileName);
                MessageBox.Show("Grafik CSV olarak kaydedildi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    private void BtnSaveJson_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "JSON Dosyaları (*.json)|*.json";
        saveFileDialog.DefaultExt = "json";
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                _jsonFileService.SaveGraphToJson(_graph, saveFileDialog.FileName);
                MessageBox.Show("Grafik JSON olarak kaydedildi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    private void BtnExportMatrix_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv";
        saveFileDialog.DefaultExt = "csv";
        saveFileDialog.FileName = "adjacency_matrix.csv";
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                _fileService.SaveAdjacencyMatrixToCsv(_graph, saveFileDialog.FileName);
                MessageBox.Show("Komşuluk matrisi kaydedildi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    private void BtnExportList_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv";
        saveFileDialog.DefaultExt = "csv";
        saveFileDialog.FileName = "adjacency_list.csv";
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                _fileService.SaveAdjacencyListToCsv(_graph, saveFileDialog.FileName);
                MessageBox.Show("Komşuluk listesi kaydedildi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    #endregion

    #region Node/Edge Management

    private void BtnAddNode_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

            var dialog = new NodeDialog();
            if (dialog.ShowDialog() == true)
            {
                var node = new Node(dialog.NodeId, dialog.NodeName, dialog.Activity, dialog.Interaction, dialog.ConnectionCount);
                
                if (!_graph.AddNode(node))
                {
                    MessageBox.Show("Bu ID'ye sahip bir düğüm zaten var veya ID geçersiz!");
                    return;
                }
                
                DrawGraph();
                MessageBox.Show("Düğüm eklendi!");
            }
    }

    private void BtnRemoveNode_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null || string.IsNullOrEmpty(_selectedNodeId))
        {
            MessageBox.Show("Lütfen silmek için bir düğüm seçin!");
            return;
        }

        if (MessageBox.Show($"'{_selectedNodeId}' düğümünü silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            _graph.RemoveNode(_selectedNodeId);
            _selectedNodeId = null;
            DrawGraph();
            MessageBox.Show("Düğüm silindi!");
        }
    }

    private void BtnUpdateNode_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null || string.IsNullOrEmpty(_selectedNodeId))
        {
            MessageBox.Show("Lütfen güncellemek için bir düğüm seçin!");
            return;
        }

        var node = _graph.Nodes[_selectedNodeId];
        var dialog = new NodeDialog(node.Id, node.Name, node.Activity, node.Interaction, node.ConnectionCount);
        if (dialog.ShowDialog() == true)
        {
            _graph.UpdateNode(_selectedNodeId, dialog.NodeName, dialog.Activity, dialog.Interaction, dialog.ConnectionCount);
            DrawGraph();
            MessageBox.Show("Düğüm güncellendi!");
        }
    }

    private void BtnAddEdge_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

        var dialog = new EdgeDialog(_graph.Nodes.Keys.ToList());
        if (dialog.ShowDialog() == true)
        {
            if (_graph.Nodes.ContainsKey(dialog.SourceId) && _graph.Nodes.ContainsKey(dialog.TargetId))
            {
                var sourceNode = _graph.Nodes[dialog.SourceId];
                var targetNode = _graph.Nodes[dialog.TargetId];
                double weight = WeightCalculator.CalculateWeight(sourceNode, targetNode);
                _graph.AddEdge(dialog.SourceId, dialog.TargetId, weight);
                DrawGraph();
                MessageBox.Show("Kenar eklendi!");
            }
        }
    }

    private void BtnRemoveEdge_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null)
        {
            MessageBox.Show("Önce bir graf yükleyin!");
            return;
        }

        var dialog = new EdgeDialog(_graph.Nodes.Keys.ToList());
        dialog.Title = "Kenar Sil";
        if (dialog.ShowDialog() == true)
        {
            if (_graph.RemoveEdge(dialog.SourceId, dialog.TargetId))
            {
                DrawGraph();
                MessageBox.Show("Kenar silindi!");
            }
            else
            {
                MessageBox.Show("Kenar bulunamadı!");
            }
        }
    }

    #endregion

    #region Algorithms

    private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_graph == null) return;

        var clickPoint = e.GetPosition(GraphCanvas);

        // Tıklanan düğümü bul
        foreach (var kvp in _nodePositions)
        {
            var pos = kvp.Value;
            double distance = Math.Sqrt(Math.Pow(clickPoint.X - pos.X, 2) + Math.Pow(clickPoint.Y - pos.Y, 2));
            
            if (distance <= 20) // Düğüm yarıçapı
            {
                _selectedNodeId = kvp.Key;
                var node = _graph.Nodes[kvp.Key];
                
                // Düğüm bilgilerini göster
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Düğüm ID: {node.Id}");
                sb.AppendLine($"İsim: {node.Name}");
                sb.AppendLine($"Aktiflik: {node.Activity:F2}");
                sb.AppendLine($"Etkileşim: {node.Interaction:F2}");
                sb.AppendLine($"Bağlantı Sayısı: {node.ConnectionCount:F2}");
                sb.AppendLine($"Komşu Sayısı: {node.Neighbors.Count}");
                sb.AppendLine($"Komşular: {string.Join(", ", node.Neighbors)}");
                
                TxtInfo.Text = sb.ToString();

                // Seçim moduna göre kaynak/hedef ayarla
                if (RadioSource.IsChecked == true)
                {
                    TxtSourceId.Text = kvp.Key;
                }
                else if (RadioTarget.IsChecked == true)
                {
                    TxtTargetId.Text = kvp.Key;
                }

                // Seçili düğümü vurgula
                DrawGraph();
                if (_nodeShapes.ContainsKey(kvp.Key))
                {
                    _nodeShapes[kvp.Key].Stroke = Brushes.Red;
                    _nodeShapes[kvp.Key].StrokeThickness = 4;
                }

                break;
            }
        }
    }

    private void BtnBfs_Click(object sender, RoutedEventArgs e)
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

        var stopwatch = Stopwatch.StartNew();
        var bfs = new BfsAlgorithm();
        var visited = bfs.Execute(_graph, startId);
        stopwatch.Stop();

        var resultStr = string.Join(" -> ", visited.Select(n => n.Id));
        TxtInfo.Text = $"BFS Sonucu ({startId} başlangıçlı):\n{resultStr}\n\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms";
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

        var stopwatch = Stopwatch.StartNew();
        var dfs = new DfsAlgorithm();
        var visited = dfs.Execute(_graph, startId);
        stopwatch.Stop();

        var resultStr = string.Join(" -> ", visited.Select(n => n.Id));
        TxtInfo.Text = $"DFS Sonucu ({startId} başlangıçlı):\n{resultStr}\n\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms";
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

        var stopwatch = Stopwatch.StartNew();
        List<Node> path;

        if (algorithmType == "AStar")
        {
            var aStar = new AStarAlgorithm();
            path = aStar.FindPath(_graph, src, tgt);
        }
        else
        {
            var dijkstra = new DijkstraAlgorithm();
            path = dijkstra.FindPath(_graph, src, tgt);
        }

        stopwatch.Stop();

        if (path.Count > 0)
        {
            var pathStr = string.Join(" -> ", path.Select(n => n.Id));
            TxtInfo.Text = $"{algorithmType} Yolu:\n{pathStr}\n\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms";
        }
        else
        {
            TxtInfo.Text = $"{algorithmType}: Yol bulunamadı.\n\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms";
        }
    }

    private void BtnCentrality_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null) return;

        var stopwatch = Stopwatch.StartNew();
        var centrality = new CentralityAlgorithm();
        var results = centrality.CalculateDegreeCentrality(_graph, 5);
        stopwatch.Stop();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Derece Merkeziliği (Top 5):");
        sb.AppendLine("─────────────────────────");
        int rank = 1;
        foreach (var item in results)
        {
            sb.AppendLine($"{rank}. {item.Key.Name} ({item.Key.Id}): {item.Value}");
            rank++;
        }
        sb.AppendLine($"\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms");

        TxtInfo.Text = sb.ToString();
    }

    private void BtnConnectedComponents_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null) return;

        var stopwatch = Stopwatch.StartNew();
        var componentsAlgo = new ConnectedComponentsAlgorithm();
        var components = componentsAlgo.FindComponents(_graph);
        stopwatch.Stop();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Bağlı Bileşenler ({components.Count} adet):");
        sb.AppendLine("─────────────────────────");
        
        for (int i = 0; i < components.Count; i++)
        {
            sb.AppendLine($"Bileşen {i + 1}: {string.Join(", ", components[i].Select(n => n.Id))}");
        }
        sb.AppendLine($"\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms");

        TxtInfo.Text = sb.ToString();
    }

    private void BtnColoring_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null) return;

        var stopwatch = Stopwatch.StartNew();
        var coloringAlgo = new WelshPowellColoringAlgorithm();
        var coloring = coloringAlgo.ColorGraphByComponents(_graph);
        var colorGroups = coloringAlgo.GetColorGroups(_graph);
        stopwatch.Stop();

        ApplyColoring(coloring);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Welsh-Powell Renklendirme:");
        sb.AppendLine("─────────────────────────");
        sb.AppendLine($"Kullanılan Renk Sayısı: {colorGroups.Count}");
        sb.AppendLine("\nRenk Grupları:");
        
        foreach (var kvp in colorGroups.OrderBy(x => x.Key))
        {
            sb.AppendLine($"Renk {kvp.Key}: {string.Join(", ", kvp.Value.Select(n => n.Id))}");
        }
        sb.AppendLine($"\nÇalışma Süresi: {stopwatch.ElapsedMilliseconds} ms");

        TxtInfo.Text = sb.ToString();
    }

    #endregion
}
