using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;

namespace FELFactura
{
    /// <summary>
    /// Descripción breve de getDocument
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class getDocument : System.Web.Services.WebService
    {
        XML xml = new XML();

        [WebMethod]
        public DataSet GetDocument(String xml_enc, String xml_det, string xml_frases, String num_fac)
        {
            DataSet ds = new DataSet();
            try
            {
                string[] xmlDoc = new string[12];
                string xmlGenerado = "";            

                String path = "";             
                XmlDocument doc = new XmlDocument();

                xmlGenerado = xml.getXML(xml_enc, xml_det, xml_frases, num_fac);

                xmlDoc = xml.setDocument(Constants.IDENTIFICADOR_DTE, xmlGenerado);

                doc.PreserveWhitespace = true;
                doc.LoadXml(xmlGenerado);
      
                path = ConfigurationManager.AppSettings["RutaArchivos"].ToString();

                using (XmlTextWriter writer = new XmlTextWriter(path + Constants.TIPO_DOC + "-" + Constants.IDENTIFICADOR_DTE + ".xml", null))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                   // writer.WriteStartElement("root");
                    writer.Formatting = System.Xml.Formatting.Indented;
                    doc.Save(writer);
                }

                ds =  setResponse(xmlDoc);
             
            }
            catch (DirectoryNotFoundException ex)
            {
                ds = setError("LA RUTA PARA ALMACENAR LOS ARCHIVOS NO ES VALIDA O NO EXISTE.");
            }
            catch (ArgumentNullException ex)
            {
                ds = setError("EL DOCUMENTO XML NO SE PUDO CREAR POR LO TANTO ES NULO");
            }
            catch (XmlException ex)
            {
                ds = setError("EL DOCUMENTO XML CONTIENE VALORES INCORRECTOS POR LO TANTO NO SE PUDO GENERAR");
            }
            catch (TypeInitializationException ex)
            {
                ds = setError("LAS VARIABLES DEL ARCHIVO WEB.CONFIG SON INCORRECTAS");
            }
            catch (Exception ex)
            {
                ds = setError("CONTACTE AL ADMINISTRADOR"  + ex.ToString());
            }

            return ds;
        }


        [WebMethod]
        public string[] SetAnulacion(String xmlDoc, String cod)
        {
            string[] xmlArr = new string[12];
            DataSet ds = new DataSet();
            String xmlGenerado = "";
            XMLAnular xMLAnular = new XMLAnular();

            if (!string.IsNullOrEmpty(xmlDoc))
            {
                xmlGenerado = xMLAnular.getXML(xmlDoc, cod);

                xmlArr = xml.setAnulacion(xmlGenerado);
            }
            else
            {
                xmlArr[3] = "Error en las etiquetas de SQL. No se envio correctamente el XML de FOX ";
            }


            return xmlArr;
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
        private DataSet setError(string ex)
        {
            DataSet dsError = new DataSet();
            DataTable dt = new DataTable("resultado");
            dt.Columns.Add(new DataColumn("resultado", typeof(string)));
            dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
            DataRow dr = dt.NewRow();
            dr["resultado"] = "False";
            dr["descripcion"] = ex;
            dt.Rows.Add(dr);
            dsError.Tables.Add(dt);
            return dsError;
        }



        private DataSet setResponse(string[] response)
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("resultado");
            dt.Columns.Add(new DataColumn("resultado", typeof(string)));
            dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
            dt.Columns.Add(new DataColumn("uuid", typeof(string)));
            dt.Columns.Add(new DataColumn("xml", typeof(string)));


            DataRow dr = dt.NewRow();
            dr["resultado"] = response[1];
            dr["descripcion"] = response[2];
            dr["uuid"] = response[6];
            dr["xml"] = response[5];
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return ds;
        }
    }
}
