﻿using Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace FELFactura
{
    public class XMLNotasAbono : IDocumentRegister
    {

        private DataSet dstinvoicexml = new DataSet();
        private DataSet dstdetailinvoicexml = new DataSet();
        private DatosGenerales datosGenerales = new DatosGenerales();
        private Emisor emisor = new Emisor();
        private NotaCredito nota = new NotaCredito();
        private Receptor receptor = new Receptor();
        private List<Item> items = new List<Item>();
        private Totales totales = new Totales();
        string v_rootxml = "";
        string fac_num = "";
        public String getXML(string XMLInvoice, string XMLDetailInvoce, string fac_num)
        {

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
            LlenarEstructuras.DatosNotaCredito(dstinvoicexml, nota);
        }





        private String getXML()
        {
            XNamespace dte = XNamespace.Get("http://www.sat.gob.gt/dte/fel/0.2.0");
            XNamespace cfc = XNamespace.Get("http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0");
            XNamespace cno = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0");
            XNamespace ds = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
            XNamespace cfe = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0");
            XNamespace cex = XNamespace.Get("http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            //Encabezado del Documento
            XDeclaration declaracion = new XDeclaration("1.0", "utf-8", "no");

            //GTDocumento
            XElement parameters = new XElement(dte + "GTDocumento",
                            new XAttribute(XNamespace.Xmlns + "ds", ds.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "cfc", cfc.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "cno", cno.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "cex", cex.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "dte", dte.NamespaceName),
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
                 new XAttribute("FechaHoraEmision", datosGenerales.FechaHoraEmision), new XAttribute("Tipo", datosGenerales.Tipo));
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
                new XAttribute("IDReceptor", receptor.IDReceptor),
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
            // detalle de factura 
            XElement Items = new XElement(dte + "Items");
            DatosEmision.Add(Items);
            if (items != null)
            {
                foreach (Item item in items)
                {

                    //item
                    XElement Item = new XElement(dte + "Item", new XAttribute("BienOServicio", item.BienOServicio), new XAttribute("NumeroLinea", item.NumeroLinea));
                    XElement Cantidad = new XElement(dte + "Cantidad", item.Cantidad);
                    XElement UnidadMedida = new XElement(dte + "UnidadMedida", item.UnidadMedida);
                    XElement Descripcion = new XElement(dte + "Descripcion", item.Descripcion);
                    XElement PrecioUnitario = new XElement(dte + "PrecioUnitario", item.PrecioUnitario);
                    XElement Precio = new XElement(dte + "Precio", item.Precio);
                    XElement Descuento = new XElement(dte + "Descuento", item.Descuento);
                    XElement TotalItem = new XElement(dte + "Total", item.Total);

                    Item.Add(Cantidad);
                    Item.Add(UnidadMedida);
                    Item.Add(Descripcion);
                    Item.Add(PrecioUnitario);
                    Item.Add(Precio);
                    Item.Add(Descuento);
                    Item.Add(TotalItem);
                    Items.Add(Item);

                }
            }

            //Totales
            XElement Totales = new XElement(dte + "Totales");
            DatosEmision.Add(Totales);

            //total general
            XElement GranTotal = new XElement(dte + "GranTotal", totales.GranTotal);
            Totales.Add(GranTotal);

            //Complementos

            /*    XElement Complementos = new XElement(dte + "Complementos");
                DatosEmision.Add(Complementos);

                XElement Complemento = new XElement(dte + "Complemento",
                     new XAttribute("NombreComplemento", "NOTA DE CREDITO"),
                     new XAttribute("IDComplemento", "ReferenciasNota"),
                     new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0"));
                Complementos.Add(Complemento);

                XElement Referencias = new XElement(cno + "ReferenciasNota"
                    , new XAttribute(XNamespace.Xmlns + "cno", cno)
                    , new XAttribute("FechaEmisionDocumentoOrigen", nota.FechaEmisionDocumentoOrigen)
                    , new XAttribute("MotivoAjuste", nota.MotivoAjuste)
                    , new XAttribute("NumeroAutorizacionDocumentoOrigen", nota.NumeroAutorizacionDocumentoOrigen)
                    , new XAttribute("SerieDocumentoOrigen", nota.SerieDocumentoOrigen)
                    , new XAttribute("Version", "0")

                    );
                Complemento.Add(Referencias);*/

            XDocument myXML = new XDocument(declaracion, parameters);
            String res = myXML.ToString();
            return res;
        }
    }
}