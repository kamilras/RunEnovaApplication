using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunEnovaApplication.Extension
{
    public static class SqlHelper
    {
        public static IEnumerable<string> ListLocalSqlInstances()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    foreach (string item in ListLocalSqlInstances(hive))
                    {
                        yield return item;
                    }
                }

                using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    foreach (string item in ListLocalSqlInstances(hive))
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                foreach (string item in ListLocalSqlInstances(Registry.LocalMachine))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<string> ListLocalSqlInstances(RegistryKey hive)
        {
            const string keyName = @"Software\Microsoft\Microsoft SQL Server";
            const string valueName = "InstalledInstances";
            const string defaultName = "MSSQLSERVER";

            using (var key = hive.OpenSubKey(keyName, false))
            {
                if (key == null) return Enumerable.Empty<string>();

                var value = key.GetValue(valueName) as string[];
                if (value == null) return Enumerable.Empty<string>();

                for (int index = 0; index < value.Length; index++)
                {
                    if (string.Equals(value[index], defaultName, StringComparison.OrdinalIgnoreCase))
                    {
                        value[index] = Environment.MachineName;
                    }
                    else
                    {
                        value[index] = $@"{Environment.MachineName}\{value[index]}";
                    }
                }
                return value;
            }
        }

        public static List<T> ReadAll<T>(SqlDataReader reader, Func<SqlDataReader, T> readFunc)
        {
            List<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add(readFunc(reader));
            }
            return list;
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002588 File Offset: 0x00000788
        public static Tuple<string[], string[][]> ReadAll(SqlDataReader reader)
        {
            List<string[]> list = new List<string[]>();
            string[] item = SqlHelper.ReadHeader(reader);
            while (reader.Read())
            {
                string[] array = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string[] array2 = array;
                    int num = i;
                    object value = reader.GetValue(i);
                    array2[num] = ((value != null) ? value.ToString() : null);
                }
                list.Add(array);
            }
            return new Tuple<string[], string[][]>(item, list.ToArray());
        }

        public static List<T> ReadAll<T>(SqlConnection conn, string sql, Func<SqlDataReader, T> readFunc, params SqlParameter[] parameters)
        {
            List<T> result;
            using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
            {
                bool flag = parameters != null;
                if (flag)
                {
                    foreach (SqlParameter value in parameters)
                    {
                        sqlCommand.Parameters.Add(value);
                    }
                }
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    result = SqlHelper.ReadAll<T>(sqlDataReader, readFunc);
                }
            }
            return result;
        }

        public static T ReadSingle<T>(SqlConnection conn, string sql, Func<SqlDataReader, T> readFunc, T defaultValue = default(T), params SqlParameter[] parameters)
        {
            T result;
            using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
            {
                bool flag = parameters != null;
                if (flag)
                {
                    foreach (SqlParameter value in parameters)
                    {
                        sqlCommand.Parameters.Add(value);
                    }
                }
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    result = SqlHelper.ReadSingle<T>(sqlDataReader, readFunc, defaultValue);
                }
            }
            return result;
        }

        public static T ReadSingle<T>(SqlDataReader reader, Func<SqlDataReader, T> readFunc, T defaultValue = default(T))
        {
            T result;
            if (!reader.Read())
            {
                result = defaultValue;
            }
            else
            {
                result = readFunc(reader);
            }
            return result;
        }

        public static string[] ReadHeader(SqlDataReader reader)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                list.Add(reader.GetName(i));
            }
            return list.ToArray();
        }

        public static int ExecuteNonQuery(SqlConnection conn, string sql, params SqlParameter[] parameters)
        {
            int result;
            using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
            {
                bool flag = parameters != null;
                if (flag)
                {
                    foreach (SqlParameter value in parameters)
                    {
                        sqlCommand.Parameters.Add(value);
                    }
                }
                result = sqlCommand.ExecuteNonQuery();
            }
            return result;
        }
    }
}
