using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Modelos
{
    public class RespuestaSolitud
    {
        
        public String resultado { get; set; }
        public String descripcion { get; set; }
        public String archivo { get; set; }
        public String xml { get; set; }

        public override string ToString()
        {
            return "resultado: " + resultado + System.Environment.NewLine +
                "descripcion: " + descripcion + System.Environment.NewLine +
                "archivo: " + archivo ;
        }
    }
}
