using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RunEnova.Extension;
using RunEnova.Model;
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
        public static Config Config { get; set; }
        public Baza Baza { get; set; }
        public Dictionary<string, string> ListaBazSQL { get; set; }
        public Dictionary<string, string> ListaBazEnova { get; set; }
        public static string AktualnaBazaEnova { get; set; }
        public static string AktualnaBazaSQL { get; set; }
        public string SonetaExplorerParam = "";
        public string SonetaServerParam = "";
        private bool ConfigChanged { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Load(string nazwa_bazy = null)
        {
            Config = Config ?? new Config();
            Config.Baza = Config.Baza ?? new Baza() { NazwaBazy = nazwa_bazy };
        }

        private void LoadContext(string database_name)
        {
            //string connectionString = DbSetting.GetConnectionString(database_name);
            string connectionString = ConfigurationManager.ConnectionStrings[database_name]?.ConnectionString;
            if (connectionString == null)
            {
                MessageBox.Show("Nie znaleziono wpisu w App.config dla podanej bazy");
                return;
            }
            Context = new BazaDb();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            if ((bool)SonetaExplorerRadioBtn?.IsChecked)
            {
                startInfo.FileName = $"C:\\Program Files (x86)\\Soneta\\{WersjaComboBox.Text}\\SonetaExplorer.exe";
            }
            else
            {
                startInfo.FileName = $"C:\\Multi\\{WersjaComboBox.Text}\\SonetaServer.exe";
            }
            startInfo.Arguments = PanelTxt.Text;

            try
            {
                //using (Process exeProcess = Process.Start(startInfo))
                //{
                //    exeProcess.WaitForExit();
                //}
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void WersjaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem == null)
                return;

            if ((bool)SonetaServerRadioBtn?.IsChecked)
                Config.Baza.FolderServ = ((ComboBox)sender).SelectedItem?.ToString();
            else
                Config.Baza.FolderApp = ((ComboBox)sender).SelectedItem?.ToString();
        }
        private void SonetaExplorerRadioBtn_Checked_1(object sender, RoutedEventArgs e)
        {
            if (WersjaComboBox == null)
                return;
            DirectoryInfo di = new DirectoryInfo($"C:\\Program Files (x86)\\Soneta");
            WersjaComboBox.ItemsSource = di.GetDirectories().Select(x => x.Name).ToList();
            if (Config?.Baza != null)
                WersjaComboBox.SelectedItem = Config.Baza.FolderApp;

            PanelTxt.Text = PanelDlaSonetaExplorer(SonetaExplorerParam);
        }

        private string PanelDlaSonetaExplorer(string sonetaExplorerParam)
        {
            if (string.IsNullOrEmpty(sonetaExplorerParam))
                return string.Empty;

            if (!string.IsNullOrEmpty(AktualnaBazaEnova))
                return $"/database={AktualnaBazaEnova} {sonetaExplorerParam}";
            return sonetaExplorerParam;
        }

        private string PanelDlaSonetaSerwer(string sonetaServerParam)
        {
            if (string.IsNullOrEmpty(sonetaServerParam))
                return string.Empty;

            if (!string.IsNullOrEmpty(AktualnaBazaEnova))
                return $"/console /database={AktualnaBazaEnova} {sonetaServerParam}";
            return "/console " + sonetaServerParam;
        }

        private void SonetaServerRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo($"C:\\Multi");
            WersjaComboBox.ItemsSource = dir.GetDirectories().Select(x => x.Name).ToList();
            if (Config?.Baza != null)
                WersjaComboBox.SelectedItem = Config.Baza.FolderServ;

            PanelTxt.Text = PanelDlaSonetaSerwer(SonetaServerParam);
        }

        private void ZapiszBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Context == null || Context.Baza == null)
            {
                MessageBox.Show("Nie wybrano żadnej bazy danych");
                return;
            }

            if (Context.Baza.FirstOrDefault(x => x.Id == Config.Baza.Id) == null)
                Context.Baza.Add(Config.Baza);
            //else
            //Context.Baza.Update(Config.Baza);
            Context.SaveChanges();

            MessageBox.Show("Zapisano ustawienia!");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string nazwa_bazy = ((ComboBox)sender).SelectedItem.ToString();

            if (ListaBazEnova.ContainsKey(nazwa_bazy))
                AktualnaBazaEnova = ListaBazEnova[nazwa_bazy];
            AktualnaBazaSQL = ListaBazSQL[nazwa_bazy];

            Load(AktualnaBazaSQL);
            LoadContext("BazyEnova");
            Baza c = Context?.Baza?.FirstOrDefault(x => x.NazwaBazy == AktualnaBazaSQL);
            if (c != null)
                Config.Baza = c;
            else
                Config.Baza = new Baza() { NazwaBazy = AktualnaBazaSQL };

            OdswiezUstawienia();
        }

        private void OdswiezUstawienia()
        {
            Config.ShowOnPanel(Config.Baza, out SonetaExplorerParam, out SonetaServerParam);

            if ((bool)SonetaExplorerRadioBtn?.IsChecked)
            {
                WersjaComboBox.SelectedItem = Config.Baza.FolderApp;
                PanelTxt.Text = PanelDlaSonetaExplorer(SonetaExplorerParam);
            }
            else
            {
                WersjaComboBox.SelectedItem = Config.Baza.FolderServ;
                PanelTxt.Text = PanelDlaSonetaSerwer(SonetaServerParam);
            }
        }

        internal static IEnumerable GetOperators()
        {
            List<string> lista = new List<string>();

            //string conString = DbSetting.GetConnectionString(AktualnaBazaSQL);
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
            ListaBazEnova = new Dictionary<string, string>();

            List<string> nazwyBaz = new List<string>();
            Dictionary<string, string> bazy = new Dictionary<string, string>();

            string listaBazDanych = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\Soneta\\Lista baz danych.xml";

            var dokument = XDocument.Load(listaBazDanych);
            IEnumerable<XElement> databaseList = dokument.Element("DatabaseCollection").Elements("MsSqlDatabase");
            //IEnumerable<XElement> dbTupleDefinitionXml = sessionXml.Element("MsSqlDatabase").Elements();

            foreach (var item in databaseList)
            {
                bazy.Add(item.Element("Name").Value, item.Element("DatabaseName").Value);
            }

            //var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");
            //string json = File.ReadAllText(filePath);
            //JObject jsonObj = JsonConvert.DeserializeObject<JObject>(json);
            //JToken jtoken = jsonObj.Value<JToken>();

            //var t = jtoken.Children().Values();

            //foreach (var item in t.Values())
            //{
            //    nazwyBaz.Add(item.ToString().Split('"').ElementAt(1));
            //}

            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;

            foreach (ConnectionStringSettings conn in connectionStrings)
            {
                nazwyBaz.Add(conn.Name);
            }

            for (int i = 0; i < nazwyBaz.Count; i++)
            {
                if (nazwyBaz[i] == "BazyEnova")
                    continue;

                bool dodaj = true;
                foreach (var item in bazy)
                {
                    if (item.Value == nazwyBaz[i])
                    {
                        ListaBazSQL.Add(nazwyBaz[i] + " ( " + item.Key + " ) ", nazwyBaz[i]);
                        ListaBazEnova.Add(nazwyBaz[i] + " ( " + item.Key + " ) ", item.Key);
                        dodaj = false;
                    }
                }

                if (dodaj)
                    ListaBazSQL.Add(nazwyBaz[i] + " ( x ) ", nazwyBaz[i]);
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


            //string conString = DbSetting.GetConnectionString(AktualnaBazaSQL);
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

            Info info = new Info(AktualnaBazaSQL, sysInfoList, featureDict);
            //info.SizeToContent = SizeToContent.WidthAndHeight;
            info.Show();
        }

        private void SerwerSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            ServerSQLConfig serverSQLConfig = new ServerSQLConfig();
            serverSQLConfig.Show();
        }

        private void ConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Config == null)
            {
                MessageBox.Show("Nie wybrano bazy");
                return;
            }

            Config config = new Config(Config.Baza);
            config.TextBoxValueChanged += ConfigWindowOnTextBoxValueChanged;
            config.Show();
        }

        private void ConfigWindowOnTextBoxValueChanged(object sender, TextBoxValueEventArgs e)
        {
            SonetaExplorerParam = e.SonetaExplorerParam;
            SonetaServerParam = e.SonetaServerParam;

            ConfigChanged = true;

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
            //if (ListaBaz != null && ListaBaz.Count > 0)
            //{
            //    BazaComboBox.ItemsSource = ListaBaz;
            //    return;
            //}

            Cursor = Cursors.Wait;

            //List<string> list = new List<string>();
            //ListaBaz = new List<string>();

            string conString;
            string[] listaInstancjiSQL = ServerSQLConfig.GetLocalSqlServerInstances();

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

                        var conect = ConfigurationManager.ConnectionStrings;
                    }
                }
            }

            Cursor = Cursors.Arrow;
        }

        private void PanelTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConfigChanged)
            {
                ConfigChanged = false;
                return;
            }

            if (Config?.Baza == null)
            {
                PanelTxt.Text = string.Empty;
                System.Windows.Forms.MessageBox.Show("Proszę wybrać bazę enovy");
                return;
            }

            TextBox textBox = (TextBox)sender;

            string[] parametry = textBox.Text.Split(new string[] { " /" }, StringSplitOptions.RemoveEmptyEntries);

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
                    if (parametry[i].Contains("extpath=") && !znaleziono_FolderDodatkowApp)
                    {
                        znaleziono_FolderDodatkowApp = true;
                        Config.Baza.FolderDodatkowApp = parametr.Replace("extpath=", "").Trim('"');
                    }
                    else if (!znaleziono_FolderDodatkowApp)
                        Config.Baza.FolderDodatkowApp = "";

                    if (parametry[i].Contains("dbconfig=") && !znaleziono_ListaBazDanychApp)
                    {
                        znaleziono_ListaBazDanychApp = true;
                        Config.Baza.ListaBazDanychApp = parametr.Replace("dbconfig=", "").Trim('"');
                        continue;
                    }
                    else if (!znaleziono_ListaBazDanychApp)
                        Config.Baza.ListaBazDanychApp = "";

                    if (parametry[i].Contains("config=") && !znaleziono_KonfiguracjaApp)
                    {
                        znaleziono_KonfiguracjaApp = true;
                        Config.Baza.KonfiguracjaApp = parametr.Replace("config=", "").Trim('"');
                    }
                    else if (!znaleziono_KonfiguracjaApp)
                        Config.Baza.KonfiguracjaApp = "";

                    if (parametry[i].Contains("folder=") && !znaleziono_FolderUIApp)
                    {
                        znaleziono_FolderUIApp = true;
                        Config.Baza.FolderUIApp = parametr.Replace("folder=", "").Trim('"');
                    }
                    else if (!znaleziono_FolderUIApp)
                        Config.Baza.FolderUIApp = "";

                    if (parametry[i].Contains("operator=") && !znaleziono_Operator)
                    {
                        znaleziono_Operator = true;
                        Config.Baza.Operator = parametr.Replace("operator=", "").Trim('"');
                    }
                    else if (!znaleziono_Operator)
                        Config.Baza.Operator = "";

                    if (parametry[i].Contains("ext=") && !znaleziono_BezDodatkowApp)
                    {
                        znaleziono_BezDodatkowApp = true;
                        Config.Baza.BezDodatkowApp = true;
                    }
                    else if (!znaleziono_BezDodatkowApp)
                        Config.Baza.BezDodatkowApp = false;

                    if (parametry[i].Contains("nodbextensions") && !znaleziono_BezDLLSerweraApp)
                    {
                        znaleziono_BezDLLSerweraApp = true;
                        Config.Baza.BezDLLSerweraApp = true;
                    }
                    else if (!znaleziono_BezDLLSerweraApp)
                        Config.Baza.BezDLLSerweraApp = false;
                }
                else
                {
                    if (znaleziono_ListaBazDanychServ)
                    {
                        znaleziono_ListaBazDanychServ = true;
                        Config.Baza.ListaBazDanychServ = parametr.Replace("dbconfig=", "").Trim('"');
                    }
                    else if (!znaleziono_ListaBazDanychServ)
                        Config.Baza.ListaBazDanychServ = "";

                    if (znaleziono_FolderDodatkowServ)
                    {
                        znaleziono_FolderDodatkowServ = true;
                        Config.Baza.FolderDodatkowServ = parametr.Replace("extpath=", "").Trim('"');
                    }
                    else if (znaleziono_FolderDodatkowServ)
                        Config.Baza.FolderDodatkowServ = "";

                    if (znaleziono_PortServ)
                    {
                        znaleziono_PortServ = true;
                        Config.Baza.PortServ = parametr.Replace("port=", "").Trim('"');
                    }
                    else if (!znaleziono_PortServ)
                        Config.Baza.PortServ = "";

                    if (znaleziono_BezHarmonogramuServ)
                    {
                        znaleziono_BezHarmonogramuServ = true;
                        Config.Baza.BezHarmonogramuServ = true;
                    }
                    else if (!znaleziono_BezHarmonogramuServ)
                        Config.Baza.BezHarmonogramuServ = false;

                    if (znaleziono_BezDodatkowServ)
                    {
                        znaleziono_BezDodatkowServ = true;
                        Config.Baza.BezDodatkowServ = true;
                    }
                    else if (!znaleziono_BezDodatkowServ)
                        Config.Baza.BezDodatkowServ = false;

                    if (znaleziono_BezDLLSerweraServ)
                    {
                        znaleziono_BezDLLSerweraServ = true;
                        Config.Baza.BezDLLSerweraServ = true;
                    }
                    else if (!znaleziono_BezDLLSerweraServ)
                        Config.Baza.BezDLLSerweraServ = false;
                }
            }
        }
    }
}
