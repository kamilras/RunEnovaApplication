using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RunEnova.Extension;

namespace RunEnovaApplication
{
    /// <summary>
    /// Interaction logic for DodajSerwer.xaml
    /// </summary>
    public partial class DodajSerwer : Window
    {
        public DodajSerwer()
        {
            InitializeComponent();
        }

        private void WinAutenticationChkBox_Checked(object sender, RoutedEventArgs e)
        {
            UzytkownikTxtBox.IsEnabled = false;
            HasloTxtBox.IsEnabled = false;
        }

        private void WinAutenticationChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UzytkownikTxtBox.IsEnabled = true;
            HasloTxtBox.IsEnabled = true;
        }

        private void DodajSerwerBtn_Click(object sender, RoutedEventArgs e)
        {
            string serwer = SerwerTxtBox.Text;

            if (!string.IsNullOrEmpty(serwer))
            {
                string pass = null;
                string user = null;

                if ((bool)!WinAutenticationChkBox.IsChecked)
                {
                    if (!string.IsNullOrEmpty(UzytkownikTxtBox.Text))
                    {
                        user = UzytkownikTxtBox.Text;
                    }
                    if (!string.IsNullOrEmpty(HasloTxtBox.Password))
                    {
                        pass = HasloTxtBox.Password;
                    }
                }

                AppConfig.AddConnectionString(serwer, serwer, "BazyEnova", user, pass);
                ConfigurationManager.RefreshSection("connectionStrings");
            }

            Close();
        }
    }
}
