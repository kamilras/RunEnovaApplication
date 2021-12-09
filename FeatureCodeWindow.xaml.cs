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
using System.Windows.Shapes;

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
