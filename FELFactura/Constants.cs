using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FELFactura
{
    public class Constants
    {
        #region Rutas INFILE


        public static string URL_CERTIFICACION_XML = "https://certificador.feel.com.gt/";
        public static string METODO_CERTIFICACION_XML = "fel/procesounificado/transaccion/v2/xml";


        //IDENTIFICADOR PARA LA SOLICITUD DE DOCUMENTOS
        public static String IDENTIFICADOR_DTE = "";

        /*** Certificacion INFILE ***/
        public const String URL_CERTIFICACION_DTE = "https://certificador.feel.com.gt/";
        public const String METODO_CERTIFICACION_DTE = "fel/certificacion/dte";
        public const String HEADER_USUARIO = "usuario";
        public const String HEADER_LLAVE = "llave";
        public const String HEADER_IDENTIFICADOR = "identificador";

        /** Anulaciones infile ***/
        public const String URL_ANULACION_DTE = "https://certificador.feel.com.gt/";
        public const String METODO_ANULACION_DTE = "fel/anulacion/v2/dte";
        public static bool isEXP = false;


        /*** Solitud de firma ***/
        public const String URL_SOLICITUD_FIRMA = "https://signer-emisores.feel.com.gt/";
        public const String METODO_SOLICITUD_FIRMA = "sign_solicitud_firmas/firma_xml";

        //PARA EPIDAURO
        public static String ALIAS = ConfigurationManager.AppSettings["USUARIO"].ToString();
        public static String ES_ANULACION = "N";
        public static String LLAVE_TOKEN = ConfigurationManager.AppSettings["LLAVE_TOKEN"].ToString();
        public static String NIT_EMISOR = ConfigurationManager.AppSettings["NIT_EMISOR"].ToString();
        public static String CORREO_COPIA = ConfigurationManager.AppSettings["CORREO"].ToString();
        //
        public static String HEADER_USUARIO_TOKEN = ConfigurationManager.AppSettings["USUARIO"].ToString();
        public static String HEADER_LLAVE_EMISOR = ConfigurationManager.AppSettings["LLAVE_EMISOR"].ToString();
        public const String HEADER_IDENTIFICADOR_TOKEN = "NDEBEXC2";

        public static String NUMERO_ACCESO = "";
        public static String TIPO_DOC = "";
        public static String RECEPTOR = "";
        public static String PAGO = "";
        public static String VENDEDOR = "";
        public static String TIPO_EXPO = "";
        public static bool isNCREGFACE = false;
        public static bool EXENTA = false;
        #endregion

        #region Rutas Megaprint
        /*** Rutas de megaprint ***/
        public static String URL_SOLICITAR_TOKEN = "https://dev.api.ifacere-fel.com/fel-dte-services/api/solicitarToken";
        public static String URL_REGISTRAR_DOCUMENTO = "https://dev.api.ifacere-fel.com/fel-dte-services/api/registrarDocumentoXML";
        public static String URL_VAIDAR_DOCUMENTO = "https://dev.api.ifacere-fel.com/fel-dte-services/api/validarDocumento";
        public static String URL_ANULAR_DOCUMENTO = "https://dev.api.ifacere-fel.com/fel-dte-services/api/anularDocumentoXML";

        public static String URL_CERTIFICADO = "C:\\certificado\\3225607-61589fea042b4cfc.pfx";
        public static String URL_CERTIFICADO_CONTRASENIA = "MayaZac/49";

        public static String UBICACION_XML_FACTURA = "C:\\Users\\leyla\\Dropbox\\universidad\\prosisco\\";
        public static String FRASE_2 = "1";
        public static String FRASE_1 = "1";
        public static String FRASE_CODIGO_2 = "2";
        public static String FRASE_CODIGO_1 = "1";
        public static String TIPO_FACTURA = "FACT";
        public static String TIPO_FACTURA_CAMBIARIA = "FCAM";
        public static String TIPO_FACTURA_PEQUENIO_CONTRIBUYENTE = "FPEQ";
        public static String TIPO_FACTURA_ESPECIAL = "FESP";
        public static String TIPO_NOTA_ABONO = "NABN";
        public static String TIPO_RECIBO_DONACION = "RDON";
        public static String TIPO_RECIBO = "RECI";
        public static String TIPO_NOTA_DEBITO = "NDEB";
        public static String TIPO_NOTA_CREDITO = "NCRE";
        #endregion

    }
}