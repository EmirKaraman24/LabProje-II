using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using SocialNetworkAnalysis.Core;

namespace SocialNetworkAnalysis.UI
{
    public partial class MainWindow : Window
    {
        private Graph? _graph;
        private FileService _fileService;
        private Dictionary<string, Point> _nodePositions;
        private Random _random;

        public MainWindow()
        {
            InitializeComponent();
            _fileService = new FileService();
            _nodePositions = new Dictionary<string, Point>();
            _random = new Random();
        }

        private void BtnLoadGraph_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _graph = _fileService.LoadGraphFromCsv(openFileDialog.FileName);
                    GenerateRandomPositions();
                    DrawGraph();
                    if (_graph != null)
                    {
                         TxtInfo.Text = $"Graf yüklendi. {_graph.Nodes.Count} düğüm, {_graph.Edges.Count} kenar.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }

        private void BtnSaveGraph_Click(object sender, RoutedEventArgs e)
        {
            if (_graph == null) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                _fileService.SaveGraphToCsv(_graph, saveFileDialog.FileName);
                TxtInfo.Text = "Graf kaydedildi.";
            }
        }

        private void GenerateRandomPositions()
        {
            _nodePositions.Clear();
            if (_graph == null) return;
            
            double width = GraphCanvas.ActualWidth > 0 ? GraphCanvas.ActualWidth : 600;
            double height = GraphCanvas.ActualHeight > 0 ? GraphCanvas.ActualHeight : 500;
            double padding = 50;

            foreach (var node in _graph.Nodes.Values)
            {
                double x = _random.NextDouble() * (width - 2 * padding) + padding;
                double y = _random.NextDouble() * (height - 2 * padding) + padding;
                _nodePositions[node.Id] = new Point(x, y);
            }
        }

        private void DrawGraph()
        {
            GraphCanvas.Children.Clear();

            if (_graph == null) return;

            // Draw Edges first (so they are behind nodes)
            foreach (var edge in _graph.Edges)
            {
                if (_nodePositions.ContainsKey(edge.SourceId) && _nodePositions.ContainsKey(edge.TargetId))
                {
                    Point p1 = _nodePositions[edge.SourceId];
                    Point p2 = _nodePositions[edge.TargetId];

                    Line line = new Line
                    {
                        X1 = p1.X,
                        Y1 = p1.Y,
                        X2 = p2.X,
                        Y2 = p2.Y,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1
                    };
                    
                    // Optional: Show weight as tooltip
                    line.ToolTip = $"Ağırlık: {edge.Weight:F3}";
                    
                    GraphCanvas.Children.Add(line);
                }
            }

            // Draw Nodes
            foreach (var node in _graph.Nodes.Values)
            {
                if (_nodePositions.ContainsKey(node.Id))
                {
                    Point p = _nodePositions[node.Id];
                    
                    // Circle
                    Ellipse ellipse = new Ellipse
                    {
                        Width = 30,
                        Height = 30,
                        Fill = Brushes.CornflowerBlue,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Tag = node.Id
                    };

                    Canvas.SetLeft(ellipse, p.X - 15);
                    Canvas.SetTop(ellipse, p.Y - 15);

                    // Text (ID)
                    TextBlock textBlock = new TextBlock
                    {
                        Text = node.Id,
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold
                    };
                    
                    // Center text approximate
                    Canvas.SetLeft(textBlock, p.X - 5);
                    Canvas.SetTop(textBlock, p.Y - 8);

                    GraphCanvas.Children.Add(ellipse);
                    GraphCanvas.Children.Add(textBlock);
                }
            }
        }

        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Simple hit test or interaction handling can go here
            if (e.OriginalSource is Shape shape && shape.Tag != null)
            {
                string? nodeId = shape.Tag.ToString();
                if (nodeId != null && _graph != null && _graph.Nodes.ContainsKey(nodeId))
                {
                    var node = _graph.Nodes[nodeId];
                    TxtInfo.Text = $"Düğüm: {node.Id}\nAd: {node.Name}\nAktiflik: {node.Activity}\nEtkileşim: {node.Interaction}";
                }
            }
        }

        private void BtnBfs_Click(object sender, RoutedEventArgs e)
        {
            if (_graph == null || _graph.Nodes.Count == 0)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.");
                return;
            }

            // For simplicity, pick the first node or let user select.
            // Requirement says "from a start node". Let's ask for ID or pick random/first.
            // Ideally we should have a selected node.
            
            string startNodeId = _graph.Nodes.Keys.First(); 
            // If user selected a node (clicked), we could use that.
            // Let's check if we can store "SelectedNode" functionality.
            // For now, prompt or just take first.
           
            var bfs = new BfsAlgorithm();
            var result = bfs.Execute(_graph, startNodeId);
            
            var resultStr = string.Join(" -> ", result.Select(n => n.Id));
            TxtInfo.Text = $"BFS Sonucu ({startNodeId} başlangıçlı):\n{resultStr}";
            
            HighlightNodes(result);
        }

        private void BtnDfs_Click(object sender, RoutedEventArgs e)
        {
             if (_graph == null || _graph.Nodes.Count == 0)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.");
                return;
            }

            string startNodeId = _graph.Nodes.Keys.First();
            
            var dfs = new DfsAlgorithm();
            var result = dfs.Execute(_graph, startNodeId);

            var resultStr = string.Join(" -> ", result.Select(n => n.Id));
            TxtInfo.Text = $"DFS Sonucu ({startNodeId} başlangıçlı):\n{resultStr}";
            
            HighlightNodes(result);
        }

        private void HighlightNodes(List<Node> nodes)
        {
            // Reset colors
            foreach (var child in GraphCanvas.Children)
            {
                if (child is Ellipse el)
                {
                    el.Fill = Brushes.CornflowerBlue;
                }
            }
            
            // Highlight visited
            // This is a simple visual indication.
            // Maybe animate? For now just static color change.
            foreach (var node in nodes)
            {
                 foreach (var child in GraphCanvas.Children)
                {
                    if (child is Ellipse el && el.Tag?.ToString() == node.Id)
                    {
                        el.Fill = Brushes.Orange;
                    }
                }
            }
        }

        private void BtnShortestPath_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Henüz eklenmedi."); }
        private void BtnCentrality_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Merkezilik henüz eklenmedi."); }
    }
}