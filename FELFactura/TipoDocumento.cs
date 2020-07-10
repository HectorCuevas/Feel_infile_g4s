using Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FELFactura
{
    public  class TipoDocumento
    {
        public  void getTipo(DataSet dstcompanyxml)
        {
            foreach (DataRow reader in dstcompanyxml.Tables[0].Rows)
            {
                // var tipo = EXPO
                var tipo = reader["exportacion"];
                if (tipo != null)
                {
                    if (tipo.ToString() == "SI")
                    {
                        Constants.isEXP = true;
                        Constants.TIPO_EXPO = "SI";
                    }
                }
                else {
                    Constants.TIPO_EXPO = "NO";
                }

            }
        }
    }
}