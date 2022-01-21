using System.Collections.Generic;
using System.Windows;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info(string baza, List<string> systemInfos, Dictionary<string, string> features, string connString)
        {
            InitializeComponent();
            NazwaBazyTxt.Text = baza;

            ConnString = connString;

            Features = features;
            SystemInfos = systemInfos;

            Main.Content = new SystemInfosPage(SystemInfos);
        }
        Dictionary<string, string> Features { get; set; }
        List<string> SystemInfos { get; set; }
        public string ConnString { get; set; }

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
            Main.Content = new UstawieniaPage(ConnString);
        }
    }
}
