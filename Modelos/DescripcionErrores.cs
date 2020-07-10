using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class DescripcionErrores
    {
        public bool resultado { get; set; }
        public string fuente { get; set; }
        public string categoria { get; set; }
        public string numeral { get; set; }
        public string validacion { get; set; }
        public string mensaje_error { get; set; }

        public override string ToString()
        {
             return "resultado: " + this.resultado.ToString() + System.Environment.NewLine +
                 "fuente: " + this.fuente.ToString() + System.Environment.NewLine +
                 "categoria: " + this.categoria.ToString() + System.Environment.NewLine +
                 "numeral: " + this.numeral.ToString() + System.Environment.NewLine +
                 "validacion: " + this.validacion.ToString() + System.Environment.NewLine +
                 "mensaje_error: " + this.mensaje_error.ToString() + System.Environment.NewLine;
        }
    }
}
