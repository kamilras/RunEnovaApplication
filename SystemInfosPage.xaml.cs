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
    /// Interaction logic for SystemInfosPage.xaml
    /// </summary>
    public partial class SystemInfosPage : Page
    {
        public SystemInfosPage(List<string> systemInfos)
        {
            InitializeComponent();
            SystemInfosListBox.ItemsSource = systemInfos;
        }
    }
}
