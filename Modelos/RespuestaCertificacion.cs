using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos
{
    public class RespuestaCertificacion
    {
        public String resultado { get; set; }
        public String fecha { get; set; }
        public String origen { get; set; }
        public String descripcion { get; set; }
        public ControlEmision control_emision { get; set; }
        public bool alertas_infile { get; set; }
        public List<string> descripcion_alertas_infile { get; set; }
        public bool alertas_sat { get; set; }
        public List<object> descripcion_alertas_sat { get; set; }
        public int cantidad_errores { get; set; }
        public List<DescripcionErrores> descripcion_errores { get; set; }
        public string informacion_adicional { get; set; }
        public string uuid { get; set; }
        public string serie { get; set; }
        public string numero { get; set; }
        public string xml_certificado { get; set; }
    }
}
