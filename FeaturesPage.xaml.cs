using System;
using System.Collections.Generic;
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

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for FeaturesPage.xaml
    /// </summary>
    public partial class FeaturesPage : Page
    {
        public FeaturesPage(Dictionary<string, string> features)
        {
            InitializeComponent();
            Features = features;
            FeaturesListBox.ItemsSource = features.Keys;
        }

        public Dictionary<string, string> Features { get; set; }

        public static FeatureCodeWindow FeatureCodeWindow { get; set; }

        private void FeaturesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = ((ListBox)sender).SelectedItem.ToString();

            if (string.IsNullOrEmpty(Features[selected]))
            {
                FeatureCodeWindow?.Close();
                return;
            }

            if (FeatureCodeWindow == null)
                FeatureCodeWindow = new FeatureCodeWindow(Features[selected]);
            else
                FeatureCodeWindow.CodeTxtBox.Text = Features[selected];

            FeatureCodeWindow.SizeToContent = SizeToContent.WidthAndHeight;
            FeatureCodeWindow.Show();
        }
    }
}
