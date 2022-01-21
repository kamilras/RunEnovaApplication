using System.Collections.Generic;
using System.Windows.Controls;

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
