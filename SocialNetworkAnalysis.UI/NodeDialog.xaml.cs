using System.Windows;

namespace SocialNetworkAnalysis.UI;

public partial class NodeDialog : Window
{
    public string NodeId => TxtId.Text.Trim();
    public string NodeName => TxtName.Text.Trim();
    public double Activity => double.TryParse(TxtActivity.Text, out double val) ? val : 0;
    public double Interaction => double.TryParse(TxtInteraction.Text, out double val) ? val : 0;
    public double ConnectionCount => double.TryParse(TxtConnectionCount.Text, out double val) ? val : 0;

    public NodeDialog(string? nodeId = null, string? name = null, double activity = 0, double interaction = 0, double connectionCount = 0)
    {
        InitializeComponent();
        
        if (nodeId != null)
        {
            TxtId.Text = nodeId;
            TxtName.Text = name ?? "";
            TxtActivity.Text = activity.ToString();
            TxtInteraction.Text = interaction.ToString();
            TxtConnectionCount.Text = connectionCount.ToString();
            Title = "Düğüm Güncelle";
        }
        else
        {
            Title = "Düğüm Ekle";
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtId.Text))
        {
            MessageBox.Show("Düğüm ID'si boş olamaz!");
            return;
        }

        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

