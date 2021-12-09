using Microsoft.Data.SqlClient;
using RunEnova.Extension;
using RunEnova.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RunEnova
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Config : Window
    {
        public event EventHandler<TextBoxValueEventArgs> TextBoxValueChanged;
        public Config()
        {
            InitializeComponent();
        }

        public Config(Baza baza)
        {
            InitializeComponent();
            Baza = baza;
            WprowadzUstawienia();
        }

        public Baza Baza { get; set; }

        private void WprowadzUstawienia()
        {
            AplikacjaDLLFolderTxt.Text = MainWindow.Config.Baza.ApplicationConfig.FolderDodatkowApp;
            ListaBazDanychAplikacjaTxt.Text = MainWindow.Config.Baza.ApplicationConfig.ListaBazDanychApp;
            KonfiguracjaAplikacjaTxt.Text = MainWindow.Config.Baza.ApplicationConfig.KonfiguracjaApp;
            FolderUIAplikacjaTxt.Text = MainWindow.Config.Baza.ApplicationConfig.FolderUIApp;
            ListaBazDanychSerwerTxt.Text = MainWindow.Config.Baza.ServerConfig.ListaBazDanychServ;
            SerwerDLLFolderTxt.Text = MainWindow.Config.Baza.ServerConfig.FolderDodatkowServ;
            BezHarmonogramuChkBox.IsChecked = MainWindow.Config.Baza.ServerConfig.BezHarmonogramuServ;
            BezDodatkowAppChkBox.IsChecked = MainWindow.Config.Baza.ApplicationConfig.BezDodatkowApp;
            BezDodatkowSerwChkBox.IsChecked = MainWindow.Config.Baza.ServerConfig.BezDodatkowServ;
            BezDLLSerweraAppChkBox.IsChecked = MainWindow.Config.Baza.ApplicationConfig.BezDLLSerweraApp;
            BezDLLSerweraSerwChkBox.IsChecked = MainWindow.Config.Baza.ServerConfig.BezDLLSerweraServ;
            PortTxt.Text = MainWindow.Config.Baza.ServerConfig.PortServ;
            OperatorBtn.SelectedItem = MainWindow.Config.Baza.Operator;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Config.Baza.ApplicationConfig.FolderDodatkowApp = AplikacjaDLLFolderTxt.Text;
            MainWindow.Config.Baza.ApplicationConfig.ListaBazDanychApp = ListaBazDanychAplikacjaTxt.Text;
            MainWindow.Config.Baza.ApplicationConfig.KonfiguracjaApp = KonfiguracjaAplikacjaTxt.Text;
            MainWindow.Config.Baza.ApplicationConfig.FolderUIApp = FolderUIAplikacjaTxt.Text;
            MainWindow.Config.Baza.ServerConfig.ListaBazDanychServ = ListaBazDanychSerwerTxt.Text;
            MainWindow.Config.Baza.ServerConfig.FolderDodatkowServ = SerwerDLLFolderTxt.Text;
            MainWindow.Config.Baza.ServerConfig.BezHarmonogramuServ = (bool)BezHarmonogramuChkBox.IsChecked;
            MainWindow.Config.Baza.ApplicationConfig.BezDodatkowApp = (bool)BezDodatkowAppChkBox.IsChecked;
            MainWindow.Config.Baza.ServerConfig.BezDodatkowServ = (bool)BezDodatkowSerwChkBox.IsChecked;
            MainWindow.Config.Baza.ApplicationConfig.BezDLLSerweraApp = (bool)BezDLLSerweraAppChkBox.IsChecked;
            MainWindow.Config.Baza.ServerConfig.BezDLLSerweraServ = (bool)BezDLLSerweraSerwChkBox.IsChecked;
            MainWindow.Config.Baza.ServerConfig.PortServ = PortTxt.Text;
            MainWindow.Config.Baza.Operator = OperatorBtn.Text;

            string sonetaExplorerParam = "";
            string sonetaServerParam = "";

            ShowOnPanel(MainWindow.Config.Baza, out sonetaExplorerParam, out sonetaServerParam);
            OnTextBoxValueChanged(new TextBoxValueEventArgs(sonetaExplorerParam, sonetaServerParam));

            this.Close();
        }

        public static void ShowOnPanel(Baza baza, out string sonetaExplorerParam, out string sonetaServerParam)
        {
            StringBuilder sonetaExplorerBuilder = new StringBuilder();
            StringBuilder sonetaServerBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(baza.ApplicationConfig.FolderDodatkowApp))
                sonetaExplorerBuilder.Append($"/extpath=\"{baza.ApplicationConfig.FolderDodatkowApp}\" ");

            if (!string.IsNullOrEmpty(baza.ApplicationConfig.ListaBazDanychApp))
                sonetaExplorerBuilder.Append($"/dbconfig=\"{baza.ApplicationConfig.ListaBazDanychApp}\" ");

            if (!string.IsNullOrEmpty(baza.ApplicationConfig.KonfiguracjaApp))
                sonetaExplorerBuilder.Append($"/config=\"{baza.ApplicationConfig.KonfiguracjaApp}\" ");

            if (!string.IsNullOrEmpty(baza.ApplicationConfig.FolderUIApp))
                sonetaExplorerBuilder.Append($"/folder=\"{baza.ApplicationConfig.FolderUIApp}\" ");

            if (!string.IsNullOrEmpty(baza.ServerConfig.ListaBazDanychServ))
                sonetaServerBuilder.Append($"/dbconfig=\"{baza.ServerConfig.ListaBazDanychServ}\" ");

            if (!string.IsNullOrEmpty(baza.ServerConfig.FolderDodatkowServ))
                sonetaServerBuilder.Append($"/extpath=\"{baza.ServerConfig.FolderDodatkowServ}\" ");

            if (!string.IsNullOrEmpty(baza.ServerConfig.PortServ))
                sonetaServerBuilder.Append($"/port=\"{baza.ServerConfig.PortServ}\" ");

            if (!string.IsNullOrEmpty(baza.Operator))
                sonetaExplorerBuilder.Append($"/operator=\"{baza.Operator}\" ");

            if (baza.ServerConfig.BezHarmonogramuServ)
                sonetaServerBuilder.Append($"/noscheduler "); 
            
            if (baza.ApplicationConfig.BezDodatkowApp)
                sonetaExplorerBuilder.Append($"/ext= ");

            if (baza.ServerConfig.BezDodatkowServ)
                sonetaServerBuilder.Append($"/ext= "); 

            if (baza.ApplicationConfig.BezDLLSerweraApp)
                sonetaExplorerBuilder.Append($"/nodbextensions "); 

            if (baza.ServerConfig.BezDLLSerweraServ)
                sonetaServerBuilder.Append($"/nodbextensions ");

            sonetaExplorerParam = sonetaExplorerBuilder.ToString();
            sonetaServerParam = sonetaServerBuilder.ToString();
        }

        protected virtual void OnTextBoxValueChanged(TextBoxValueEventArgs e)
        {
            TextBoxValueChanged?.Invoke(this, e);
        }

        private void OperatorBtn_DropDownOpened(object sender, EventArgs e)
        {
            OperatorBtn.ItemsSource = MainWindow.GetOperators();
        }
    }

    public class TextBoxValueEventArgs : EventArgs
    {
        public string SonetaExplorerParam { get; set; }
        public string SonetaServerParam { get; set; }

        public TextBoxValueEventArgs(string sonetaExplorerParam, string sonetaServerParam)
        {
            SonetaExplorerParam = sonetaExplorerParam;
            SonetaServerParam = sonetaServerParam;
        }
    }
}
