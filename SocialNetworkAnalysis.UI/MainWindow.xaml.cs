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

namespace SocialNetworkAnalysis.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLoadCsv_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CSV Yükleme Henüz Hazır Değil");
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