using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RunEnova;
using RunEnova.Model;

namespace RunEnovaApplication
{
    /// <summary>
    /// Interaction logic for CopyDatabase.xaml
    /// </summary>
    public partial class CopyDatabase : Window
    {
        public CopyDatabase()
        {
            Context = new BazaDb();
            InitializeComponent();
        }

        public event EventHandler<KopiujBtnValueEventArgs> KopiujBtnClicked;

        public BazaDb Context { get; }
        public static Baza Baza { get; set; }

        private void ListaBaz_DropDownOpened(object sender, EventArgs e)
        {
            List<string> listaNazw = new List<string>();
            var lista = Context.Baza.ToList();

            ListaBaz.ItemsSource = lista.Select(x => x.NazwaBazy);
        }

        private void ListaBaz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            string baza = (string)comboBox.SelectedItem;

            Baza = Context.Baza.Where(x => x.NazwaBazy == baza).FirstOrDefault();
            WprowadzDane();
        }

        private void WprowadzDane()
        {
            extpathAppTxtBox.Text = Baza.FolderDodatkowApp;
            dbconfigAppLbl.Text = Baza.ListaBazDanychApp;
            configLbl.Text = Baza.KonfiguracjaApp;
            folderLbl.Text = Baza.FolderUIApp;
            operatorLbl.Text = Baza.Operator;
            nodbextensionsAppChBox.IsChecked = Baza.BezDLLSerweraApp;
            extAppChBox.IsChecked = Baza.BezDodatkowApp;
            WersjaAppTxtBox.Text = Baza.FolderApp;

            extpathSerwLbl.Text = Baza.FolderDodatkowServ;
            dbconfigSerwLbl.Text = Baza.ListaBazDanychServ;
            portLbl.Text = Baza.PortServ;
            nodbextensionsSerwChBox.IsChecked = Baza.BezDLLSerweraServ;
            extSerwChBox.IsChecked = Baza.BezDodatkowServ;
            noshedulerSerwChBox.IsChecked = Baza.BezHarmonogramuServ;
            WersjaSerwTxtBox.Text = Baza.FolderServ;
        }

        private void KopiujBtn_Click(object sender, RoutedEventArgs e)
        {
            OnKopiujBtnClicked(new KopiujBtnValueEventArgs(Baza));
            Close();
        }

        protected virtual void OnKopiujBtnClicked(KopiujBtnValueEventArgs e)
        {
            KopiujBtnClicked?.Invoke(this, e);
        }

        private void ZamknijBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }

    public class KopiujBtnValueEventArgs : EventArgs
    {
        public Baza WybranaBaza { get; set; }

        public KopiujBtnValueEventArgs(Baza baza)
        {
            WybranaBaza = baza;
        }
    }
}