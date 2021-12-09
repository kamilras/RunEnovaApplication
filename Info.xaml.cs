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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info(string baza, List<string> systemInfos, Dictionary<string, string> features)
        {
            InitializeComponent();
            NazwaBazyTxt.Text = baza;

            Features = features;
            SystemInfos = systemInfos;

            Main.Content = new SystemInfosPage(SystemInfos);
        }
        Dictionary<string, string> Features { get; set; }
        List<string> SystemInfos { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new FeaturesPage(Features);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Main.Content = new SystemInfosPage(SystemInfos);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Main.Content = new UstawieniaPage();
        }
    }
}
