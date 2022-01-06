using Microsoft.Data.SqlClient;
using RunEnova.Extension;
using RunEnova.Model;
using RunEnovaApplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows;

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
            OperatorBtn.ItemsSource = MainWindow.GetOperators();

            AplikacjaDLLFolderTxt.Text = Baza.FolderDodatkowApp;
            ListaBazDanychAplikacjaTxt.Text = Baza.ListaBazDanychApp;
            KonfiguracjaAplikacjaTxt.Text = Baza.KonfiguracjaApp;
            FolderUIAplikacjaTxt.Text = Baza.FolderUIApp;
            ListaBazDanychSerwerTxt.Text = Baza.ListaBazDanychServ;
            SerwerDLLFolderTxt.Text = Baza.FolderDodatkowServ;
            BezHarmonogramuChkBox.IsChecked = Baza.BezHarmonogramuServ;
            BezDodatkowAppChkBox.IsChecked = Baza.BezDodatkowApp;
            BezDodatkowSerwChkBox.IsChecked = Baza.BezDodatkowServ;
            BezDLLSerweraAppChkBox.IsChecked = Baza.BezDLLSerweraApp;
            BezDLLSerweraSerwChkBox.IsChecked = Baza.BezDLLSerweraServ;
            PortTxt.Text = Baza.PortServ;
            OperatorBtn.SelectedItem = Baza.Operator;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Baza.FolderDodatkowApp = AplikacjaDLLFolderTxt.Text;
            Baza.ListaBazDanychApp = ListaBazDanychAplikacjaTxt.Text;
            Baza.KonfiguracjaApp = KonfiguracjaAplikacjaTxt.Text;
            Baza.FolderUIApp = FolderUIAplikacjaTxt.Text;
            Baza.ListaBazDanychServ = ListaBazDanychSerwerTxt.Text;
            Baza.FolderDodatkowServ = SerwerDLLFolderTxt.Text;
            Baza.BezHarmonogramuServ = (bool)BezHarmonogramuChkBox.IsChecked;
            Baza.BezDodatkowApp = (bool)BezDodatkowAppChkBox.IsChecked;
            Baza.BezDodatkowServ = (bool)BezDodatkowSerwChkBox.IsChecked;
            Baza.BezDLLSerweraApp = (bool)BezDLLSerweraAppChkBox.IsChecked;
            Baza.BezDLLSerweraServ = (bool)BezDLLSerweraSerwChkBox.IsChecked;
            Baza.PortServ = PortTxt.Text;
            Baza.Operator = OperatorBtn.Text;

            string sonetaExplorerParam = "";
            string sonetaServerParam = "";

            ShowOnPanel(Baza, out sonetaExplorerParam, out sonetaServerParam);
            OnTextBoxValueChanged(new TextBoxValueEventArgs(Baza, sonetaExplorerParam, sonetaServerParam));

            this.Close();
        }

        public static void ShowOnPanel(Baza baza, out string sonetaExplorerParam, out string sonetaServerParam)
        {
            StringBuilder sonetaExplorerBuilder = new StringBuilder();
            StringBuilder sonetaServerBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(baza.FolderDodatkowApp))
                sonetaExplorerBuilder.Append($"/extpath=\"{baza.FolderDodatkowApp}\" ");

            if (!string.IsNullOrEmpty(baza.ListaBazDanychApp))
                sonetaExplorerBuilder.Append($"/dbconfig=\"{baza.ListaBazDanychApp}\" ");

            if (!string.IsNullOrEmpty(baza.KonfiguracjaApp))
                sonetaExplorerBuilder.Append($"/config=\"{baza.KonfiguracjaApp}\" ");

            if (!string.IsNullOrEmpty(baza.FolderUIApp))
                sonetaExplorerBuilder.Append($"/folder=\"{baza.FolderUIApp}\" ");

            if (!string.IsNullOrEmpty(baza.ListaBazDanychServ))
                sonetaServerBuilder.Append($"/dbconfig=\"{baza.ListaBazDanychServ}\" ");

            if (!string.IsNullOrEmpty(baza.FolderDodatkowServ))
                sonetaServerBuilder.Append($"/extpath=\"{baza.FolderDodatkowServ}\" ");

            if (!string.IsNullOrEmpty(baza.PortServ))
                sonetaServerBuilder.Append($"/port=\"{baza.PortServ}\" ");

            if (!string.IsNullOrEmpty(baza.Operator))
                sonetaExplorerBuilder.Append($"/operator=\"{baza.Operator}\" ");

            if (baza.BezHarmonogramuServ)
                sonetaServerBuilder.Append($"/noscheduler ");

            if (baza.BezDodatkowApp)
                sonetaExplorerBuilder.Append($"/ext= ");

            if (baza.BezDodatkowServ)
                sonetaServerBuilder.Append($"/ext= ");

            if (baza.BezDLLSerweraApp)
                sonetaExplorerBuilder.Append($"/nodbextensions ");

            if (baza.BezDLLSerweraServ)
                sonetaServerBuilder.Append($"/nodbextensions ");

            sonetaExplorerParam = sonetaExplorerBuilder.ToString().Trim(' ');
            sonetaServerParam = sonetaServerBuilder.ToString().Trim(' ');
        }

        protected virtual void OnTextBoxValueChanged(TextBoxValueEventArgs e)
        {
            TextBoxValueChanged?.Invoke(this, e);
        }

        private void OperatorBtn_DropDownOpened(object sender, EventArgs e)
        {
            OperatorBtn.ItemsSource = MainWindow.GetOperators();
        }

        private void KopiujBazeBtn_Click(object sender, RoutedEventArgs e)
        {
            CopyDatabase copyDatabase = new CopyDatabase();
            copyDatabase.KopiujBtnClicked += KopiujBtn_Click;
            copyDatabase.Show();
        }

        private void KopiujBtn_Click(object sender, KopiujBtnValueEventArgs e)
        {
            string baza = Baza.NazwaBazy;
            Baza = e.WybranaBaza;
            Baza.NazwaBazy = baza;
            WprowadzUstawienia();
        }
    }

    public class TextBoxValueEventArgs : EventArgs
    {
        public string SonetaExplorerParam { get; set; }
        public string SonetaServerParam { get; set; }
        public Baza Baza { get; set; }

        public TextBoxValueEventArgs(Baza baza, string sonetaExplorerParam, string sonetaServerParam)
        {
            SonetaExplorerParam = sonetaExplorerParam;
            SonetaServerParam = sonetaServerParam;
            Baza = baza;
        }
    }
}