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

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for ServerSQLConfig.xaml
    /// </summary>
    public partial class ServerSQLConfig : Window
    {
        string WybranaBaza { get; set; }
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

        //private const string defaultMsSqlInstanceName = "MSSQLSERVER";

        public static string[] GetLocalSqlServerInstances()
        {
            return SqlHelper.ListLocalSqlInstances().ToArray();
            
            //return new ManagedComputer()
            //    .ServerInstances
            //    .Cast<ServerInstance>()
            //    .Select(instance => string.IsNullOrEmpty(instance.Name) || instance.Name == defaultMsSqlInstanceName ?
            //        instance.Parent.Name : System.IO.Path.Combine(instance.Parent.Name, instance.Name))
            //    .ToArray();
        }

        private void ListaSQLCmbBox_DropDownOpened(object sender, EventArgs e)
        {
            ListaSQLCmbBox.ItemsSource = GetLocalSqlServerInstances();
        }

        private void ListaSQLCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
            //string str = DbSetting.GetConnectionString("BazyEnova");

            string str = ConfigurationManager.ConnectionStrings["BazyEnova"]?.ConnectionString;

            if (str == null)
            {
                BazaChkBox.IsEnabled = true;
                BazaChkBox.IsChecked = false;
                MessageBox.Show($"Oznacz serwer na którym zostanie utworzona baza danych 'BazyEnova'.");
                return;
            } else
            {
                string[] serwer = str.Split(';');
                BazaChkBox.IsChecked = serwer[0] == $"Data Source={ListaSQLCmbBox.SelectedItem}";

                if (CheckIfDatabaseExist("BazyEnova", str))
                {
                    BazaChkBox.IsChecked = true;
                    BazaChkBox.IsEnabled = false;
                }
                else
                {
                    BazaChkBox.IsEnabled = true;
                    BazaChkBox.IsChecked = false;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //if (BazaChkBox.IsEnabled)
            //    BazaChkBox.IsEnabled = false;
            //else
            //    return;

            WybranaBaza = (string)ListaSQLCmbBox.SelectedItem;

            if (!string.IsNullOrEmpty(WybranaBaza))
            {
                DbSetting.CreateConnectionString(WybranaBaza, "BazyEnova", null, null);
            }

            //string connectionString = $"Server={selected};Database=BazyEnova;Trusted_Connection=True;";

            //DbSetting.AddOrUpdateAppSetting($"ConnectionStrings:BazyEnova", connectionString);
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
    }
}