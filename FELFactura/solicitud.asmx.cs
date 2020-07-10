using Modelos;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using System.Xml;

namespace FELFactura
{
    /// <summary>
    /// Descripción breve de solicitud
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class solicitud : System.Web.Services.WebService
    {


        [WebMethod]
        public DataSet envioSolicitud(String xml_enc, String xml_det, String num_fac)
        {
            String xmlDoc = "";
            String asd = "";
            DataSet ds = new DataSet();
            try
            {
                XMLFactura xml = new XMLFactura();

                xmlDoc = xml.getXML(xml_enc, xml_det, "ashdashd", num_fac);
                //bool hayInternet = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(xmlDoc);
                String path = ConfigurationManager.AppSettings["RutaArchivos"].ToString();
                using (XmlTextWriter writer = new XmlTextWriter(path + Constants.TIPO_DOC + "-" + Constants.IDENTIFICADOR_DTE + ".xml", null))
                {
                    writer.Formatting = System.Xml.Formatting.Indented;
                    doc.Save(writer);
                }

                //saveJSon("", "C:\\FACTURAS_JSON\\archivo.txt");
                ds = certificacion(xmlDoc, num_fac);
            }
            catch (DirectoryNotFoundException ex)
            {
                DataSet dsError = new DataSet();
                DataTable dt = new DataTable("resultado");
                dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                //dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                DataRow dr = dt.NewRow();
                dr["resultado"] = "LA RUTA PARA ALMACENAR LOS ARCHIVOS NO ES VALIDA O NO EXISTE. \n " + Environment.NewLine + ex.ToString();
                //  dr["xmlGenerado"] =xmlDoc;
                dt.Rows.Add(dr);
                dsError.Tables.Add(dt);
                ds = dsError;
            }
            catch (ArgumentNullException ex)
            {
                DataSet dsError = new DataSet();
                DataTable dt = new DataTable("resultado");
                dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                //dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                DataRow dr = dt.NewRow();
                dr["resultado"] = "EL DOCUMENTO XML NO SE PUDO CREAR POR LO TANTO ES NULO. \n " + Environment.NewLine + ex.ToString();
                //  dr["xmlGenerado"] =xmlDoc;
                dt.Rows.Add(dr);
                dsError.Tables.Add(dt);
                ds = dsError;
            }
            catch (XmlException ex)
            {
                DataSet dsError = new DataSet();
                DataTable dt = new DataTable("resultado");
                dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                //dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                DataRow dr = dt.NewRow();
                dr["resultado"] = "EL DOCUMENTO XML CONTIENE VALORES INCORRECTOS POR LO TANTO NO SE PUDO GENERAR \n " + Environment.NewLine + ex.ToString();
                //  dr["xmlGenerado"] =xmlDoc;
                dt.Rows.Add(dr);
                dsError.Tables.Add(dt);
                ds = dsError;
            }
            catch (Exception ex)
            {
                DataSet dsError = new DataSet();
                DataTable dt = new DataTable("resultado");
                dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                //dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                DataRow dr = dt.NewRow();
                dr["resultado"] = ex.ToString();
                //  dr["xmlGenerado"] =xmlDoc;
                dt.Rows.Add(dr);
                dsError.Tables.Add(dt);
                ds = dsError;
            }

            return ds;
        }

        [WebMethod]
        public DataSet envioAnulacion(String xml, String cod)
        {
            String xmlDoc = "";
            String xmlGenerado = "";
            DataSet ds = new DataSet();
            try
            {
                XMLAnular xMLAnular = new XMLAnular();
                xmlGenerado = xMLAnular.getXML(xml, "", cod);
                ds = wsSolicitudAnulacion(xmlGenerado, cod, xmlGenerado);
            }
            catch (Exception ex)
            {
                DataSet dsError = new DataSet();
                DataTable dt = new DataTable("resultado");
                dt.Columns.Add(new DataColumn("resultado", typeof(string)));

                DataRow dr = dt.NewRow();
                dr["resultado"] = ex.ToString();
                dt.Rows.Add(dr);
                dsError.Tables.Add(dt);
                ds = dsError;
            }
            return ds;
        }


        #region Metodos Anulacion
        public DataSet wsSolicitudAnulacion(String xml, String cod, String xmlGenerado)
        {
            DataSet dsError = new DataSet();
            DataSet respuesta = new DataSet();
            String d = "";
            Task t = Task.Factory.StartNew(async () =>
            {
                using (var client = new HttpClient())
                {
                    XmlDocument xmltest = new XmlDocument();
                    xmltest.LoadXml(xml);
                    String xmlNoBreaks = xml.Replace(System.Environment.NewLine, "");
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(xmlNoBreaks);
                    System.Convert.ToBase64String(plainTextBytes);
                    RespuestaSolitud respuestaSolitud = new RespuestaSolitud();
                    Solictud solictud = new Solictud();
                    solictud.llave = Constants.LLAVE_TOKEN;
                    // solictud.archivo = "PGR0ZTpHVERvY3VtZW50byB4bWxuczpkcz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxkc2lnIyIgeG1sbnM6ZHRlPSJodHRwOi8vd3d3LnNhdC5nb2IuZ3QvZHRlL2ZlbC8wLjEuMCIgeG1sbnM6eHNpPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYS1pbnN0YW5jZSIgVmVyc2lvbj0iMC40IiB4c2k6c2NoZW1hTG9jYXRpb249Imh0dHA6Ly93d3cuc2F0LmdvYi5ndC9kdGUvZmVsLzAuMS4wIj4gIAogIDxkdGU6U0FUIENsYXNlRG9jdW1lbnRvPSJkdGUiPgogICAgPGR0ZTpEVEUgSUQ9IkRhdG9zQ2VydGlmaWNhZG9zIj4KICAgICAgPGR0ZTpEYXRvc0VtaXNpb24gSUQ9IkRhdG9zRW1pc2lvbiI+CiAgICAgICAgPGR0ZTpEYXRvc0dlbmVyYWxlcyBDb2RpZ29Nb25lZGE9IkdUUSIgRmVjaGFIb3JhRW1pc2lvbj0iMjAxOS0wNy0wOFQwOTo1ODowMC0wNjowMCIgVGlwbz0iRkFDVCI+PC9kdGU6RGF0b3NHZW5lcmFsZXM+CiAgICAgICAgPGR0ZTpFbWlzb3IgQWZpbGlhY2lvbklWQT0iR0VOIiBDb2RpZ29Fc3RhYmxlY2ltaWVudG89IjEiIENvcnJlb0VtaXNvcj0iZGVtb0BkZW1vLmNvbS5ndCIgTklURW1pc29yPSIyMzc1MDI3OCIgTm9tYnJlQ29tZXJjaWFsPSJERU1PIiBOb21icmVFbWlzb3I9IkRFTU8sIFNPQ0lFREFEIEFOT05JTUEiPgogICAgICAgICAgPGR0ZTpEaXJlY2Npb25FbWlzb3I+CiAgICAgICAgICAgIDxkdGU6RGlyZWNjaW9uPkNVSURBRDwvZHRlOkRpcmVjY2lvbj4KICAgICAgICAgICAgPGR0ZTpDb2RpZ29Qb3N0YWw+MDEwMDE8L2R0ZTpDb2RpZ29Qb3N0YWw+CiAgICAgICAgICAgIDxkdGU6TXVuaWNpcGlvPkdVQVRFTUFMQTwvZHRlOk11bmljaXBpbz4KICAgICAgICAgICAgPGR0ZTpEZXBhcnRhbWVudG8+R1VBVEVNQUxBPC9kdGU6RGVwYXJ0YW1lbnRvPgogICAgICAgICAgICA8ZHRlOlBhaXM+R1Q8L2R0ZTpQYWlzPgogICAgICAgICAgPC9kdGU6RGlyZWNjaW9uRW1pc29yPgogICAgICAgIDwvZHRlOkVtaXNvcj4KICAgICAgICA8ZHRlOlJlY2VwdG9yIENvcnJlb1JlY2VwdG9yPSJsZXlvYWx2aXp1cmVzNDQ1NkBnbWFpbC5jb20iIElEUmVjZXB0b3I9Ijc2MzY1MjA0IiBOb21icmVSZWNlcHRvcj0iSmFpbWUgQWx2aXp1cmVzIj4KICAgICAgICAgIDxkdGU6RGlyZWNjaW9uUmVjZXB0b3I+CiAgICAgICAgICAgIDxkdGU6RGlyZWNjaW9uPkNVSURBRDwvZHRlOkRpcmVjY2lvbj4KICAgICAgICAgICAgPGR0ZTpDb2RpZ29Qb3N0YWw+MDEwMDE8L2R0ZTpDb2RpZ29Qb3N0YWw+CiAgICAgICAgICAgIDxkdGU6TXVuaWNpcGlvPkdVQVRFTUFMQTwvZHRlOk11bmljaXBpbz4KICAgICAgICAgICAgPGR0ZTpEZXBhcnRhbWVudG8+R1VBVEVNQUxBPC9kdGU6RGVwYXJ0YW1lbnRvPgogICAgICAgICAgICA8ZHRlOlBhaXM+R1Q8L2R0ZTpQYWlzPgogICAgICAgICAgPC9kdGU6RGlyZWNjaW9uUmVjZXB0b3I+CiAgICAgICAgPC9kdGU6UmVjZXB0b3I+CjxkdGU6RnJhc2VzPjxkdGU6RnJhc2UgQ29kaWdvRXNjZW5hcmlvPSIxIiBUaXBvRnJhc2U9IjEiPjwvZHRlOkZyYXNlPjwvZHRlOkZyYXNlcz48ZHRlOkl0ZW1zPiAgICAgICAgICA8ZHRlOkl0ZW0gQmllbk9TZXJ2aWNpbz0iQiIgTnVtZXJvTGluZWE9IjEiPgogICAgICAgICAgICA8ZHRlOkNhbnRpZGFkPjE8L2R0ZTpDYW50aWRhZD4KPGR0ZTpVbmlkYWRNZWRpZGE+VU5EPC9kdGU6VW5pZGFkTWVkaWRhPgogICAgICAgICAgICA8ZHRlOkRlc2NyaXBjaW9uPlBST0RVQ1RPMTwvZHRlOkRlc2NyaXBjaW9uPgogICAgICAgICAgICA8ZHRlOlByZWNpb1VuaXRhcmlvPjEyMDwvZHRlOlByZWNpb1VuaXRhcmlvPgogICAgICAgICAgICA8ZHRlOlByZWNpbz4xMjA8L2R0ZTpQcmVjaW8+CjxkdGU6RGVzY3VlbnRvPjA8L2R0ZTpEZXNjdWVudG8+CjxkdGU6SW1wdWVzdG9zPiAgICAgICAgICAgICAgPGR0ZTpJbXB1ZXN0bz4KICAgICAgICAgICAgICAgIDxkdGU6Tm9tYnJlQ29ydG8+SVZBPC9kdGU6Tm9tYnJlQ29ydG8+CiAgICAgICAgICAgICAgICA8ZHRlOkNvZGlnb1VuaWRhZEdyYXZhYmxlPjE8L2R0ZTpDb2RpZ29VbmlkYWRHcmF2YWJsZT4KPGR0ZTpNb250b0dyYXZhYmxlPjEwNy4xNDwvZHRlOk1vbnRvR3JhdmFibGU+CiAgICAgICAgICAgICAgICA8ZHRlOk1vbnRvSW1wdWVzdG8+MTIuODY8L2R0ZTpNb250b0ltcHVlc3RvPgogICAgICAgICAgICAgIDwvZHRlOkltcHVlc3RvPgo8L2R0ZTpJbXB1ZXN0b3M+ICAgICAgICAgICAgPGR0ZTpUb3RhbD4xMjA8L2R0ZTpUb3RhbD4KICAgICAgICAgIDwvZHRlOkl0ZW0+CjwvZHRlOkl0ZW1zPiAgICAgICAgPGR0ZTpUb3RhbGVzPgo8ZHRlOlRvdGFsSW1wdWVzdG9zPjxkdGU6VG90YWxJbXB1ZXN0byBOb21icmVDb3J0bz0iSVZBIiBUb3RhbE1vbnRvSW1wdWVzdG89IjEyLjg2Ij48L2R0ZTpUb3RhbEltcHVlc3RvPjwvZHRlOlRvdGFsSW1wdWVzdG9zPiAgIDxkdGU6R3JhblRvdGFsPjEyMDwvZHRlOkdyYW5Ub3RhbD4KICAgICAgICA8L2R0ZTpUb3RhbGVzPgogICAgICA8L2R0ZTpEYXRvc0VtaXNpb24+CiAgICA8L2R0ZTpEVEU+CiA8ZHRlOkFkZW5kYT4KPENvZGlnb19jbGllbnRlPkMwMTwvQ29kaWdvX2NsaWVudGU+CjxPYnNlcnZhY2lvbmVzPkVTVEEgRVMgVU5BIEFERU5EQTwvT2JzZXJ2YWNpb25lcz4KIDwvZHRlOkFkZW5kYT4gIDwvZHRlOlNBVD4KPGRzOlNpZ25hdHVyZSBJZD0ieG1sZHNpZy03ZjEzNGQwNS00ZTk2LTQxNTgtYmNiNC0zN2RiYmVmMmZjZmQiPgo8ZHM6U2lnbmVkSW5mbz4KPGRzOkNhbm9uaWNhbGl6YXRpb25NZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy9UUi8yMDAxL1JFQy14bWwtYzE0bi0yMDAxMDMxNSI+PC9kczpDYW5vbmljYWxpemF0aW9uTWV0aG9kPgo8ZHM6U2lnbmF0dXJlTWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8wNC94bWxkc2lnLW1vcmUjcnNhLXNoYTI1NiI+PC9kczpTaWduYXR1cmVNZXRob2Q+CjxkczpSZWZlcmVuY2UgSWQ9InhtbGRzaWctN2YxMzRkMDUtNGU5Ni00MTU4LWJjYjQtMzdkYmJlZjJmY2ZkLXJlZjAiIFVSST0iI0RhdG9zRW1pc2lvbiI+CjxkczpUcmFuc2Zvcm1zPgo8ZHM6VHJhbnNmb3JtIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxkc2lnI2VudmVsb3BlZC1zaWduYXR1cmUiPjwvZHM6VHJhbnNmb3JtPgo8L2RzOlRyYW5zZm9ybXM+CjxkczpEaWdlc3RNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGVuYyNzaGEyNTYiPjwvZHM6RGlnZXN0TWV0aG9kPgo8ZHM6RGlnZXN0VmFsdWU+KzlNZWJuSk5oMFV4VUZKL0tvUDU0Uzg3OStuSm4wU2E1QXNMNUJkY2lTRT08L2RzOkRpZ2VzdFZhbHVlPgo8L2RzOlJlZmVyZW5jZT4KPGRzOlJlZmVyZW5jZSBUeXBlPSJodHRwOi8vdXJpLmV0c2kub3JnLzAxOTAzI1NpZ25lZFByb3BlcnRpZXMiIFVSST0iI3htbGRzaWctN2YxMzRkMDUtNGU5Ni00MTU4LWJjYjQtMzdkYmJlZjJmY2ZkLXNpZ25lZHByb3BzIj4KPGRzOkRpZ2VzdE1ldGhvZCBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMDQveG1sZW5jI3NoYTI1NiI+PC9kczpEaWdlc3RNZXRob2Q+CjxkczpEaWdlc3RWYWx1ZT4vWDJ6VitVK2FhdWdUa1JKd2pPeTBFVW9BOGZFRnI0WE1EdWtINmpIMjFZPTwvZHM6RGlnZXN0VmFsdWU+CjwvZHM6UmVmZXJlbmNlPgo8L2RzOlNpZ25lZEluZm8+CjxkczpTaWduYXR1cmVWYWx1ZSBJZD0ieG1sZHNpZy03ZjEzNGQwNS00ZTk2LTQxNTgtYmNiNC0zN2RiYmVmMmZjZmQtc2lndmFsdWUiPgpNUFBVVXU0NkVoYS80eFlqWjJyYXBRQWZHdmUrSWRuV1VJL2RBQVlzZHBYSjhKTUF2VUNJdkVIK2lWaUdFNlZvUkVoTk1RUTg4WGhICkZ4RlN3VGRzM0xQVEUwL0haZnhVemdJK1kreEV2c0RjM1BJdUgxblA3bnpwQURwaHNUc2FBa2hTQmxyL2QvdFIwS2tlSTkxZCtScDMKTm5wOUREa0FaRVV5N3V3a29xL2t6cEw1OG43enR6Mlk0UVgwK01GelJGSFFPV0pZZDExaEd0cFc1L0lCclNuUC9uQlNqT2o1YWdhNApJaFhuSVlrbm5WaTNtWG0zSDdEbzlQRFZZcU1PTkpHS0ZoREFhUFVEYXBVa0I2M29JMTg1R3VjV3lUa0ZqTmdwYUdRU1gzUmZsWXR2CllubEoxMUs0eS9NRWNzTFdad0o1VVRyN0JaQUhWUWZqb3lmZUhBPT0KPC9kczpTaWduYXR1cmVWYWx1ZT4KPGRzOktleUluZm8+CjxkczpYNTA5RGF0YT4KPGRzOlg1MDlDZXJ0aWZpY2F0ZT4KTUlJRFVqQ0NBanFnQXdJQkFnSUlFa1dkb056R2hKNHdEUVlKS29aSWh2Y05BUUVMQlFBd0tURU1NQW9HQTFVRUF3d0RSa1ZNTVF3dwpDZ1lEVlFRS0RBTlRRVlF4Q3pBSkJnTlZCQVlUQWtkVU1CNFhEVEU1TURZd05qRTJNelUxTVZvWERUSXhNRFl3TlRFMk16VTFNVm93CktERVJNQThHQTFVRUF3d0lNak0zTlRBeU56Z3hFekFSQmdOVkJBb01Dbk5oZEM1bmIySXVaM1F3Z2dFaU1BMEdDU3FHU0liM0RRRUIKQVFVQUE0SUJEd0F3Z2dFS0FvSUJBUUNVaGJ5LzFRR0RaR2R2SmZ0Wjk5YWZ4MUUyekMvRUYrVWxLZ1hCakUyRmNtV2IweHlmMGFKOAo5cnd4OGl2VE1VSVBmL3lLUlZtV2lWcmcrbXAwSU1IVjU0NTNRWnMrVEJGWTNOcEs2Qm1NcTU1YUR4NUtkRENsbXlVaENZRzJ3T3pFCnF2d3NVRnJsekpCK0tFR0g0Wlg1VTF5cTJ2VWh1dmdSY2VaWi9BRkl3SnpsUytHS0EyU3RmMzJYOWloNEcybVBtM1lNRHJIelpKMUwKRmdVdTloNUdqU0ZoenpIdllycStJbXdaeGxFUFAxNk5kc0RJSDZIWG9lZll4WUlZRzlyaCt0Q2hRZUYvVE9JUXpFUWdnMkRhR3ZueQpteXAwK1RGbFpMTU9jTGlBR05JcnZzUmZHS0swdEtwY0lVRENWYU1ROFRtYi9iQjZGeHlBZ2FrZVI3OVJBZ01CQUFHamZ6QjlNQXdHCkExVWRFd0VCL3dRQ01BQXdId1lEVlIwakJCZ3dGb0FVYzczemlsSnNNNWhaMS9mUXJiRnpqTFJRU2dvd0hRWURWUjBsQkJZd0ZBWUkKS3dZQkJRVUhBd0lHQ0NzR0FRVUZCd01FTUIwR0ExVWREZ1FXQkJRbnV2cEV2bkNJMEV4STczdm1JR3pab0lvdUlUQU9CZ05WSFE4QgpBZjhFQkFNQ0JlQXdEUVlKS29aSWh2Y05BUUVMQlFBRGdnRUJBSGMxWWlsYVdobThwNG1wY1dyL1djMENxbEtxbTdRcnU3RWRrdUFECnh4T0NpZVZJUTExOFlnNkJPV1VLV3NieHRnaTlRRjV3c0FBNWJ2Q1pMTTlZWGg0ZmFrQW8zenNVVjJXOC9RNUVQdC9zenUvUUdsbmcKcG1aTzlJNm1GR2VzbjNjQzFGYWVqRUUxekNzaEhaMkZlK3J2MW5RZTJNN2VFT0RSbk9tbEtET25XbkUvMXRXZmJaUDFhb09VRHVCWgpIQ2VicnlhMG9WTmc5UVRIOTRaWEVVZUNlWE9CZGFXZ0RpZ21hamV3dS9zdnlXbUJqRWE3YVZGT01ZZmVmUnVRVUh6cWw1SU4zT3hmClBmOE1hV01uOU9sMDFDM1FxRnJrSXhILzZJdENMRzZFWnNTdlJzcTBwRXJMQy9xMm02aVIyUW5CakFHc1hjdGRVZ0FvbjFrR3RTbz0KPC9kczpYNTA5Q2VydGlmaWNhdGU+CjwvZHM6WDUwOURhdGE+CjwvZHM6S2V5SW5mbz4KPGRzOk9iamVjdD48eGFkZXM6UXVhbGlmeWluZ1Byb3BlcnRpZXMgeG1sbnM6eGFkZXM9Imh0dHA6Ly91cmkuZXRzaS5vcmcvMDE5MDMvdjEuMy4yIyIgeG1sbnM6eGFkZXMxNDE9Imh0dHA6Ly91cmkuZXRzaS5vcmcvMDE5MDMvdjEuNC4xIyIgVGFyZ2V0PSIjeG1sZHNpZy03ZjEzNGQwNS00ZTk2LTQxNTgtYmNiNC0zN2RiYmVmMmZjZmQiPjx4YWRlczpTaWduZWRQcm9wZXJ0aWVzIElkPSJ4bWxkc2lnLTdmMTM0ZDA1LTRlOTYtNDE1OC1iY2I0LTM3ZGJiZWYyZmNmZC1zaWduZWRwcm9wcyI+PHhhZGVzOlNpZ25lZFNpZ25hdHVyZVByb3BlcnRpZXM+PHhhZGVzOlNpZ25pbmdUaW1lPjIwMTktMDctMTFUMTc6MzU6MDguNjA0WjwveGFkZXM6U2lnbmluZ1RpbWU+PHhhZGVzOlNpZ25pbmdDZXJ0aWZpY2F0ZT48eGFkZXM6Q2VydD48eGFkZXM6Q2VydERpZ2VzdD48ZHM6RGlnZXN0TWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8wNC94bWxlbmMjc2hhMjU2Ij48L2RzOkRpZ2VzdE1ldGhvZD48ZHM6RGlnZXN0VmFsdWU+dVg2QTZyWUFSUDBFb09mTytUUTduN01vNGpKS0ZvWXFwenlkOWxFSzRuZz08L2RzOkRpZ2VzdFZhbHVlPjwveGFkZXM6Q2VydERpZ2VzdD48eGFkZXM6SXNzdWVyU2VyaWFsPjxkczpYNTA5SXNzdWVyTmFtZT5DPUdULE89U0FULENOPUZFTDwvZHM6WDUwOUlzc3Vlck5hbWU+PGRzOlg1MDlTZXJpYWxOdW1iZXI+MTMxNjYzMTc4MDMwMDA2MzkwMjwvZHM6WDUwOVNlcmlhbE51bWJlcj48L3hhZGVzOklzc3VlclNlcmlhbD48L3hhZGVzOkNlcnQ+PC94YWRlczpTaWduaW5nQ2VydGlmaWNhdGU+PC94YWRlczpTaWduZWRTaWduYXR1cmVQcm9wZXJ0aWVzPjwveGFkZXM6U2lnbmVkUHJvcGVydGllcz48L3hhZGVzOlF1YWxpZnlpbmdQcm9wZXJ0aWVzPjwvZHM6T2JqZWN0Pgo8L2RzOlNpZ25hdHVyZT48L2R0ZTpHVERvY3VtZW50bz4=";
                    solictud.archivo = System.Convert.ToBase64String(plainTextBytes);
                    solictud.codigo = cod;
                    solictud.alias = Constants.ALIAS;
                    solictud.es_anulacion = "N";

                    //pasar a json el objeto
                    string json = JsonConvert.SerializeObject(solictud);


                    client.BaseAddress = new Uri(Constants.URL_SOLICITUD_FIRMA);
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Constants.METODO_SOLICITUD_FIRMA);
                    request.Content = new StringContent(json,
                                                        Encoding.UTF8,
                                                        "application/json");

                    //  client.SendAsync(request).RunSynchronously();
                    using (HttpResponseMessage response = client.SendAsync(request).Result)
                    {
                        using (HttpContent content = response.Content)
                        {

                            try
                            {
                                d = content.ReadAsStringAsync().Result;
                                respuestaSolitud = JsonConvert.DeserializeObject<RespuestaSolitud>(content.ReadAsStringAsync().Result);

                                // d = anulacion(respuestaSolitud.archivo);
                                String json1 = content.ReadAsStringAsync().Result;
                                respuestaSolitud = JsonConvert.DeserializeObject<RespuestaSolitud>(content.ReadAsStringAsync().Result);
                                DataTable dt = new DataTable("resultado");
                                dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                                dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
                                dt.Columns.Add(new DataColumn("archivo", typeof(string)));
                                dt.Columns.Add(new DataColumn("xml", typeof(string)));
                                //dt.Columns.Add(new DataColumn("archivo", typeof(string)));

                                DataRow dr = dt.NewRow();
                                dr["resultado"] = respuestaSolitud.resultado.ToString();
                                dr["descripcion"] = respuestaSolitud.descripcion;
                                dr["archivo"] = respuestaSolitud.archivo;
                                dr["xml"] = xmlGenerado;
                                dt.Rows.Add(dr);
                                dsError.Tables.Add(dt);
                                respuesta = dsError;
                                respuesta = anulacion(respuestaSolitud.archivo, xmlGenerado);

                            }
                            catch (Exception ex)
                            {
                                d = ex.ToString();
                                DataTable dt = new DataTable("resultado");
                                dt.Columns.Add(new DataColumn("resultado", typeof(string)));

                                DataRow dr = dt.NewRow();
                                dr["resultado"] = ex.ToString();
                                dt.Rows.Add(dr);



                                dsError.Tables.Add(dt);
                                respuesta = dsError;

                            }
                        }
                    }
                }
            });
            t.Wait();
            return respuesta;
        }

