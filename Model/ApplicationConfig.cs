using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunEnovaApplication.Model
{
    public class ApplicationConfig
    {
        public int Id { get; set; }
        public string FolderApp { get; set; }
        public string FolderUIApp { get; set; }
        public bool BezDLLSerweraApp { get; set; }
        public bool BezDodatkowApp { get; set; }
        public string ListaBazDanychApp { get; set; }
        public string FolderDodatkowApp { get; set; }
        public string KonfiguracjaApp { get; set; }
    }
}
