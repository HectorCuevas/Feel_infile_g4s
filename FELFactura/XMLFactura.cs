using Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace FELFactura
{
    public class XMLFactura
    {

        bool isFCAM = false, isNCRE = false, isNDEB = false;
        private DataSet dstinvoicexml = new DataSet();
        private DataSet dstdetailinvoicexml = new DataSet();
        private DatosGenerales datosGenerales = new DatosGenerales();
        private Emisor emisor = new Emisor();
        private Receptor receptor = new Receptor();
        private List<Item> items = new List<Item>();
        private Totales totales = new Totales();
        private Complementos complementos = new Complementos();
        private Abonos abonos = new Abonos();
        private NotaCredito nota = new NotaCredito();
        private Retenciones retenciones = new Retenciones();
        string v_rootxml = "";
        string fac_num = "";
        public String getXML(string XMLInvoice, string XMLDetailInvoce, string path, string fac_num)
        {
            String xml = "";
            v_rootxml = path;
            this.fac_num = fac_num;
            //convertir a dataset los string para mayor manupulacion
            XmlToDataSet(XMLInvoice, XMLDetailInvoce);
            //llenar estructuras
            TipoDocumento tipoDocumento = new TipoDocumento();
            tipoDocumento.getTipo(dstinvoicexml);
            ReaderDataset();
            if (Constants.TIPO_DOC == "NABN")
            {
                XMLNotasAbono xMLNotasAbono = new XMLNotasAbono();
                xml = xMLNotasAbono.getXML(XMLInvoice, XMLDetailInvoce, "123546", fac_num);
            }
            else
            if (Constants.TIPO_DOC == "FCAM" || Constants.TIPO_DOC == "FACT")
            {
                if (Constants.TIPO_EXPO == "SI")
                {
                    XMLFacturaExportacion xMLFacturaExportacion = new XMLFacturaExportacion();
                    xml = xMLFacturaExportacion.getXML(XMLInvoice, XMLDetailInvoce, path, fac_num);
                }
                else
                {
                    xml = getXML();
                }
            }
            else
            {
                xml = getXML();
            }

            //armar xml
            return xml;
        }


        //Convertir XML a DataSet
        private bool XmlToDataSet(string XMLInvoice, string XMLDetailInvoce)
        {
            try
            {

                //Convieriendo XMl a DataSet Factura
                System.IO.StringReader rdinvoice = new System.IO.StringReader(XMLInvoice);
                dstinvoicexml.ReadXml(rdinvoice);

                //Convieritiendo XML a DataSet Detalle Factura
                System.IO.StringReader rddetailinvoice = new System.IO.StringReader(XMLDetailInvoce);
                dstdetailinvoicexml.ReadXml(rddetailinvoice);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //Lectura de Documentos
        private void ReaderDataset()
        {

            LlenarEstructuras.DatosGenerales(dstinvoicexml, datosGenerales);
            LlenarEstructuras.DatosEmisor(dstinvoicexml, emisor);
            LlenarEstructuras.DatosReceptor(dstinvoicexml, receptor, datosGenerales);
            LlenarEstructuras.DatosItems(dstdetailinvoicexml, items);
            LlenarEstructuras.Totales(dstinvoicexml, totales, items);
            // LlenarEstructuras.DatosFacturaCambiaria(dstdetailinvoicexml, abonos);
            switch (Constants.TIPO_DOC)
            {
                case "FCAM":
                    LlenarEstructuras.DatosFacturaCambiaria(dstinvoicexml, dstdetailinvoicexml, abonos);
                    isFCAM = true;
                    break;
                case "NCRE":
                    LlenarEstructuras.DatosNotaCredito(dstinvoicexml, nota);
                    isNCRE = true;
                    break;
                case "NDEB":
                    LlenarEstructuras.DatosNotaCredito(dstinvoicexml, nota);
                    isNCRE = true;
                    isNDEB = true;
                    break;
                case "NABN":
                    LlenarEstructuras.DatosNotaCredito(dstinvoicexml, nota);
                    isNCRE = true;
                    isNDEB = true;
                    break;
                case "FESP":
                    LlenarEstructuras.DatosFacturaEspecial(dstinvoicexml, retenciones);
                    break;
            }


            //LlenarEstructuras.DatosNotaCredito(dstdetailinvoicexml, complementos);
        }





        private String getXML()
        {
            XNamespace dte = XNamespace.Get("http://www.sat.gob.gt/dte/fel/0.2.0");
            XNamespace cfc = XNamespace.Get("http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0");
            XNamespace cno = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0");
            XNamespace xd = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
            XNamespace cfe = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0");
            //Encabezado del Documento
            XDeclaration declaracion = new XDeclaration("1.0", "utf-8", "no");

            //GTDocumento
            XElement parameters = new XElement(dte + "GTDocumento",
                            new XAttribute(XNamespace.Xmlns + "dte", dte.NamespaceName),
                           new XAttribute(XNamespace.Xmlns + "xd", xd.NamespaceName),
                           new XAttribute("Version", "0.1"));
            //SAT
            XElement SAT = new XElement(dte + "SAT", new XAttribute("ClaseDocumento", "dte"));
            parameters.Add(SAT);

            // formando dte
            XElement DTE = new XElement(dte + "DTE", new XAttribute("ID", "DatosCertificados"));
            SAT.Add(DTE);

            //datos de emision
            XElement DatosEmision = new XElement(dte + "DatosEmision", new XAttribute("ID", "DatosEmision"));
            DTE.Add(DatosEmision);

            //datos generales
            XElement DatosGenerales = new XElement(dte + "DatosGenerales", new XAttribute("CodigoMoneda", datosGenerales.CodigoMoneda)
                /// decomentar esta linea para agregar numero de acceso
                , new XAttribute("FechaHoraEmision", datosGenerales.FechaHoraEmision)
                 //, new XAttribute("NumeroAcceso", this.datosGenerales.NumeroAcceso)
                 , new XAttribute("Tipo", datosGenerales.Tipo));
            DatosEmision.Add(DatosGenerales);

            //datos emisor
            XElement Emisor = new XElement(dte + "Emisor", new XAttribute("AfiliacionIVA", emisor.AfiliacionIVA),
                new XAttribute("CodigoEstablecimiento", emisor.CodigoEstablecimiento),
                new XAttribute("CorreoEmisor", emisor.CorreoEmisor), new XAttribute("NITEmisor", emisor.NITEmisor),
                new XAttribute("NombreComercial", emisor.NombreComercial), new XAttribute("NombreEmisor", emisor.NombreEmisor));
            DatosEmision.Add(Emisor);
            //direccion del emisor
            XElement DireccionEmisor = new XElement(dte + "DireccionEmisor");
            Emisor.Add(DireccionEmisor);
            //elementos dentro de direccion de emisor, dirección, codigopostal, municipio, departamento, pais
            XElement Direccion = new XElement(dte + "Direccion", emisor.Direccion);
            XElement CodigoPostal = new XElement(dte + "CodigoPostal", emisor.CodigoPostal);
            XElement Municipio = new XElement(dte + "Municipio", emisor.Municipio);
            XElement Departamento = new XElement(dte + "Departamento", emisor.Departamento);
            XElement Pais = new XElement(dte + "Pais", emisor.Pais);
            DireccionEmisor.Add(Direccion);
            DireccionEmisor.Add(CodigoPostal);
            DireccionEmisor.Add(Municipio);
            DireccionEmisor.Add(Departamento);
            DireccionEmisor.Add(Pais);

            //datos Receptor
            XElement Receptor = null;
            if (Constants.TIPO_DOC == "FESP")
            {
                 Receptor = new XElement(dte + "Receptor", new XAttribute("CorreoReceptor", receptor.CorreoReceptor),
               new XAttribute("IDReceptor", receptor.IDReceptor),
               new XAttribute("NombreReceptor", receptor.NombreReceptor),
               new XAttribute("TipoEspecial", retenciones.TipoEspecial));
                DatosEmision.Add(Receptor);
            }
            else
            {
                 Receptor = new XElement(dte + "Receptor", new XAttribute("CorreoReceptor", receptor.CorreoReceptor),
                  new XAttribute("IDReceptor", receptor.IDReceptor),
                  new XAttribute("NombreReceptor", receptor.NombreReceptor));
                DatosEmision.Add(Receptor);

            }

           
            //direccion del receptor
            XElement DireccionReceptor = new XElement(dte + "DireccionReceptor");
            Receptor.Add(DireccionReceptor);
            //elementos dentro de direccion de emisor, dirección, codigopostal, municipio, departamento, pais
            XElement DireccionRecp = new XElement(dte + "Direccion", receptor.Direccion);
            XElement CodigoPostalReceptor = new XElement(dte + "CodigoPostal", receptor.CodigoPostal);
            XElement MunicipioReceptor = new XElement(dte + "Municipio", receptor.Municipio);
            XElement DepartamentoReceptor = new XElement(dte + "Departamento", receptor.Departamento);
            XElement PaisReceptor = new XElement(dte + "Pais", receptor.Pais);
            DireccionReceptor.Add(DireccionRecp);
            DireccionReceptor.Add(CodigoPostalReceptor);
            DireccionReceptor.Add(MunicipioReceptor);
            DireccionReceptor.Add(DepartamentoReceptor);
            DireccionReceptor.Add(PaisReceptor);

            XElement Frases = null;
            if (Constants.TIPO_DOC == "FACT" || Constants.TIPO_DOC == "FCAM")
            {
                if (Constants.EXENTA)
                {
                    //frases
                    Frases = new XElement(dte + "Frases");
                    DatosEmision.Add(Frases);
                    XElement Frase1 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "2"), new XAttribute("TipoFrase", "1"));
                    Frases.Add(Frase1);
                }
                else
                {
                    //frases
                    Frases = new XElement(dte + "Frases");
                    DatosEmision.Add(Frases);
                    XElement Frase1 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "1"), new XAttribute("TipoFrase", "1"));
                    Frases.Add(Frase1);
                }
            }

            // detalle de factura 

            XElement Items = new XElement(dte + "Items");
            DatosEmision.Add(Items);
            if (items != null)
            {
                foreach (Item item in items)
                {
                    //Items


                    //item
                    XElement Item = new XElement(dte + "Item", new XAttribute("BienOServicio", item.BienOServicio), new XAttribute("NumeroLinea", item.NumeroLinea));
                    XElement Cantidad = new XElement(dte + "Cantidad", item.Cantidad);
                    XElement UnidadMedida = new XElement(dte + "UnidadMedida", item.UnidadMedida);
                    XElement Descripcion = new XElement(dte + "Descripcion", item.Descripcion);
                    XElement PrecioUnitario = new XElement(dte + "PrecioUnitario", item.PrecioUnitario);
                    XElement Precio = new XElement(dte + "Precio", item.Precio);
                    XElement Descuento = new XElement(dte + "Descuento", item.Descuento);
                    XElement TotalItem = new XElement(dte + "Total", item.Total);
                    //impuestos
                    XElement Impuestos = new XElement(dte + "Impuestos");

                    Item.Add(Cantidad);
                    Item.Add(UnidadMedida);
                    Item.Add(Descripcion);
                    Item.Add(PrecioUnitario);
                    Item.Add(Precio);
                    Item.Add(Descuento);
                    Item.Add(Impuestos);
                    Item.Add(TotalItem);
                    Items.Add(Item);



                    //impuesto por item
                    if (item.impuestos != null)
                    {
                        foreach (Impuesto im in item.impuestos)
                        {
                            XElement Impuesto = new XElement(dte + "Impuesto");
                            XElement NombreCorto = new XElement(dte + "NombreCorto", im.NombreCorto);
                            XElement CodigoUnidadGravable = new XElement(dte + "CodigoUnidadGravable", im.CodigoUnidadGravable);
                            XElement MontoGravable = new XElement(dte + "MontoGravable", im.MontoGravable);
                            //  XElement CantidadUnidadesGravables = new XElement(dte + "CantidadUnidadesGravables", im.CantidadUnidadesGravables);
                            XElement MontoImpuesto = new XElement(dte + "MontoImpuesto", im.MontoImpuesto);
                            Impuesto.Add(NombreCorto);
                            Impuesto.Add(CodigoUnidadGravable);
                            Impuesto.Add(MontoGravable);
                            //Impuesto.Add(CantidadUnidadesGravables);
                            Impuesto.Add(MontoImpuesto);
                            Impuestos.Add(Impuesto);
                        }
                    }
                }
            }
            if (Constants.EXENTA && Constants.TIPO_DOC == "FACT")
            {
                XElement Frase3 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "10"), new XAttribute("TipoFrase", "4"));
                Frases.Add(Frase3);

            }
            //Totales
            XElement Totales = new XElement(dte + "Totales");
            DatosEmision.Add(Totales);

            //total impuestos
            XElement TotalImpuestos = new XElement(dte + "TotalImpuestos");
            XElement TotalImpuesto = new XElement(dte + "TotalImpuesto", new XAttribute("NombreCorto", totales.NombreCorto), new XAttribute("TotalMontoImpuesto", totales.TotalMontoImpuesto));
            TotalImpuestos.Add(TotalImpuesto);
            Totales.Add(TotalImpuestos);

            //total general
            XElement GranTotal = new XElement(dte + "GranTotal", totales.GranTotal);
            Totales.Add(GranTotal);

            /* XElement Adendas = new XElement(dte + "Adenda",
                      new XElement("CodCliente", Constants.NUMERO_ACCESO)
                      , new XElement("NomCliente", Constants.RECEPTOR)
                      , new XElement("RazonSocCliente", Constants.RECEPTOR)
                      , new XElement("Vendedor", Constants.VENDEDOR)
                      , new XElement("Pedido", "")
                      , new XElement("CondicionPago", Constants.PAGO)
                 );
             SAT.Add(Adendas);*/



            switch (Constants.TIPO_DOC)
            {
                case "NCRE":
                    {
                        XElement Complementos = new XElement(dte + "Complementos");
                        DatosEmision.Add(Complementos);

                        XElement Complemento = new XElement(dte + "Complemento",
                             new XAttribute("NombreComplemento", "ncomp"),
                             new XAttribute("IDComplemento", "1"),
                             new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0"));
                        Complementos.Add(Complemento);

                        if (Constants.isNCREGFACE)
                        {
                            XElement Referencias = new XElement(cno + "ReferenciasNota"
                           , new XAttribute(XNamespace.Xmlns + "cno", cno)
                           , new XAttribute("NumeroDocumentoOrigen", nota.NumeroDocumentoOrigen)
                           , new XAttribute("RegimenAntiguo", "Antiguo")
                           , new XAttribute("FechaEmisionDocumentoOrigen", nota.FechaEmisionDocumentoOrigen)
                           , new XAttribute("MotivoAjuste", nota.MotivoAjuste)
                           , new XAttribute("NumeroAutorizacionDocumentoOrigen", nota.NumeroAutorizacionDocumentoOrigen)
                           , new XAttribute("SerieDocumentoOrigen", nota.SerieDocumentoOrigen)
                           , new XAttribute("Version", "0")

                           );
                            Complemento.Add(Referencias);
                        }
                        else
                        {
                            XElement Referencias = new XElement(cno + "ReferenciasNota"
                               , new XAttribute(XNamespace.Xmlns + "cno", cno)
                               , new XAttribute("FechaEmisionDocumentoOrigen", nota.FechaEmisionDocumentoOrigen)
                               , new XAttribute("MotivoAjuste", nota.MotivoAjuste)
                               , new XAttribute("NumeroAutorizacionDocumentoOrigen", nota.NumeroAutorizacionDocumentoOrigen)
                               , new XAttribute("SerieDocumentoOrigen", nota.SerieDocumentoOrigen)
                               , new XAttribute("Version", "0")

                               );
                            Complemento.Add(Referencias);
                        }
                    }
                    break;
                case "FCAM":
                    {
                        XElement Complementos = new XElement(dte + "Complementos");
                        DatosEmision.Add(Complementos);

                        XElement Complemento = new XElement(dte + "Complemento",
                             new XAttribute("NombreComplemento", "ncomp"),
                             new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoFacturaCambiaria/0.1.0"));
                        Complementos.Add(Complemento);

                        XElement AbonosFactura = new XElement(cfc + "AbonosFacturaCambiaria"
                            , new XAttribute("Version", "1")
                            , new XAttribute(XNamespace.Xmlns + "cfc", cfc)
                            );
                        Complemento.Add(AbonosFactura);

                        XElement abono = new XElement(cfc + "Abono",
                             new XElement(cfc + "NumeroAbono", abonos.numeroAbono),
                            new XElement(cfc + "FechaVencimiento", abonos.FechaVencimiento),
                            new XElement(cfc + "MontoAbono", abonos.MontoAbono)
                            );
                        AbonosFactura.Add(abono);
                        break;
                    }
                case "NDEB":

                    {
                        XElement Complementos = new XElement(dte + "Complementos");
                        DatosEmision.Add(Complementos);

                        XElement Complemento = new XElement(dte + "Complemento",
                             new XAttribute("NombreComplemento", "ncomp"),
                             new XAttribute("IDComplemento", "1"),
                             new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoFacturaCambiaria/0.1.0"));
                        Complementos.Add(Complemento);

                        XElement Referencias = new XElement(cno + "ReferenciasNota"
                            , new XAttribute(XNamespace.Xmlns + "cno", cno)
                            , new XAttribute("FechaEmisionDocumentoOrigen", nota.FechaEmisionDocumentoOrigen)
                            , new XAttribute("MotivoAjuste", nota.MotivoAjuste)
                            , new XAttribute("NumeroAutorizacionDocumentoOrigen", nota.NumeroAutorizacionDocumentoOrigen)
                            , new XAttribute("SerieDocumentoOrigen", nota.SerieDocumentoOrigen)
                            , new XAttribute("Version", "0")

                            );
                        Complemento.Add(Referencias);
                    }
                    break;
             /*   case "NABN":

                    {
                        XElement Complementos = new XElement(dte + "Complementos");
                        DatosEmision.Add(Complementos);

                        XElement Complemento = new XElement(dte + "Complemento",
                             new XAttribute("NombreComplemento", "ncomp"),
                             new XAttribute("IDComplemento", "1"),
                             new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoFacturaCambiaria/0.1.0"));
                        Complementos.Add(Complemento);

                        XElement Referencias = new XElement(cno + "ReferenciasNota"
                            , new XAttribute(XNamespace.Xmlns + "cno", cno)
                            , new XAttribute("FechaEmisionDocumentoOrigen", nota.FechaEmisionDocumentoOrigen)
                            , new XAttribute("MotivoAjuste", nota.MotivoAjuste)
                            , new XAttribute("NumeroAutorizacionDocumentoOrigen", nota.NumeroAutorizacionDocumentoOrigen)
                            , new XAttribute("SerieDocumentoOrigen", nota.SerieDocumentoOrigen)
                            , new XAttribute("Version", "0")

                            );
                        Complemento.Add(Referencias);
                    }
                    break;*/
                case "FESP":
                    {
                        XElement Complementos = new XElement(dte + "Complementos");
                        DatosEmision.Add(Complementos);

                        XElement Complemento = new XElement(dte + "Complemento",
                             new XAttribute("NombreComplemento", "text"),
                             new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0"));
                        Complementos.Add(Complemento);

                        XElement RetencionFacturaEsp = new XElement(cfe + "RetencionesFacturaEspecial"
                            , new XAttribute("Version", "1")
                            , new XAttribute(XNamespace.Xmlns + "cfe", cfe)
                            , new XElement(cfe + "RetencionISR", retenciones.RetencionISR)
                            , new XElement(cfe + "RetencionIVA", retenciones.RetencionIVA)
                            , new XElement(cfe + "TotalMenosRetenciones", retenciones.TotalMenosRetenciones)
                            );
                        Complemento.Add(RetencionFacturaEsp);
                        break;
                    }
            }
            XDocument myXML = new XDocument(declaracion, parameters);
            String res = myXML.ToString();


            return res;
        }
    }
}