        public DataSet anulacion(String xml, String xmlGenerado)
        {
            Certificacion certificacion = new Certificacion();
            certificacion.nit_emisor = Constants.NIT_EMISOR;
            certificacion.correo_copia = Constants.CORREO_COPIA;
            certificacion.xml_dte = xml;
            DataSet dataSet = new DataSet();
            DataSet dsError = new DataSet();
            //pasar a json el objeto
            string json = JsonConvert.SerializeObject(certificacion);

            RespuestaCertificacion respuesta = new RespuestaCertificacion();
            String d = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.URL_ANULACION_DTE);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Constants.METODO_ANULACION_DTE);
                request.Headers.Add(Constants.HEADER_USUARIO, Constants.HEADER_USUARIO_TOKEN);
                request.Headers.Add(Constants.HEADER_LLAVE, Constants.HEADER_LLAVE_EMISOR);
                request.Headers.Add(Constants.HEADER_IDENTIFICADOR, Constants.TIPO_DOC + Constants.NUMERO_ACCESO);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    using (HttpContent content = response.Content)
                    {

                        respuesta = JsonConvert.DeserializeObject<RespuestaCertificacion>(content.ReadAsStringAsync().Result);


                        try
                        {
                            //d = content.ReadAsStringAsync().Result;
                            DataTable dt = new DataTable("respuesta");
                            dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                            dt.Columns.Add(new DataColumn("fecha", typeof(string)));
                            dt.Columns.Add(new DataColumn("origen", typeof(string)));
                            //dt.Columns.Add(new DataColumn("control_emision", typeof(ControlEmision)));
                            dt.Columns.Add(new DataColumn("alertas_infile", typeof(bool)));
                            //dt.Columns.Add(new DataColumn("descripcion_alertas_infile", typeof(List<string>)));
                            //dt.Columns.Add(new DataColumn("alertas_sat", typeof(bool)));
                            //dt.Columns.Add(new DataColumn("descripcion_alertas_sat", typeof(List<object>)));
                            //dt.Columns.Add(new DataColumn("cantidad_errores", typeof(int)));
                            //// dt.Columns.Add(new DataColumn("descripcion_errores", typeof(List<DescripcionErrores>)));
                            dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
                            //dt.Columns.Add(new DataColumn("informacion_adicional", typeof(string)));
                            dt.Columns.Add(new DataColumn("uuid", typeof(string)));
                            dt.Columns.Add(new DataColumn("serie", typeof(string)));
                            dt.Columns.Add(new DataColumn("numero", typeof(long)));
                            dt.Columns.Add(new DataColumn("xml_certificado", typeof(string)));
                            dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                            DataRow dr = dt.NewRow();
                            dr["resultado"] = respuesta.resultado.ToString();
                            dr["fecha"] = respuesta.fecha;
                            dr["origen"] = respuesta.origen;
                            //dr["control_emision"] = respuesta.control_emision;
                            dr["alertas_infile"] = respuesta.alertas_infile;
                            //dr["descripcion_alertas_infile"] = respuesta.descripcion_alertas_infile;
                            //dr["alertas_sat"] = respuesta.alertas_sat;
                            //dr["descripcion_alertas_sat"] = respuesta.descripcion_alertas_sat;
                            //dr["cantidad_errores"] = respuesta.cantidad_errores;
                            //dr["resultado"] = respuesta.resultado;
                            string text = string.Join(",", respuesta.descripcion_errores);
                            dr["descripcion"] = text;
                            //dr["informacion_adicional"] = respuesta.informacion_adicional;
                            dr["uuid"] = respuesta.uuid;
                            dr["serie"] = respuesta.serie;
                            // dr["numero"] = respuesta.serie;
                            dr["xml_certificado"] = respuesta.xml_certificado;
                            dr["xmlGenerado"] = xmlGenerado;

                            dt.Rows.Add(dr);
                            dataSet.Tables.Add(dt);
                            //dataSet = dsError;
                        }
                        catch (Exception ex)
                        {

                            d = ex.ToString();
                            DataTable dt = new DataTable("resultado");
                            dt.Columns.Add(new DataColumn("resultado", typeof(string)));

                            DataRow dr = dt.NewRow();
                            dr["resultado"] = ex.ToString();
                            dt.Rows.Add(dr);
                            dsError.Tables.Add(dt);
                            dataSet = dsError;
                        }
                    }
                }

            }
            return dataSet;
        }

        #endregion


        #region Funciones Certificacion
        private static DataSet certificacion(String xml, String xmlGenerado)
        {
          /*  Certificacion certificacion = new Certificacion();
            certificacion.nit_emisor = Constants.NIT_EMISOR;
            certificacion.correo_copia = Constants.CORREO_COPIA;
            certificacion.xml_dte = xml;*/
            DataSet dataSet = new DataSet();
            DataSet dsError = new DataSet();
            //pasar a json el objeto
            //string json = JsonConvert.SerializeObject(certificacion);

            RespuestaCertificacion respuesta = new RespuestaCertificacion();
            String d = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.URL_CERTIFICACION_XML);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Constants.METODO_CERTIFICACION_XML);
                request.Headers.Add("UsuarioFirma", Constants.HEADER_USUARIO_TOKEN);
                request.Headers.Add("LlaveFirma", Constants.LLAVE_TOKEN);
                request.Headers.Add("UsuarioApi", Constants.HEADER_USUARIO_TOKEN);
                request.Headers.Add("LlaveApi", Constants.HEADER_LLAVE_EMISOR);
                request.Headers.Add("identificador", Constants.IDENTIFICADOR_DTE);
                //request.Headers.Add("Content-Type", Constants.HEADER_LLAVE_EMISOR);
                request.Headers.Add(Constants.HEADER_IDENTIFICADOR, Constants.IDENTIFICADOR_DTE);
                request.Content = new StringContent(xml, Encoding.UTF8, "application/xml");

                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string str = "";
                        str = content.ReadAsStringAsync().Result.ToString();
                        respuesta = JsonConvert.DeserializeObject<RespuestaCertificacion>(str);

                     //   string str =content.ReadAsStringAsync().Result.ToString();
                        try
                        {

                            DataTable dt = new DataTable("respuesta");
                            dt.Columns.Add(new DataColumn("resultado", typeof(string)));
                            dt.Columns.Add(new DataColumn("fecha", typeof(string)));

                            dt.Columns.Add(new DataColumn("origen", typeof(string)));
                            //dt.Columns.Add(new DataColumn("control_emision", typeof(ControlEmision)));
                            dt.Columns.Add(new DataColumn("alertas_infile", typeof(bool)));
                            //dt.Columns.Add(new DataColumn("descripcion_alertas_infile", typeof(List<string>)));
                            //dt.Columns.Add(new DataColumn("alertas_sat", typeof(bool)));
                            //dt.Columns.Add(new DataColumn("descripcion_alertas_sat", typeof(List<object>)));
                            dt.Columns.Add(new DataColumn("cantidad_errores", typeof(int)));
                            //// dt.Columns.Add(new DataColumn("descripcion_errores", typeof(List<DescripcionErrores>)));
                            dt.Columns.Add(new DataColumn("descripcion", typeof(string)));
                            //dt.Columns.Add(new DataColumn("informacion_adicional", typeof(string)));
                            dt.Columns.Add(new DataColumn("uuid", typeof(string)));
                            dt.Columns.Add(new DataColumn("serie", typeof(string)));
                            dt.Columns.Add(new DataColumn("numero", typeof(string)));
                            dt.Columns.Add(new DataColumn("xml_certificado", typeof(string)));
                            dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                            DataRow dr = dt.NewRow();
                            String fechaEmision = respuesta.fecha;
                            DateTime oDate = Convert.ToDateTime(fechaEmision);
                            LlenarEstructuras.formatDate(oDate);

                            dr["resultado"] = respuesta.resultado.ToString();
                            dr["fecha"] = respuesta.fecha;
                            dr["origen"] = respuesta.origen;
                            //dr["control_emision"] = respuesta.control_emision;
                            dr["alertas_infile"] = respuesta.alertas_infile;
                            //dr["descripcion_alertas_infile"] = respuesta.descripcion_alertas_infile;
                            //dr["alertas_sat"] = respuesta.alertas_sat;
                            //dr["descripcion_alertas_sat"] = respuesta.descripcion_alertas_sat;
                            dr["cantidad_errores"] = respuesta.cantidad_errores;
                            //dr["resultado"] = respuesta.resultado;
                            string text = string.Join(",", respuesta.descripcion_errores);
                            dr["descripcion"] = text;
                            //dr["informacion_adicional"] = respuesta.informacion_adicional;
                            dr["uuid"] = respuesta.uuid;
                            dr["serie"] = respuesta.serie;
                            // dr["numero"] = respuesta.serie;
                            dr["xml_certificado"] = respuesta.xml_certificado;
                            dr["xmlGenerado"] = xml;

                            dt.Rows.Add(dr);
                            dataSet.Tables.Add(dt);
                            //dataSet = dsError;
                        }
                        catch (Exception ex)
                        {
                            DataTable dt = new DataTable("resultado");
                            dt.Columns.Add(new DataColumn("xmlGenerado", typeof(string)));

                            DataRow dr = dt.NewRow();
                            dr["resultado"] = ex.ToString();
                            dr["xmlGenerado"] = xmlGenerado;
                            dt.Rows.Add(dr);
                            dsError.Tables.Add(dt);
                            dataSet = dsError;
                        }
                    }
                }

            }
            return dataSet;
        }
        #endregion
        private static void saveJSon(String json, String name)
        {
            String path = ConfigurationManager.AppSettings["RutaJson"].ToString() + name;
            File.WriteAllText(path, json);
        }

    }


}