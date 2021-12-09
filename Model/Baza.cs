using RunEnovaApplication.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunEnova.Model
{
    public class Baza
    {
        public int Id { get; set; }
        public string NazwaBazy { get; set; }
        public string Operator { get; set; }
        public virtual ApplicationConfig ApplicationConfig { get; set; }
        public virtual ServerConfig ServerConfig { get; set; }
    }
}
