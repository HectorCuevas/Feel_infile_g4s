using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class NotaCredito
    {
        public String IDComplemento { get; set; }
        public String nombreComplemento { get; set; }
        public String FechaEmisionDocumentoOrigen { get; set; }
        public String MotivoAjuste { get; set; }
        public String NumeroAutorizacionDocumentoOrigen { get; set; }
        public String SerieDocumentoOrigen { get; set; }
        public String NumeroDocumentoOrigen { get; set; }
    }
}
