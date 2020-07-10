using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class Complemento
    {
        public String nombreComplemento { get; set; }
        public String uriComplemento { get; set; }
        public ReferenciasNota referenciasNota { get; set; }
    }
}
