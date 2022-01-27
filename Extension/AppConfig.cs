using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace RunEnova.Extension
{
    public static class AppConfig
    {
        public static void AddConnectionString(string nazwa, string server, string baza, string userId, string password)
        {
            try
            {
                //Integrated security will be off if either UserID or Password is supplied
                var integratedSecurity = string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password);

                SqlConnectionStringBuilder connectionBuilder;

                //Create the connection string using the connection builder
                if (integratedSecurity)
                {
                    connectionBuilder = new SqlConnectionStringBuilder {
                        DataSource = server,
                        InitialCatalog = baza,
                        IntegratedSecurity = true
                    };
                }
                else
                {
                    connectionBuilder = new SqlConnectionStringBuilder {
                        DataSource = server,
                        InitialCatalog = baza,
                        UserID = userId,
                        Password = password,
                        IntegratedSecurity = false
                    };
                }

                //Open the app.config for modification
                //string file = System.Windows.Forms.Application.ExecutablePath;
                //Configuration sysconfig = ConfigurationManager.OpenExeConfiguration(file);

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //Retreive connection string setting
                var connectionString = config.ConnectionStrings.ConnectionStrings[nazwa];
                if (connectionString == null)
                {
                    //Create connection string if it doesn't exist
                    config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings {
                        Name = nazwa,
                        ConnectionString = connectionBuilder.ConnectionString,
                        ProviderName = "System.Data.SqlClient" //Depends on the provider, this is for SQL Server
                    });
                }
                else
                {
                    //Only modify the connection string if it does exist
                    connectionString.ConnectionString = connectionBuilder.ConnectionString;
                }

                //Save changes in the app.config
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania ustawień: " + Environment.NewLine + ex.Message);
            }
        }

        public static void ChangeConnectionString(string nazwa, ConnectionStringSettings connectionStringSettings)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                var connectionString = config.ConnectionStrings.ConnectionStrings[nazwa];
                if (connectionString == null)
                    config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
                else
                    connectionString.ConnectionString = connectionStringSettings.ConnectionString;

                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania ustawień: " + Environment.NewLine + ex.Message);
            }
        }
    }
}
