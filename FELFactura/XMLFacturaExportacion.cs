using Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
namespace FELFactura
{
    public class XMLFacturaExportacion : IDocumentRegister
    {

        private DataSet dstinvoicexml = new DataSet();
        private DataSet dstdetailinvoicexml = new DataSet();
        private DatosGenerales datosGenerales = new DatosGenerales();
        private Complementos complementos = new Complementos();
        private Abonos abonos = new Abonos();
        private Emisor emisor = new Emisor();
        private Receptor receptor = new Receptor();
        private List<Item> items = new List<Item>();
        private Totales totales = new Totales();
        string v_rootxml = "";
        string fac_num = "";
        public String getXML(string XMLInvoice, string XMLDetailInvoce, string path, string fac_num)
        {

            v_rootxml = path;
            this.fac_num = fac_num;
            //convertir a dataset los string para mayor manupulacion
            XmlToDataSet(XMLInvoice, XMLDetailInvoce);
            //llenar estructuras
            ReaderDataset();
            //armar xml
            return getXML();
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
                ex.GetBaseException();
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
            LlenarEstructuras.DatosFactExportacion(dstinvoicexml, complementos);
            if (Constants.TIPO_DOC == "FCAM") {
                LlenarEstructuras.DatosFacturaCambiaria(dstinvoicexml, dstdetailinvoicexml, abonos);
            }
        }





        private String getXML()
        {
            Boolean exenta = false;
            XNamespace dte = XNamespace.Get("http://www.sat.gob.gt/dte/fel/0.2.0");
            XNamespace xd = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
            XNamespace ns = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace cfc = XNamespace.Get("http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0");
            XNamespace cex = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0");

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
            XElement DatosGenerales = new XElement(dte + "DatosGenerales", new XAttribute("CodigoMoneda", datosGenerales.CodigoMoneda),
                new XAttribute("Exp", "SI"),
                 new XAttribute("FechaHoraEmision", datosGenerales.FechaHoraEmision), new XAttribute("Tipo", datosGenerales.Tipo +
                 ""));
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
            XElement Receptor = new XElement(dte + "Receptor", new XAttribute("CorreoReceptor", receptor.CorreoReceptor),
                new XAttribute("IDReceptor", "CF"),
                new XAttribute("NombreReceptor", receptor.NombreReceptor));
            DatosEmision.Add(Receptor);
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

            //frases
            XElement Frases = new XElement(dte + "Frases");
            DatosEmision.Add(Frases);

            XElement Frase1 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "1"), new XAttribute("TipoFrase", "1"));
            Frases.Add(Frase1);

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

                            if ("2".Equals(im.CodigoUnidadGravable))
                            {
                                exenta = true;

                            }
                        }
                    }
                }
            }

            if (exenta)
            {
                XElement Frase3 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "1"), new XAttribute("TipoFrase", "4"));
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

            //si es FCAM




            //total general
            XElement GranTotal = new XElement(dte + "GranTotal", totales.GranTotal);
            Totales.Add(GranTotal);

            XElement Complementos = new XElement(dte + "Complementos");
            DatosEmision.Add(Complementos);
            XElement Complemento = new XElement(dte + "Complemento", 
                new XAttribute("NombreComplemento", "Exportacion")
                , new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0"));
            Complementos.Add(Complemento);

            XElement Exportacion = new XElement(cex + "Exportacion",
                new XAttribute(XNamespace.Xmlns + "cex", ns.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),

                                              new XAttribute("Version", "1")
                   );
            Complemento.Add(Exportacion);



            if (Constants.TIPO_DOC == "FCAM")
            {
                XElement ComplementoAbonos = new XElement(dte + "Complemento",
               new XAttribute("NombreComplemento", "Cambiaria")
               , new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0"));
                Complementos.Add(ComplementoAbonos);

                XElement AbonosFactura = new XElement(cfc + "AbonosFacturaCambiaria"
                          , new XAttribute("Version", "1")
                          , new XAttribute(XNamespace.Xmlns + "cfc", cfc)
                          );
                ComplementoAbonos.Add(AbonosFactura);

                XElement abono = new XElement(cfc + "Abono",
                     new XElement(cfc + "NumeroAbono", abonos.numeroAbono),
                    new XElement(cfc + "FechaVencimiento", abonos.FechaVencimiento),
                    new XElement(cfc + "MontoAbono", abonos.MontoAbono)
                    );
                AbonosFactura.Add(abono);
            }


            XElement NombreConsignatarioODestinatario = new XElement(cex + "NombreConsignatarioODestinatario", receptor.NombreReceptor);
            XElement DireccionConsignatarioODestinatario = new XElement(cex + "DireccionConsignatarioODestinatario", receptor.Direccion);
            XElement CodigoConsignatarioODestinatario = new XElement(cex + "CodigoConsignatarioODestinatario", receptor.IDReceptor);
            XElement NombreComprador = new XElement(cex + "NombreComprador", receptor.NombreReceptor);
            XElement DireccionComprador = new XElement(cex + "DireccionComprador", receptor.Direccion);
            XElement CodigoComprador = new XElement(cex + "CodigoComprador", receptor.IDReceptor);
            XElement OtraReferencia = new XElement(cex + "OtraReferencia","NO HAY");
            XElement INCOTERM = new XElement(cex + "INCOTERM", complementos.transporte);
            XElement NombreExportador = new XElement(cex + "NombreExportador", "EPIDAURO, S.A.");
            XElement CodigoExportador = new XElement(cex + "CodigoExportador", "E23556");
            Exportacion.Add(NombreConsignatarioODestinatario);
            Exportacion.Add(DireccionConsignatarioODestinatario);
            Exportacion.Add(CodigoConsignatarioODestinatario);
            Exportacion.Add(NombreComprador);
            Exportacion.Add(DireccionComprador);
            Exportacion.Add(CodigoComprador);
            Exportacion.Add(OtraReferencia);
            Exportacion.Add(INCOTERM);
            Exportacion.Add(NombreExportador);
            Exportacion.Add(CodigoExportador);

            XDocument myXML = new XDocument(declaracion, parameters);
            String res = myXML.ToString();


            return res;
        }
    }
}