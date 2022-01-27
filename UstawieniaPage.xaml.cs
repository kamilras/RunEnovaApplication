using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RunEnovaApplication.Extension;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for UstawieniaPage.xaml
    /// </summary>
    public partial class UstawieniaPage : Page
    {
        public UstawieniaPage(string connectionString)
        {
            ConnectionString = connectionString;
            InitializeComponent();
            WczytajListeOperatorow();
        }

        private void WczytajListeOperatorow()
        {
            Cursor = Cursors.Wait;

            ListaOperatorow = new List<Operator>();
            try
            {
                List<string> lista = new List<string>();
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    ListaOperatorow = SqlHelper.ReadAll<Operator>(sqlConnection, "select Guid, Name from dbo.Operators", delegate (SqlDataReader x) {
                        Operator @operator = new Operator();
                        @operator.Guid = x.GetGuid(0);
                        @operator.Name = x.GetString(1);
                        return @operator;
                    }, Array.Empty<SqlParameter>()).ToList();
                    //OperatorComBox.Items.Clear();
                    lista = ListaOperatorow.Select(x => x.Name).ToList<string>();
                    OperatorComBox.ItemsSource = lista;
                    //var items = OperatorComBox.Items;
                    //object[] items2 = array;
                    //items.AddRange(items2);
                    OperatorComBox.SelectedItem = lista.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string str = "Błąd wczytywania listy operatorów: ";
                Exception ex2 = ex;
                System.Windows.Forms.MessageBox.Show(str + ((ex2 != null) ? ex2.ToString() : null));
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        public string ConnectionString { get; set; }
        public Operator Operator { get; set; }
        public List<Operator> ListaOperatorow { get; set; }

        private void OperatorComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string oper = (string)OperatorComBox.SelectedItem;
            Operator = ListaOperatorow.FirstOrDefault(x => x.Name == oper);
        }

        private void ResetPassBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    string g = SqlHelper.ReadSingle<string>(sqlConnection, "Select Value from SystemInfos where id=1", (SqlDataReader x) => x.GetString(0), null, Array.Empty<SqlParameter>());
                    //Operator @operator = this.t_operator.SelectedItem as MainForm.Operator;
                    if (Operator == null)
                    {
                        throw new ApplicationException("Nie wybrano operatora");
                    }
                    Operator operator2 = Operator;
                    string passwordHash = OperatorExt.GetPasswordHash(new Guid(g), OperatorPassTxtBox.Text, operator2.Guid);
                    SqlHelper.ExecuteNonQuery(sqlConnection, "update Operators set Password=@hash where Guid=@guid", new SqlParameter[]
                    {
                            new SqlParameter("@hash", passwordHash),
                            new SqlParameter("@guid", operator2.Guid)
                    });
                    MessageBox.Show(string.Concat(new string[]
                    {
                            "Zmieniono hasło operatora [",
                            operator2.Name,
                            "]"
                    }));
                }
            }
            catch (Exception ex)
            {
                string str = "Błąd ustawiania hasła: ";
                Exception ex2 = ex;
                System.Windows.Forms.MessageBox.Show(str + ((ex2 != null) ? ex2.ToString() : null));
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void DemoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    SqlHelper.ExecuteNonQuery(sqlConnection, "update SystemInfos set Value = '00000003' where Ident = 10", Array.Empty<SqlParameter>());
                }
                MessageBox.Show("Ustawiono licencję DEMO");
            }
            catch (Exception ex)
            {
                string str = "Błąd ustawiania licencji DEMO: ";
                Exception ex2 = ex;
                MessageBox.Show(str + ((ex2 != null) ? ex2.ToString() : null));
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }
    }

    public class Operator
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}
