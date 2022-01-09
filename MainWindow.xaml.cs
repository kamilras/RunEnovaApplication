using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RunEnova.Extension;
using RunEnova.Model;
using RunEnovaApplication.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BazaDb Context { get; set; }
        public Baza Baza { get; set; }
        public Dictionary<string, string> ListaBazSQL { get; set; }
        public static string AktualnaBazaSQL { get; set; }
        public string SonetaExplorerParam = "";
        public string SonetaServerParam = "";
        public string SonetaExplorerCatalog { get; set; }
        public string SonetaSerwerCatalog { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SonetaSerwerCatalog = config.AppSettings.Settings["SonetaSerwerPath"].Value;
            SonetaExplorerCatalog = config.AppSettings.Settings["SonetaExplorerPath"].Value;

            DirectoryInfo di = new DirectoryInfo($"{SonetaExplorerCatalog}");
            WersjaComboBox.ItemsSource = di.GetDirectories().Select(x => x.Name).ToList();
        }

        private void LoadContext(string database_name)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[database_name]?.ConnectionString;
            if (connectionString == null)
            {
                MessageBox.Show("Nie znaleziono wpisu w App.config dla bazy - " + database_name);
                return;
            }
            Context = new BazaDb();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Baza == null)
            {
                MessageBox.Show("Nie wybrano żadnej bazy danych");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();

            if ((bool)SonetaExplorerRadioBtn?.IsChecked)
            {
                startInfo.FileName = $"{SonetaExplorerCatalog}\\{WersjaComboBox.Text}\\SonetaExplorer.exe";
            }
            else
            {
                startInfo.FileName = $"{SonetaSerwerCatalog}\\{WersjaComboBox.Text}\\SonetaServer.exe";
            }
            startInfo.Arguments = PanelTxt.Text;

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void WersjaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Baza == null)
                return;

            if (((ComboBox)sender).SelectedItem == null)
                return;

            if ((bool)SonetaServerRadioBtn?.IsChecked)
                Baza.FolderServ = ((ComboBox)sender).SelectedItem?.ToString();
            else
                Baza.FolderApp = ((ComboBox)sender).SelectedItem?.ToString();
        }
        private void SonetaExplorerRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SonetaExplorerCatalog))
                return;

            DirectoryInfo di = new DirectoryInfo($"{SonetaExplorerCatalog}");
            WersjaComboBox.ItemsSource = di.GetDirectories().Select(x => x.Name).ToList();
            if (Baza != null)
                WersjaComboBox.SelectedItem = Baza?.FolderApp;

            OdswiezUstawienia();
        }

        private string PanelDlaSonetaExplorer(string sonetaExplorerParam)
        {
            if (string.IsNullOrEmpty(sonetaExplorerParam))
                return string.Empty;

            if (!string.IsNullOrEmpty(Baza.NazwaBazy))
                return $"/database={Baza.NazwaBazy} {sonetaExplorerParam}";
            return sonetaExplorerParam;
        }

        private string PanelDlaSonetaSerwer(string sonetaServerParam)
        {
            if (string.IsNullOrEmpty(sonetaServerParam))
                return string.Empty;

            if (!string.IsNullOrEmpty(Baza.NazwaBazy))
                return $"/console /database={Baza.NazwaBazy} {sonetaServerParam}";
            return "/console " + sonetaServerParam;
        }

        private void SonetaServerRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SonetaSerwerCatalog))
                return;

            DirectoryInfo dir = new DirectoryInfo($"{SonetaSerwerCatalog}");
            WersjaComboBox.ItemsSource = dir.GetDirectories().Select(x => x.Name).ToList();
            if (Baza != null)
                WersjaComboBox.SelectedItem = Baza?.FolderServ;

            OdswiezUstawienia();
        }

        private void ZapiszBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Context == null || Context.Baza == null)
            {
                MessageBox.Show("Nie wybrano żadnej bazy danych");
                return;
            }

            if (Context.Baza.FirstOrDefault(x => x.Id == Baza.Id) == null)
                Context.Baza.Add(Baza);

            Context.SaveChanges();

            MessageBox.Show("Zapisano ustawienia!");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string nazwa_bazy = ((ComboBox)sender).SelectedItem.ToString();

            AktualnaBazaSQL = ListaBazSQL[nazwa_bazy];

            LoadContext("BazyEnova");
            Baza c = Context?.Baza?.FirstOrDefault(x => x.NazwaBazy == AktualnaBazaSQL);
            if (c != null)
                Baza = c;
            else
                Baza = new Baza() { NazwaBazy = AktualnaBazaSQL };

            OdswiezUstawienia();
        }

        private void OdswiezUstawienia()
        {
            if (Baza == null)
                return;

            Config.ShowOnPanel(Baza, out SonetaExplorerParam, out SonetaServerParam);

            if ((bool)SonetaExplorerRadioBtn?.IsChecked)
            {
                WersjaComboBox.SelectedItem = Baza.FolderApp;
                PanelTxt.Text = PanelDlaSonetaExplorer(SonetaExplorerParam);
            }
            else
            {
                WersjaComboBox.SelectedItem = Baza.FolderServ;
                PanelTxt.Text = PanelDlaSonetaSerwer(SonetaServerParam);
            }
        }

        internal static IEnumerable GetOperators()
        {
            List<string> lista = new List<string>();

            string conString = ConfigurationManager.ConnectionStrings[AktualnaBazaSQL]?.ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT name FROM operators WHERE locked = 0 And fullName NOT LIKE '%.NET%'", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return lista;
        }

        private void BazaComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ListaBazSQL = new Dictionary<string, string>();

            Dictionary<string, string> bazy = new Dictionary<string, string>();

            string listaBazDanych = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\Soneta\\Lista baz danych.xml";

            var dokument = XDocument.Load(listaBazDanych);
            IEnumerable<XElement> databaseList = dokument.Element("DatabaseCollection").Elements("MsSqlDatabase");

            foreach (var item in databaseList)
            {
                bazy.Add(item.Element("Name").Value, item.Element("DatabaseName").Value);
            }

            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;

            foreach (ConnectionStringSettings conn in connectionStrings)
            {
                if (conn.Name == "BazyEnova")
                    continue;

                bool dodaj = true;
                foreach (var item in bazy)
                {
                    if (item.Value == conn.Name)
                    {
                        ListaBazSQL.Add(conn.Name + " ( " + item.Key + " ) ", conn.Name);
                        dodaj = false;
                    }
                }

                if (dodaj)
                    ListaBazSQL.Add(conn.Name + " ( x ) ", conn.Name);
            }
            BazaComboBox.ItemsSource = ListaBazSQL.Keys;
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> sysInfoList = new List<string>();
            Dictionary<string, string> featureDict = new Dictionary<string, string>();

            if (AktualnaBazaSQL == null)
            {
                MessageBox.Show("Proszę wybrać bazę danych");
                return;
            }
            
            string conString = ConfigurationManager.ConnectionStrings[AktualnaBazaSQL]?.ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT value From SystemInfos", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            sysInfoList.Add(dr[0].ToString());
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand("SELECT Name, TableName, Code From FeatureDefs ORDER BY Name", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string code = $"{dr[2]}";

                            if (string.IsNullOrEmpty(code))
                                featureDict.Add($"{dr[0]} ({dr[1]})", $"{dr[2]}");
                            else
                                featureDict.Add($"{dr[0]} ({dr[1]}) --- code --->", $"{dr[2]}");
                        }
                    }
                }
            }

            Info info = new Info(AktualnaBazaSQL, sysInfoList, featureDict, conString);
            info.Show();
        }

        private void SerwerSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            ServerSQLConfig serverSQLConfig = new ServerSQLConfig();
            serverSQLConfig.Show();
        }

        private void ConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Baza == null)
            {
                MessageBox.Show("Nie wybrano bazy");
                return;
            }

            Config config = new Config(Baza);
            config.TextBoxValueChanged += ConfigWindowOnTextBoxValueChanged;
            config.Show();
        }

        private void ConfigWindowOnTextBoxValueChanged(object sender, TextBoxValueEventArgs e)
        {
            SonetaExplorerParam = e.SonetaExplorerParam;
            SonetaServerParam = e.SonetaServerParam;
            Baza = e.Baza;

            if ((bool)SonetaExplorerRadioBtn.IsChecked)
            {
                PanelTxt.Text = PanelDlaSonetaExplorer(e.SonetaExplorerParam);
            }
            else
            {
                PanelTxt.Text = PanelDlaSonetaSerwer(e.SonetaServerParam);
            }
        }

        private void PobierzBazyBtn_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            string conString;
            string[] listaInstancjiSQL = SqlHelper.ListLocalSqlInstances().ToArray();

            foreach (string sqlName in listaInstancjiSQL)
            {
                conString = $"Server={sqlName};Trusted_Connection=True;";

                List<string> list = new List<string>();
                List<string> listaBazTemp = new List<string>();

                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(dr[0].ToString());
                            }
                        }
                    }

                    foreach (string baza in list)
                    {
                        con.ChangeDatabase(baza);
                        using (SqlCommand cmd = new SqlCommand("SELECT TABLE_CATALOG FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SystemInfos'", con))
                        {
                            using (IDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    listaBazTemp.Add(dr[0].ToString());
                                }
                            }
                        }
                    }

                    foreach (string baza in listaBazTemp)
                    {
                        DbSetting.CreateConnectionString(sqlName, baza, null, null);

                        ConfigurationManager.RefreshSection("connectionStrings");
                    }
                }
            }

            Cursor = Cursors.Arrow;
        }

        private void WprowadzZmianyZPanelu()
        {
            if (Baza == null)
            {
                PanelTxt.Text = string.Empty;
                System.Windows.Forms.MessageBox.Show("Proszę wybrać bazę enovy");
                return;
            }

            string[] parametry = PanelTxt.Text.Split(new string[] { " /" }, StringSplitOptions.RemoveEmptyEntries);

            if (parametry.Length == 0)
                return;

            parametry[0] = parametry[0].Contains('/') ? parametry[0].Remove(0, 1) : parametry[0];

            bool znaleziono_FolderDodatkowApp = false;
            bool znaleziono_ListaBazDanychApp = false;
            bool znaleziono_KonfiguracjaApp = false;
            bool znaleziono_FolderUIApp = false;
            bool znaleziono_Operator = false;
            bool znaleziono_BezDodatkowApp = false;
            bool znaleziono_BezDLLSerweraApp = false;
            bool znaleziono_ListaBazDanychServ = false;
            bool znaleziono_FolderDodatkowServ = false;
            bool znaleziono_PortServ = false;
            bool znaleziono_BezHarmonogramuServ = false;
            bool znaleziono_BezDodatkowServ = false;
            bool znaleziono_BezDLLSerweraServ = false;

            for (int i = 0; i < parametry.Length; i++)
            {
                string parametr = parametry[i].Trim(new char[] { ' ', '"' });
                if ((bool)SonetaExplorerRadioBtn.IsChecked)
                {
                    if (parametr.Contains("extpath=") && !znaleziono_FolderDodatkowApp)
                    {
                        znaleziono_FolderDodatkowApp = true;
                        Baza.FolderDodatkowApp = parametr.Replace("extpath=", "").Trim('"');
                    }
                    else if (!znaleziono_FolderDodatkowApp)
                        Baza.FolderDodatkowApp = "";

                    if (parametr.Contains("dbconfig=") && !znaleziono_ListaBazDanychApp)
                    {
                        znaleziono_ListaBazDanychApp = true;
                        Baza.ListaBazDanychApp = parametr.Replace("dbconfig=", "").Trim('"');
                        continue;
                    }
                    else if (!znaleziono_ListaBazDanychApp)
                        Baza.ListaBazDanychApp = "";

                    if (parametr.Contains("config=") && !znaleziono_KonfiguracjaApp)
                    {
                        znaleziono_KonfiguracjaApp = true;
                        Baza.KonfiguracjaApp = parametr.Replace("config=", "").Trim('"');
                    }
                    else if (!znaleziono_KonfiguracjaApp)
                        Baza.KonfiguracjaApp = "";

                    if (parametr.Contains("folder=") && !znaleziono_FolderUIApp)
                    {
                        znaleziono_FolderUIApp = true;
                        Baza.FolderUIApp = parametr.Replace("folder=", "").Trim('"');
                    }
                    else if (!znaleziono_FolderUIApp)
                        Baza.FolderUIApp = "";

                    if (parametr.Contains("operator=") && !znaleziono_Operator)
                    {
                        znaleziono_Operator = true;
                        Baza.Operator = parametr.Replace("operator=", "").Trim('"');
                    }
                    else if (!znaleziono_Operator)
                        Baza.Operator = "";

                    if (parametr.Contains("ext=") && !znaleziono_BezDodatkowApp)
                    {
                        znaleziono_BezDodatkowApp = true;
                        Baza.BezDodatkowApp = true;
                    }
                    else if (!znaleziono_BezDodatkowApp)
                        Baza.BezDodatkowApp = false;

                    if (parametr.Contains("nodbextensions") && !znaleziono_BezDLLSerweraApp)
                    {
                        znaleziono_BezDLLSerweraApp = true;
                        Baza.BezDLLSerweraApp = true;
                    }
                    else if (!znaleziono_BezDLLSerweraApp)
                        Baza.BezDLLSerweraApp = false;
                }
                else
                {
                    if (parametr.Contains("dbconfig=") && !znaleziono_ListaBazDanychServ)
                    {
                        znaleziono_ListaBazDanychServ = true;
                        Baza.ListaBazDanychServ = parametr.Replace("dbconfig=", "").Trim('"');
                    }
                    else if (!znaleziono_ListaBazDanychServ)
                        Baza.ListaBazDanychServ = "";

                    if (parametr.Contains("extpath=") && !znaleziono_FolderDodatkowServ)
                    {
                        znaleziono_FolderDodatkowServ = true;
                        Baza.FolderDodatkowServ = parametr.Replace("extpath=", "").Trim('"');
                    }
                    else if (!znaleziono_FolderDodatkowServ)
                        Baza.FolderDodatkowServ = "";

                    if (parametr.Contains("port=") && !znaleziono_PortServ)
                    {
                        znaleziono_PortServ = true;
                        Baza.PortServ = parametr.Replace("port=", "").Trim('"');
                    }
                    else if (!znaleziono_PortServ)
                        Baza.PortServ = "";

                    if (parametr.Contains("noscheduler") && !znaleziono_BezHarmonogramuServ)
                    {
                        znaleziono_BezHarmonogramuServ = true;
                        Baza.BezHarmonogramuServ = true;
                    }
                    else if (!znaleziono_BezHarmonogramuServ)
                        Baza.BezHarmonogramuServ = false;

                    if (parametr.Contains("ext=") && !znaleziono_BezDodatkowServ)
                    {
                        znaleziono_BezDodatkowServ = true;
                        Baza.BezDodatkowServ = true;
                    }
                    else if (!znaleziono_BezDodatkowServ)
                        Baza.BezDodatkowServ = false;

                    if (parametr.Contains("nodbextensions") && !znaleziono_BezDLLSerweraServ)
                    {
                        znaleziono_BezDLLSerweraServ = true;
                        Baza.BezDLLSerweraServ = true;
                    }
                    else if (!znaleziono_BezDLLSerweraServ)
                        Baza.BezDLLSerweraServ = false;
                }
            }
        }

        private void PanelTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            WprowadzZmianyZPanelu();
        }

        private void WersjaComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if ((bool)SonetaExplorerRadioBtn.IsChecked)
            {
                DirectoryInfo dir = new DirectoryInfo($"{SonetaExplorerCatalog}");
                WersjaComboBox.ItemsSource = dir.GetDirectories().Select(x => x.Name).ToList();
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo($"{SonetaSerwerCatalog}");
                WersjaComboBox.ItemsSource = dir.GetDirectories().Select(x => x.Name).ToList();
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupInfo.IsOpen = true;
            PopupInfo.Visibility = Visibility.Visible;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            PopupInfo.IsOpen = false;
            PopupInfo.Visibility = Visibility.Hidden;
        }
    }
}