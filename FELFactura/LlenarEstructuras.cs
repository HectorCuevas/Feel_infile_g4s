using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using Modelos;

namespace FELFactura
{
    public class LlenarEstructuras
    {
       public static String fecha = "2017-12-12";

        public static void DatosGenerales(DataSet dstcompanyxml, DatosGenerales datosGenerales)
        {

            foreach (DataRow reader in dstcompanyxml.Tables[0].Rows)
            {
                //var vendedor = reader["vendedor"];
                //if (vendedor != null)
                //{
                //    Constants.VENDEDOR = vendedor.ToString();

                //}

                //var pago = reader["condicionpago"];
                //if (pago != null)
                //{
                //    Constants.PAGO = pago.ToString();

                //}

               ///////////////////////////////////

                var CodigoMoneda = reader["codigomoneda"];
                if (CodigoMoneda != null)
                {
                    datosGenerales.CodigoMoneda = CodigoMoneda.ToString();

                }

                var NumeroAcceso = reader["numeroaccesso"];
                if (NumeroAcceso != null)
                {
                    datosGenerales.NumeroAcceso = NumeroAcceso.ToString();
                    Constants.NUMERO_ACCESO = datosGenerales.NumeroAcceso;

                }
                var FechaHoraEmision = reader["FechaHoraEmision"];
                if (FechaHoraEmision != null)
                {
                    datosGenerales.FechaHoraEmision = FechaHoraEmision.ToString();
                    //fecha = FechaHoraEmision.ToString();

                }
                
                var tipo = reader["tipo"];
                if (tipo != null)
                {
                    // datosGenerales.FechaHoraEmision = FechaHoraEmision.ToString();
                    Constants.TIPO_DOC = tipo.ToString();
                    datosGenerales.Tipo = tipo.ToString();

                }
                var identificador = reader["identificadorunico"];
                if (identificador != null)
                {
                    Constants.IDENTIFICADOR_DTE = identificador.ToString();
                }               
            }
        }

        public static void Totales(DataSet dstcompanyxml, Totales totales,List<Item>lst)
        {

            Double impuetos = 0d;
            foreach (DataRow reader in dstcompanyxml.Tables[0].Rows)
            {
                var GranTotal = reader["grantotal"];
                if (GranTotal != null)
                {
                    totales.GranTotal =  GranTotal.ToString();

                }

            }

            if (lst != null)
            {
                foreach (Item item in lst)
                {
                    if (item.impuestos != null)
                    {
                        foreach (Impuesto im in item.impuestos)
                        {
                            if (im.MontoImpuesto != null)
                            {
                                impuetos = impuetos + Double.Parse(im.MontoImpuesto);

                            }
                        }

                    }

                }

            }


            totales.TotalMontoImpuesto = impuetos.ToString();
            totales.NombreCorto = "IVA";

        }

        public static void DatosEmisor(DataSet dstinvoicexml, Emisor emisor)
        {
            foreach (DataRow reader in dstinvoicexml.Tables[0].Rows)
            {
                //este dato hay que preguntarlo
                var AfiliacionIVA = reader["afiliacioniva"];
                if (AfiliacionIVA != null)
                {
                    emisor.AfiliacionIVA = AfiliacionIVA.ToString();

                }
                var CodigoEstablecimiento = reader["codigoestablecimiento"];
                if (CodigoEstablecimiento != null)
                {
                    emisor.CodigoEstablecimiento = CodigoEstablecimiento.ToString();

                }
                var CorreoEmisor = reader["correoemisor"];
                if (CorreoEmisor != null)
                {
                    emisor.CorreoEmisor = CorreoEmisor.ToString();

                }
                else {
                    emisor.CorreoEmisor = "";
                }

                var NITEmisor = reader["nitemisor"];
                if (NITEmisor != null)
                {
                    emisor.NITEmisor = NITEmisor.ToString();

                }
                var NombreComercial = reader["nombrecomercial"];
                if (NombreComercial != null)
                {
                    emisor.NombreComercial = NombreComercial.ToString();

                }
                var NombreEmisor = reader["nombreemisor"];
                if (NombreEmisor != null)
                {
                    emisor.NombreEmisor = NombreEmisor.ToString();

                }
                var Direccion = reader["direccionemisor"];
                if (Direccion != null)
                {
                    emisor.Direccion = Direccion.ToString();

                }
                var CodigoPostal = reader["codigoPostalemisor"];
                if (CodigoPostal != null)
                {
                    emisor.CodigoPostal = CodigoPostal.ToString();

                }
                var Municipio = reader["municipioemisor"];
                if (Municipio != null)
                {
                    emisor.Municipio = Municipio.ToString();

                }
                var Departamento = reader["departamentoemisor"];
                if (Departamento != null)
                {
                    emisor.Departamento = Departamento.ToString();


                }
                var Pais = reader["paisemisor"];
                if (Pais != null)
                {
                    emisor.Pais = Pais.ToString();


                }


            }

        }

