using Modelos;
using System;
using System.Configuration;
using System.Data;

namespace FELFactura
{
    public class XML
    {
        private DataSet dsEncabezado = new DataSet();
        private DataSet dsDetalle = new DataSet();
        String xml = "";
        static string UUID = "LOOKUP_ISSUED_INTERNAL_ID";
        static string DOCUMENTO = "GET_DOCUMENT";

        TipoDocumento tipoDocumento = new TipoDocumento();

        public String getXML(string xmlEncabezado, string xmlDetalle, string xmlFrases, string fac_num)
        {
            XmlToDataSet(xmlEncabezado, xmlDetalle);
            tipoDocumento.getTipo(dsEncabezado);

            switch (Constants.TIPO_DOCUMENTO)
            {
                case "NABN":
                    XMLNotasAbono xMLNotasAbono = new XMLNotasAbono();
                    xml = xMLNotasAbono.getXML(xmlEncabezado, xmlDetalle, fac_num);
                    break;
                default:
                    esExportacion(Constants.TIPO_EXPO, xmlEncabezado, xmlDetalle, fac_num, xmlFrases);
                    break;
            }

         //   getUUID("", xml);

            return xml;
        }


        private string esExportacion(bool esExportacion, string xmlEncabezado, string xmlDetalle, string fac_num, string xmlFrases)
        {
            if (esExportacion)
            {
                XMLFacturaExportacion xMLFacturaExportacion = new XMLFacturaExportacion();
                 return xml = xMLFacturaExportacion.getXML(xmlEncabezado, xmlDetalle, fac_num);
            }
            else
            {
                XMLFactura xMLFactura = new XMLFactura();
                return xml = xMLFactura.generarXml(xmlEncabezado, xmlDetalle, xmlFrases, fac_num);
            }
        }

        //Convertir XML a DataSet
        private bool XmlToDataSet(string XMLInvoice, string XMLDetailInvoce)
        {
            try
            {
                //Convieriendo XMl a DataSet Factura
                System.IO.StringReader rdinvoice = new System.IO.StringReader(XMLInvoice);
                dsEncabezado.ReadXml(rdinvoice);

                //Convieritiendo XML a DataSet Detalle Factura
                System.IO.StringReader rddetailinvoice = new System.IO.StringReader(XMLDetailInvoce);
                dsDetalle.ReadXml(rddetailinvoice);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string[] getUUID(string id, string xml)
        {
            Conector conector = new Conector();
            wsConnector wsConnector = new wsConnector();
            conector.id = id;
            conector.cData = "";
            conector.xml = "XML";
            conector.user = ConfigurationManager.AppSettings["USUARIO"].ToString();
            conector.url = ConfigurationManager.AppSettings["URL"].ToString();
            conector.requestor = ConfigurationManager.AppSettings["REQUESTOR"].ToString();
            conector.transaccion = UUID;
            conector.country = "GT";
            conector.nit = ConfigurationManager.AppSettings["NIT_EMISOR"].ToString();
            conector.pdf = false;
            conector.rutaPDf = "";
            return wsConnector.wsObtener(
                    conector.id,
                    conector.cData,
                    conector.xml,
                    conector.user,
                    conector.url,
                    conector.requestor,
                    conector.transaccion,
                    conector.country,
                    conector.nit,
                    conector.pdf,
                    conector.rutaPDf               
                );
        }

        public string[] setDocument(string id, string xml)
        {
            Conector conector = new Conector();
            wsConnector wsConnector = new wsConnector();
            conector.id = "POST_DOCUMENT_SAT";
            conector.cData = base64Encode(xml);
            conector.xml = id;
            conector.user = ConfigurationManager.AppSettings["USUARIO"].ToString();
            conector.url = ConfigurationManager.AppSettings["URL"].ToString();
            conector.requestor = ConfigurationManager.AppSettings["REQUESTOR"].ToString();
            conector.transaccion = "SYSTEM_REQUEST";
            conector.country = "GT";
            conector.nit = ConfigurationManager.AppSettings["NIT_EMISOR"].ToString();
            conector.pdf = false;
            conector.rutaPDf = "";
            return wsConnector.wsEnvio(
                    conector.id,
                    conector.cData,
                    conector.xml,
                    conector.user,
                    conector.url,
                    conector.requestor,
                    conector.transaccion,
                    conector.country,
                    conector.nit,
                    conector.pdf,
                    conector.rutaPDf
                );
        }
        public string[] setAnulacion(string xml)
        {
            Conector conector = new Conector();
            wsConnector wsConnector = new wsConnector();
            conector.id = "VOID_DOCUMENT";
            conector.cData = base64Encode(xml);
            conector.xml = "XML";
            conector.user = ConfigurationManager.AppSettings["USUARIO"].ToString();
            conector.url = ConfigurationManager.AppSettings["URL"].ToString();
            conector.requestor = ConfigurationManager.AppSettings["REQUESTOR"].ToString();
            conector.transaccion = "SYSTEM_REQUEST";
            conector.country = "GT";
            conector.nit = ConfigurationManager.AppSettings["NIT_EMISOR"].ToString();
            conector.pdf = false;
            conector.rutaPDf = "";
            return wsConnector.wsEnvio(
                    conector.id,
                    conector.cData,
                    conector.xml,
                    conector.user,
                    conector.url,
                    conector.requestor,
                    conector.transaccion,
                    conector.country,
                    conector.nit,
                    conector.pdf,
                    conector.rutaPDf
                );
        }

        public static string base64Encode(string plainText)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));

        }
    }  
}


