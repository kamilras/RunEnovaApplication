using RunEnova.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System.Linq;
using RunEnovaApplication.Extension;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for ServerSQLConfig.xaml
    /// </summary>
    public partial class ServerSQLConfig : Window
    {
        string WybranaBaza { get; set; }
        public string CatalogExplorer { get; set; }
        public string CatalogSerwer { get; set; }
        public ServerSQLConfig()
        {
            InitializeComponent();
            UstawWlasciwosci();
        }

        private void UstawWlasciwosci()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            CatalogExplorer = config.AppSettings.Settings["SonetaExplorerPath"].Value;
            CatalogSerwer = config.AppSettings.Settings["SonetaSerwerPath"].Value;

            SonetaExplorerTxtBox.Text = CatalogExplorer;
            SonetaSerwerTxtBox.Text = CatalogSerwer;

            ListaSQLCmbBox.ItemsSource = SqlHelper.ListLocalSqlInstances().ToArray();

            string str = ConfigurationManager.ConnectionStrings["BazyEnova"]?.ConnectionString;

            HasloTxtBox = new PasswordBox() { IsEnabled = false };
            WinAutenticationChkBox.IsChecked = true;

            if (string.IsNullOrEmpty(str))
            {
                BazaChkBox.IsEnabled = true;
                BazaChkBox.IsChecked = false;
                MessageBox.Show($"Oznacz serwer na którym zostanie utworzona baza danych 'BazyEnova'.");
            }
            else
            {
                var conn = new SqlConnectionStringBuilder(str);
                ListaSQLCmbBox.SelectedItem = conn.DataSource;
                UzytkownikTxtBox.Text = conn.UserID;
                BazaChkBox.IsChecked = true;

                if (!string.IsNullOrEmpty(conn.UserID))
                    WinAutenticationChkBox.IsChecked = false;

                if (CheckIfDatabaseExist("BazyEnova", str))
                {
                    BazaChkBox.IsChecked = true;
                    BazaChkBox.IsEnabled = false;
                }
                else
                {
                    BazaChkBox.IsChecked = false;
                    BazaChkBox.IsEnabled = true;
                }
            }
        }

        private void DodajSerwerSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WybranaBaza != (string)ListaSQLCmbBox.SelectedItem)
            {
                string baza = (string)ListaSQLCmbBox.SelectedItem;
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

                if (!string.IsNullOrEmpty(baza))
                {
                    DbSetting.CreateConnectionString(baza, "BazyEnova", user, pass);
                }
            }

            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BazaChkBox.IsChecked = false;
            BazaChkBox.IsEnabled = true;
        }

        public bool CheckIfDatabaseExist(string databaseName, string connString)
        {
            var cmdText = "select count(*) from master.dbo.sysdatabases where name=@database";

            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCmd = new SqlCommand(cmdText, sqlConnection))
                {
                    sqlCmd.Parameters.Add("@database", SqlDbType.NVarChar).Value = databaseName;

                    sqlConnection.Open();

                    return Convert.ToInt32(sqlCmd.ExecuteScalar()) > 0;
                }
            }
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

        private void SelectExplorerCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.ProgramFilesX86;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                CatalogExplorer = dialog.SelectedPath;
            }

            SonetaExplorerTxtBox.Text = CatalogExplorer;

            config.AppSettings.Settings["SonetaExplorerPath"].Value = CatalogExplorer;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void SelectSerwerCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                CatalogSerwer = dialog.SelectedPath;
            }

            SonetaSerwerTxtBox.Text = CatalogSerwer;

            config.AppSettings.Settings["SonetaSerwerPath"].Value = CatalogSerwer;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}