        public static void DatosReceptor(DataSet dstinvoicexml, Receptor receptor,DatosGenerales datosGenerales)
        {
            foreach (DataRow reader in dstinvoicexml.Tables[0].Rows)
            {

               
                var CorreoReceptor = reader["correoreceptor"];
                if (CorreoReceptor != null)
                {
                    receptor.CorreoReceptor = CorreoReceptor.ToString();

                }
                var IDReceptor = reader["idreceptor"];
                if (IDReceptor != null)
                {
                    receptor.IDReceptor = IDReceptor.ToString();

                }
                var NombreReceptor = reader["nombrereceptor"];
                if (NombreReceptor != null)
                {
                    receptor.NombreReceptor = NombreReceptor.ToString();
                    Constants.RECEPTOR = NombreReceptor.ToString();

                }

                var Direccion = reader["direccionReceptor"];
                if (Direccion != null)
                {
                    receptor.Direccion = Direccion.ToString();

                }
                var CodigoPostal = reader["codigoPostalReceptor"];
                if (CodigoPostal != null)
                {
                    receptor.CodigoPostal = CodigoPostal.ToString();

                }
                var Municipio = reader["municipioReceptor"];
                if (Municipio != null)
                {
                    receptor.Municipio = Municipio.ToString();

                }
                var Departamento = reader["departamentoReceptor"];
                if (Departamento != null)
                {
                    receptor.Departamento = Departamento.ToString();

                }
                var Pais = reader["paisReceptor"];
                if (Pais != null)
                {
                    receptor.Pais = Pais.ToString();

                }

                var exento = reader["exenta"];
                if (exento != null)
                {       
                    if (exento.ToString() == "SI") {
                        Constants.EXENTA = true;
                    }
                    
                }

            }

        }

        public static void DatosItems(DataSet dstdetailinvoicexml, List<Item> items)
        {
            foreach (DataRow reader in dstdetailinvoicexml.Tables[0].Rows)
            {
                Item item = new Item();
                item.impuestos = new List<Impuesto>();
                Impuesto impuesto = new Impuesto();
                //impuesto
                var impuestonombrecorto = reader["impuestonombrecorto"];
                if (impuestonombrecorto != null)
                {
                    impuesto.NombreCorto = impuestonombrecorto.ToString();

                }
                var codigounidadgravable = reader["codigounidadgravable"];
                if (codigounidadgravable != null)
                {
                    impuesto.CodigoUnidadGravable = codigounidadgravable.ToString();

                }
           
                var montoimpuesto = reader["montoimpuesto"];
                if (montoimpuesto != null)
                {


                    impuesto.MontoImpuesto = montoimpuesto.ToString();
                    
                }
                var montogravable = reader["montogravable"];
                if (montogravable != null)
                {
                    impuesto.MontoGravable =  montogravable.ToString();

                }
                //item en general
                var bienoservicio = reader["bienoservicio"];
                if (bienoservicio != null)
                {
                    item.BienOServicio = bienoservicio.ToString();

                }
                var descripcion = reader["descripcion"];
                if (descripcion != null)
                {
                    item.Descripcion = descripcion.ToString();

                }
                var numerolinea = reader["numerolinea"];
                if (numerolinea != null)
                {
                    item.NumeroLinea = numerolinea.ToString();

                }
                var cantidad = reader["cantidad"];
                if (cantidad != null)
                {
                    item.Cantidad = cantidad.ToString();

                }
                var unidadMedida = reader["unidadMedida"];
                if (unidadMedida != null)
                {
                    item.UnidadMedida = unidadMedida.ToString();

                }
                var precio = reader["precio"];
                if (precio != null)
                {
                    item.Precio =  precio.ToString();

                }
                var preciounitario = reader["preciounitario"];
                if (preciounitario != null)
                {
                    item.PrecioUnitario =  preciounitario.ToString();

                }

                var total = reader["total"];
                if (total != null)
                {
                    item.Total =  total.ToString();

                }

                var descuento = reader["descuento"];
                if (descuento != null )
                {
                    if (Double.Parse(descuento.ToString())>0d)
                    {
                        item.Descuento = descuento.ToString();

                    }

                }
                items.Add(item);
                item.impuestos.Add(impuesto);
            }
        }



