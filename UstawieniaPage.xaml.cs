using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for UstawieniaPage.xaml
    /// </summary>
    public partial class UstawieniaPage : Page
    {
        public UstawieniaPage()
        {
            InitializeComponent();
        }

        private void OperatorComBox_DropDownOpened(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Brak implementacji");
        }

        private void OperatorComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ResetPassBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Brak implementacji");
        }

        private void DemoBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Brak implementacji");
        }
    }
}
