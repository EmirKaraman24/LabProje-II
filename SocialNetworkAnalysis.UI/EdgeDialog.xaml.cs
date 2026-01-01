using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SocialNetworkAnalysis.UI;

public partial class EdgeDialog : Window
{
    public string SourceId => CmbSource.Text.Trim();
    public string TargetId => CmbTarget.Text.Trim();

    public EdgeDialog(List<string> nodeIds)
    {
        InitializeComponent();
        
        CmbSource.ItemsSource = nodeIds;
        CmbTarget.ItemsSource = nodeIds;
        
        if (nodeIds.Count > 0)
        {
            CmbSource.SelectedIndex = 0;
            if (nodeIds.Count > 1)
                CmbTarget.SelectedIndex = 1;
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SourceId) || string.IsNullOrWhiteSpace(TargetId))
        {
            MessageBox.Show("Kaynak ve Hedef düğümleri seçin!");
            return;
        }

        if (SourceId == TargetId)
        {
            MessageBox.Show("Kaynak ve Hedef düğümleri aynı olamaz!");
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

