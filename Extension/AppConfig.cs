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
                ConfigurationManager.RefreshSection("connectionStrings");
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
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania ustawień: " + Environment.NewLine + ex.Message);
            }
        }
        //public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        //{
        //    try
        //    {
        //        var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");
        //        string json = File.ReadAllText(filePath);
        //        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        //        SetValueRecursively(sectionPathKey, jsonObj, value);

        //        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
        //        File.WriteAllText(filePath, output);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error writing app settings | {0}", ex.Message);
        //    }
        //}

        //private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        //{
        //    // split the string at the first ':' character
        //    var remainingSections = sectionPathKey.Split(':');

        //    var currentSection = remainingSections[0];
        //    if (remainingSections.Length > 1)
        //    {
        //        // continue with the procress, moving down the tree
        //        var nextSection = remainingSections[1];
        //        SetValueRecursively(nextSection, jsonObj[currentSection], value);
        //    }
        //    else
        //    {
        //        // we've got to the end of the tree, set the value
        //        jsonObj[currentSection] = value;
        //    }
        //}

        //public static string GetConnectionString(string database_name)
        //{
        //    var builder = new ConfigurationBuilder()
        //                        .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
        //                        .AddJsonFile("appsettings.json");

        //    var config = builder.Build();
        //    return config.GetConnectionString(database_name);
        //}

        //public static DbContextOptions<BazaDb> LoadConfiguration(string connectionString)
        //{
        //    DbContextOptionsBuilder<BazaDb> optionsBuilder;

        //    optionsBuilder = new DbContextOptionsBuilder<BazaDb>();
        //    var options = optionsBuilder
        //            .UseSqlServer(connectionString)
        //            .Options;

        //    return options;
        //}
    }
}