        public static void DatosAnular(DataSet dstcancelxml, Anular anular)
        {

            foreach (DataRow reader in dstcancelxml.Tables[0].Rows)
            {
                var NumeroDocumentoAAnular = reader["numerodocumentoanular"];
                var NITEmisor = reader["nitemisor"];
                var IDReceptor = reader["idreceptor"];
                var FechaEmisionDocumentoAnular = reader["fechaemisiondocumentoanular"];
                var FechaHoraAnulacion = reader["fechahoraanulacion"];
                var MotivoAnulacion = reader["motivoanulacion"];
                var uuid = reader["uuid"];

                anular.NumeroDocumentoAAnular = NumeroDocumentoAAnular.ToString();
                anular.NITEmisor = NITEmisor.ToString();
                anular.IDReceptor = IDReceptor.ToString();
                anular.FechaEmisionDocumentoAnular = FechaEmisionDocumentoAnular.ToString();
                anular.FechaHoraAnulacion = FechaHoraAnulacion.ToString();
                anular.MotivoAnulacion = MotivoAnulacion.ToString();
                anular.uuid = uuid.ToString();
                Constants.TIPO_DOC = "ANU";
                string[] numero = NumeroDocumentoAAnular.ToString().Split('-');
                Constants.NUMERO_ACCESO = numero[0];

            }

        }

        public static void DatosNotaCredito(DataSet dstcancelxml, NotaCredito nota) {

            foreach (DataRow reader in dstcancelxml.Tables[0].Rows) {

                //var nombreComplemento = reader["NombreComplemento"];
                var FechaEmisionDocumentoOrigen = reader["FechaEmisionDocumentoOrigen"];
                var motivoAjuste = reader["MotivoAjuste"];
                var NumeroAutorizacionDocumentoOrigen = reader["NumeroAutorizacionDocumentoOrigen"];
                var SerieDocumentoOrigen = reader["SerieDocumentoOrigen"];
                var NumeroDocumentoOrigen = reader["NumeroDocumentoOrigen"];
                var tipoNCRE = reader["Tipo_FE"];

                if (tipoNCRE.ToString() == "GFACE")
                {
                    Constants.isNCREGFACE = true;
                }
                else {
                    Constants.isNCREGFACE = false;
                }

                nota.NumeroAutorizacionDocumentoOrigen = NumeroAutorizacionDocumentoOrigen.ToString();
                nota.SerieDocumentoOrigen = SerieDocumentoOrigen.ToString();
                nota.FechaEmisionDocumentoOrigen = FechaEmisionDocumentoOrigen.ToString();
                nota.nombreComplemento = "ncomp";
                nota.MotivoAjuste = motivoAjuste.ToString();
                nota.IDComplemento = "text";
                nota.NumeroDocumentoOrigen = NumeroDocumentoOrigen.ToString();
            }
        }

        public static void DatosFacturaEspecial(DataSet dstcancelxml, Retenciones retenciones)
        {

            foreach (DataRow reader in dstcancelxml.Tables[0].Rows)
            {
                var RetencionISR = reader["RetencionISR"];
                var RetencionIVA = reader["RetencionIVA"];
                var TotalMenosRetenciones = reader["TotalMenosRetenciones"];
                var TipoEspecial = reader["TipoEspecial"];


                retenciones.RetencionISR = RetencionISR.ToString();
                retenciones.RetencionIVA = RetencionIVA.ToString();
                retenciones.TotalMenosRetenciones = TotalMenosRetenciones.ToString();
                retenciones.TipoEspecial = TipoEspecial.ToString();

            }
        }
        public static void DatosFacturaCambiaria(DataSet dsEnc ,DataSet dstcancelxml, Abonos abonos) {

            foreach (DataRow reader in dsEnc.Tables[0].Rows)
            {
                var FechaHoraVencimiento = reader["FechaVencimiento"];
                if (FechaHoraVencimiento != null)
                {
                    fecha = FechaHoraVencimiento.ToString();
                    DateTime oDate = Convert.ToDateTime(fecha);
                    abonos.FechaVencimiento = formatDate(oDate);
                    // abonos.FechaVencimiento = fec
                    abonos.MontoAbono = "0";
                    abonos.numeroAbono = "1";

                }
               
            }

        }
        public static void DatosFactExportacion(DataSet dstinvoicexml, Complementos c)
        {
            foreach (DataRow reader in dstinvoicexml.Tables[0].Rows)
            {

                var INCOTERM = reader["transporte"];
                if (INCOTERM != null)
                {
                    c.transporte = INCOTERM.ToString();
                }
            }
        }
        public static String formatDate(DateTime oDate) {
            String mes= "", dia = "", newDate = "";
            if (oDate.Month <= 9)
            {
                mes = "0" + oDate.Month.ToString();
            }
            else {
                mes = oDate.Month.ToString();
            }
            if (oDate.Day <= 9)
            {
                dia = "0" + oDate.Day.ToString();
            }
            else {
                dia =oDate.Day.ToString();
            }
            newDate = oDate.Year + "-" + mes + "-" + dia;
            return newDate;
        }
    }
}