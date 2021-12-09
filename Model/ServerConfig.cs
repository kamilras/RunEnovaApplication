using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunEnovaApplication.Model
{
    public class ServerConfig
    {
        public int Id { get; set; }
        public string FolderServ { get; set; }
        public bool BezHarmonogramuServ { get; set; }
        public bool BezDLLSerweraServ { get; set; }
        public bool BezDodatkowServ { get; set; }
        public string PortServ { get; set; }
        public string ListaBazDanychServ { get; set; }
        public string FolderDodatkowServ { get; set; }

    }
}
