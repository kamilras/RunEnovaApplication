﻿using Microsoft.Extensions.Configuration;
using RunEnova.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RunEnova.Extension
{
    public static class DbSetting
    {
        public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        {
            try
            {
                var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                SetValueRecursively(sectionPathKey, jsonObj, value);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing app settings | {0}", ex.Message);
            }
        }

        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            var remainingSections = sectionPathKey.Split(':');

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                var nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = value;
            }
        }

        public static string GetConnectionString(string database_name)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                                .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetConnectionString(database_name);
        }

        public static DbContextOptions<BazaDb> LoadConfiguration(string connectionString)
        {
            DbContextOptionsBuilder<BazaDb> optionsBuilder;

            optionsBuilder = new DbContextOptionsBuilder<BazaDb>();
            var options = optionsBuilder
                    .UseSqlServer(connectionString)
                    .Options;

            return options;
        }
    }
}
