using RunEnova.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System.Linq;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for ServerSQLConfig.xaml
    /// </summary>
    public partial class ServerSQLConfig : Window
    {
        List<string> lista = new List<string>();
        public ServerSQLConfig()
        {
            InitializeComponent();
            BazaChkBox.IsEnabled = false;
        }

        private void DodajSerwerSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(SerwerSQLTxt.Text))
            //{
            //    string serwer = WybierzNazweSerwera();
            //    string connectionString = $"Server={SerwerSQLTxt.Text};Trusted_Connection=True;";
            //    DbSetting.AddOrUpdateAppSetting($"ConnectionStrings:{serwer}", connectionString);
            //}
            this.Close();
        }

        private const string defaultMsSqlInstanceName = "MSSQLSERVER";

        public static string[] GetLocalSqlServerInstances()
        {
            return new ManagedComputer()
                .ServerInstances
                .Cast<ServerInstance>()
                .Select(instance => string.IsNullOrEmpty(instance.Name) || instance.Name == defaultMsSqlInstanceName ?
                    instance.Parent.Name : System.IO.Path.Combine(instance.Parent.Name, instance.Name))
                .ToArray();
        }

        private void ListaSQLCmbBox_DropDownOpened(object sender, EventArgs e)
        {
            ListaSQLCmbBox.ItemsSource = GetLocalSqlServerInstances();
        }

        private void ListaSQLCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
            string str = DbSetting.GetConnectionString("BazyEnova");
            if (str == null)
            {
                BazaChkBox.IsEnabled = true;
                MessageBox.Show($"Oznacz serwer na którym zostanie utworzona baza danych 'BazyEnova'.");
                return;
            }

            string[] provider = str.Split(';');

            BazaChkBox.IsChecked = provider[0] == $"Server={ListaSQLCmbBox.SelectedItem}";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (BazaChkBox.IsEnabled)
                BazaChkBox.IsEnabled = false;
            else
                return;

            string selected = (string)ListaSQLCmbBox.SelectedItem;
            string connectionString = $"Server={selected};Database=Baza;Trusted_Connection=True;";
            DbSetting.AddOrUpdateAppSetting($"ConnectionStrings:BazyEnova", connectionString);
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
    }
}