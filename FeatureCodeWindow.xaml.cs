using System;
using System.Windows;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for FeatureCodeWindow.xaml
    /// </summary>
    public partial class FeatureCodeWindow : Window
    {
        public FeatureCodeWindow(string code)
        {
            InitializeComponent();
            CodeTxtBox.Text = code;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FeaturesPage.FeatureCodeWindow = null;
        }
    }
}
