﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.SqlServer.Management.HadrData;
using System.Windows.Markup;
using RunEnova.Extension;
using RunEnova.Model;
using RunEnovaApplication;
using RunEnovaApplication.Extension;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for ServerSQLConfig.xaml
    /// </summary>
    public partial class ServerSQLConfig : Window
    {
        public string DomyslnySerwer { get; set; }
        public string CatalogExplorer { get; set; }
        public string CatalogSerwer { get; set; }
        public string CatalogListaBazDanych { get; set; }
        public List<string> LocalServer { get; set; }
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
            CatalogListaBazDanych = config.AppSettings.Settings["ListaBazDanychPath"].Value;

            SonetaExplorerTxtBox.Text = CatalogExplorer;
            SonetaSerwerTxtBox.Text = CatalogSerwer;
            ListaBazDancyhTxtBox.Text = CatalogListaBazDanych;

            bool sprawdzBaze = true;

            LocalServer = SqlHelper.ListLocalSqlInstances().ToList();

            List<string> allServer = new List<string>();
            allServer.AddRange(LocalServer);

            foreach (ConnectionStringSettings conStr in ConfigurationManager.ConnectionStrings)
            {
                SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(conStr.ConnectionString);

                if (conStr.Name.ToLower() == connectionBuilder.DataSource.ToLower() && !allServer.Contains(connectionBuilder.DataSource))
                    allServer.Add(conStr.Name);
            }

            ListaSQLCmbBox.ItemsSource = allServer;

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                BazaChkBox.IsEnabled = true;
                BazaChkBox.IsChecked = false;
                MessageBox.Show($"Oznacz serwer na którym zostanie utworzona baza danych 'BazyEnova'.");
                return;
            }

            WinAutenticationChkBox.IsChecked = true;

            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder(connectionString);

            if (conn.DataSource == ".")
            {
                conn.DataSource = Environment.MachineName;
                AppConfig.AddConnectionString("DefaultConnection", conn.DataSource, conn.InitialCatalog, null, null);
                ConfigurationManager.RefreshSection("connectionStrings");
                sprawdzBaze = false;
            }

            DomyslnySerwer = conn.DataSource;

            if (allServer.Contains(DomyslnySerwer))
            {
                ListaSQLCmbBox.SelectedItem = DomyslnySerwer;

                BazaChkBox.IsChecked = true;

                if (!string.IsNullOrEmpty(conn.UserID))
                {
                    UzytkownikTxtBox.Text = conn.UserID;
                    HasloTxtBox.Password = conn.Password;
                    WinAutenticationChkBox.IsChecked = false;
                }

                if (sprawdzBaze && CheckIfDatabaseExist("BazyEnova", conn.ConnectionString))
                    BazaChkBox.IsEnabled = false;
            }
        }

        private void DodajSerwerSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (string serwer in ListaSQLCmbBox.Items.Cast<string>())
            {
                if (string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[serwer]?.ConnectionString))
                    AppConfig.AddConnectionString(serwer, serwer, "BazyEnova", null, null);
            }

            ConfigurationManager.RefreshSection("connectionStrings");

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["SonetaSerwerPath"].Value = CatalogSerwer;
            config.AppSettings.Settings["SonetaExplorerPath"].Value = CatalogExplorer;
            config.AppSettings.Settings["ListaBazDanychPath"].Value = CatalogListaBazDanych;

            config.Save(ConfigurationSaveMode.Modified);

            ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings[DomyslnySerwer];
            ConnectionStringSettings defaultConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            if (connection?.ConnectionString != defaultConnection?.ConnectionString)
            {
                AppConfig.ChangeConnectionString("DefaultConnection", connection);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            ConfigurationManager.RefreshSection("appSettings");

            this.Close();
        }

        public bool CheckIfDatabaseExist(string databaseName, string connString)
        {
            var cmdText = "select count(*) from master.dbo.sysdatabases where name=@database";

            using (var conn = new SqlConnection(connString))
            {
                using (var sqlCmd = new SqlCommand(cmdText, conn))
                {
                    sqlCmd.Parameters.Add("@database", SqlDbType.NVarChar).Value = databaseName;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas sprawdzania istnienia bazy " + conn.Database + " na serwerze - " + conn.DataSource + Environment.NewLine + ex.Message);
                        return false;
                    }
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
            if (string.IsNullOrEmpty(CatalogExplorer))
                CatalogExplorer = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + $"\\Soneta";

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = CatalogExplorer;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                CatalogExplorer = dialog.SelectedPath;
            }
            SonetaExplorerTxtBox.Text = CatalogExplorer;
        }

        private void SelectSerwerCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CatalogSerwer))
            {
                if (Directory.Exists($"C:\\Multi"))
                    CatalogSerwer = $"C:\\Multi";
                else
                    CatalogSerwer = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = CatalogSerwer;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                CatalogSerwer = dialog.SelectedPath;
            }
            SonetaSerwerTxtBox.Text = CatalogSerwer;
        }

        private void ListaBazDanychCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CatalogListaBazDanych))
                CatalogListaBazDanych = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\Soneta";

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = CatalogListaBazDanych;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                CatalogListaBazDanych = dialog.SelectedPath;
            }
            ListaBazDancyhTxtBox.Text = CatalogListaBazDanych;
        }

        private void ListaSQLCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string aktualnySerwer = (string)((ComboBox)sender).SelectedItem;

            if (DomyslnySerwer == aktualnySerwer)
                BazaChkBox.IsChecked = true;
            else
                BazaChkBox.IsChecked = false;

            string str = ConfigurationManager.ConnectionStrings[aktualnySerwer]?.ConnectionString;

            if (string.IsNullOrEmpty(str))
                return;

            var conn = new SqlConnectionStringBuilder(str);

            if (!string.IsNullOrEmpty(conn.UserID))
            {
                UzytkownikTxtBox.Text = conn.UserID;
                HasloTxtBox.Password = conn.Password;
                WinAutenticationChkBox.IsChecked = false;
            }
            else
            {
                UzytkownikTxtBox.Text = string.Empty;
                HasloTxtBox.Password = string.Empty;
                WinAutenticationChkBox.IsChecked = true;
            }
        }

        private void ZapiszSerwerBtn_Click(object sender, RoutedEventArgs e)
        {
            string serwer = (string)ListaSQLCmbBox.SelectedItem;

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

                if ((bool)BazaChkBox.IsChecked)
                {
                    DomyslnySerwer = serwer;
                }
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            MessageBox.Show("Zapisano ustawienia serwera");
        }

        private void DodajSerwerBtn_Click(object sender, RoutedEventArgs e)
        {
            DodajSerwer config = new DodajSerwer();
            config.ShowDialog();
        }

        private void UsunSerwerBtn_Click(object sender, RoutedEventArgs e)
        {
            string serwer = (string)ListaSQLCmbBox.SelectedItem;

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                ConnectionStringsSection csSection = config.ConnectionStrings;
                ConnectionStringSettingsCollection csCollection = csSection.ConnectionStrings;

                ConnectionStringSettings cs = csCollection[serwer];

                if (cs != null)
                {
                    if (LocalServer.Contains(cs.Name))
                    {
                        System.Windows.Forms.MessageBox.Show("Nie można usunąć serwera lokalnego " + cs.Name);
                        return;
                    }

                    csCollection.Remove(cs);
                    config.Save(ConfigurationSaveMode.Modified);

                    ConfigurationManager.RefreshSection("connectionStrings");

                    ListaSQLCmbBox.SelectedItem = ListaSQLCmbBox.Items.Cast<string>().FirstOrDefault();


                    System.Windows.Forms.MessageBox.Show($"Serwer '{cs.Name}' został usunięty z listy");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Nie odnaleziony ConnectionString dla " + serwer);
                }
            }
            catch (ConfigurationErrorsException err)
            {
                Console.WriteLine(err.ToString());
            }
        }

        private void ListaSQLCmbBox_DropDownOpened(object sender, EventArgs e)
        {
            List<string> listaSerwerow = SqlHelper.ListLocalSqlInstances().ToList();

            foreach (ConnectionStringSettings conStr in ConfigurationManager.ConnectionStrings)
            {
                SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(conStr.ConnectionString);

                if (conStr.Name.ToLower() == connectionBuilder.DataSource.ToLower() && !listaSerwerow.Contains(connectionBuilder.DataSource))
                    listaSerwerow.Add(conStr.Name);
            }

            ListaSQLCmbBox.ItemsSource = listaSerwerow;
        }
    }
}