namespace RunEnova.Model
{
    public class Baza
    {
        public int Id { get; set; }
        public string NazwaBazySQL { get; set; }
        public string NazwaBazyEnova { get; set; }
        public string Operator { get; set; }
        public string FolderApp { get; set; }
        public string FolderUIApp { get; set; }
        public bool BezDLLSerweraApp { get; set; }
        public bool BezDodatkowApp { get; set; }
        public string ListaBazDanychApp { get; set; }
        public string FolderDodatkowApp { get; set; }
        public string KonfiguracjaApp { get; set; }
        public string FolderServ { get; set; }
        public bool BezHarmonogramuServ { get; set; }
        public bool BezDLLSerweraServ { get; set; }
        public bool BezDodatkowServ { get; set; }
        public string PortServ { get; set; }
        public string ListaBazDanychServ { get; set; }
        public string FolderDodatkowServ { get; set; }
    }
}
