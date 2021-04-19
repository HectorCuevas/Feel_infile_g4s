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
                    if (tipo.ToString().ToLower() == "si")
                    {
                        Constants.TIPO_EXPO = true;
                    }
                }
                else {
                    Constants.TIPO_EXPO = false;
                }

                var tipodoc = reader["tipo"];
                if (tipodoc != null)
                {

                    Constants.TIPO_DOCUMENTO = tipodoc.ToString();

                }

            }
        }
    }
}