using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class Conector
    {
        public string id { get; set; }
        public string cData { get; set; }
        public string xml { get; set; }
        public string user { get; set; }
        public string url { get; set; }
        public string requestor { get; set; }
        public string transaccion { get; set; }
        public string country { get; set; }
        public string nit { get; set; }
        public bool pdf { get; set; }
        public string rutaPDf { get; set; }
    }
}
