using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using Modelos;

public class wsConnector
{

    private static FELFactura.wsg4s.FactWSFront wsServicio = new FELFactura.wsg4s.FactWSFront();
    private static FELFactura.wsg4s.TransactionTag tt = new FELFactura.wsg4s.TransactionTag();
    
    //

    // Clase que hara la transaccion en el servidor ws especificado
    public static string[] wsEnvio(string cData1, string cData2, string cData3, 
        string cUser, string cUrl, string cRequestor, string cTransaction, string 
        cCountry, string cNit, bool lPdf, string cRutaPdf)
    {

    
        //Variables que haran el retorno
        string[] cResultados = new string[12];
        XmlDocument cFirma = new XmlDocument();
        wsServicio.Url = new Uri(cUrl).ToString();
        string cNombreDocumento = "";
        

        
        try
        {        
            tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor,cUser, cData1, cData2, cData3);
        }
        catch (Exception ex)
        {
            //Variable para controlar Errores
            cResultados[0] = ex.Message.ToString().ToUpper();

          
            cResultados[1] = "False";
            cResultados[2] = "";
            cResultados[3] = "";
            cResultados[4] = "";
            cResultados[5] = "";
            cResultados[6] = "";
            cResultados[7] = "";
            cResultados[8] = "";
            cResultados[9] = "";
            cResultados[10] = "";
            cResultados[11] = "";
             
        }
        try
        {
			
            //Variable para controlar si es true o false
            cResultados[1] = tt.Response.Result.ToString();
            //Es el Response Data
            cResultados[2] = tt.ResponseData.ResponseData1.ToString();
        }
        catch (Exception ex)
        {
            
            cResultados[1] = "False";

            cResultados[2] = "PROBLEMA ENCONTRADO AL TRATAR DE HACER LA COMUNICACIÓN " + ex.Message.ToString();
            cResultados[3] = "";
            cResultados[4] = "";
            cResultados[5] = "";
            cResultados[6] = "";
            cResultados[7] = "";
            cResultados[8] = "";
            cResultados[9] = "";
            cResultados[10] = "";
            cResultados[11] = "";
             
        }

        try
        {

            if (tt.Response.Result == true)
            {
                cResultados[3] = tt.Response.Identifier.Batch.ToString();
                cResultados[4] = tt.Response.Identifier.Serial.ToString();
                cResultados[5] = Base64String_String(tt.ResponseData.ResponseData1);
                cFirma.InnerXml = cResultados[5];
                cResultados[6] = tt.Response.Identifier.DocumentGUID.ToString();

                XmlNodeList cNotasdePie = cFirma.GetElementsByTagName("Nota");



                if (string.IsNullOrEmpty(cNotasdePie[1].InnerText))
                {
                    cResultados[7] = cNotasdePie[1].InnerText;

                }
                else
                {
                    cResultados[7] = cResultados[6];
                }

                if (string.IsNullOrEmpty(cNotasdePie[0].InnerText))
                {
                    cResultados[8] = "";
                }
                else
                {
                    cResultados[8] = cNotasdePie[4].InnerText;
                }

                cResultados[9] = "";//cNotasdePie[2].InnerText; ;
                cResultados[10] = "6001020-7";
                cResultados[11] = "";// cFirma.InnerXml.ToString();
               // cResultados[12] = "";

               // lPdf = true;
                try
                {
                   // if (lPdf == true)
                   // {
                        if (cResultados[7] == "")
                        {
                            cNombreDocumento = cResultados[3] + ".pdf";
                        }
                        else
                        {
                            cNombreDocumento = cResultados[7] + ".pdf";
                        }

                        cRutaPdf = cRutaPdf + cNombreDocumento;
                        System.IO.FileStream oFileStream = new System.IO.FileStream(cRutaPdf, System.IO.FileMode.Create);
                        oFileStream.Write(Base64String_ByteArray(tt.ResponseData.ResponseData3), 0, Base64String_ByteArray(tt.ResponseData.ResponseData3).Length);
                        oFileStream.Close();

                   // }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else
            {
             cResultados[0] = "False";
             cResultados[1] = "False";
             cResultados[2] = tt.Response.Data.ToString();
             cResultados[3] = "";
             cResultados[4] = ""; 
             cResultados[5] = "";
             cResultados[6] = "";
             cResultados[7] = "";
             cResultados[8] = "";
             cResultados[9] = "";
             cResultados[10] = "";
             cResultados[11] = "";           


            
            }
        }
        catch
        {
        }
        return cResultados;
    }


    /// <summary>
    ///  vamos hacer el tema del get document
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>

    // Clase que hara la transaccion en el servidor ws especificado
    public static string[] wsObtener(string cData1, string cData2, string cData3, string cUser, string cUrl, string cRequestor, string cTransaction, string cCountry, string cNit, bool lPdf, string cRutaPdf)
    {
        //Variables que haran el retorno
        string[] cResultados = new string[12];
        XmlDocument cFirma = new XmlDocument();
        wsServicio.Url = new Uri(cUrl).ToString();
        string cNombreDocumento = "";



        try
        {
            tt = wsServicio.RequestTransaction(cRequestor, cTransaction, cCountry, cNit, cRequestor, cUser, cData1, cData2, cData3);
        }
        catch (Exception ex)
        {
            //Variable para controlar Errores
            cResultados[0] = ex.Message.ToString().ToUpper();


            cResultados[1] = "False";
            cResultados[2] = "";
            cResultados[3] = "";
            cResultados[4] = "";
            cResultados[5] = "";
            cResultados[6] = "";
            cResultados[7] = "";
            cResultados[8] = "";
            cResultados[9] = "";
            cResultados[10] = "";
            cResultados[11] = "";

        }
        try
        {
            //Optra general motor gm Eduardo Martinez 
            //Variable para controlar si es true o false
            cResultados[1] = tt.Response.Result.ToString();
            //Es el Response Data
            cResultados[2] = tt.ResponseData.ResponseData1.ToString();
        }
        catch (Exception ex)
        {

            cResultados[1] = "False";

            cResultados[2] = "PROBLEMA ENCONTRADO AL TRATAR DE HACER LA COMUNICACIÓN " + ex.Message.ToString();
            cResultados[3] = "";
            cResultados[4] = "";
            cResultados[5] = "";
            cResultados[6] = "";
            cResultados[7] = "";
            cResultados[8] = "";
            cResultados[9] = "";
            cResultados[10] = "";
            cResultados[11] = "";

        }

        try
        {

            if (tt.Response.Result == true)
            {
                cResultados[3] = tt.Response.Identifier.Batch.ToString();
                cResultados[4] = tt.Response.Identifier.Serial.ToString();
                cResultados[5] = "";
                
                cResultados[6] = tt.Response.Identifier.DocumentGUID.ToString();

                cResultados[10] = "6001020-7";
                cResultados[11] = "";
                cResultados[12] = "";



            }
            else
            {
                cResultados[0] = "False";
                cResultados[1] = "False";
                cResultados[2] = tt.Response.Data.ToString();
                cResultados[3] = "";
                cResultados[4] = "";
                cResultados[5] = "";
                cResultados[6] = "";
                cResultados[7] = "";
                cResultados[8] = "";
                cResultados[9] = "";
                cResultados[10] = "";
                cResultados[11] = "";



            }
        }
        catch
        {
        }
        return cResultados;
    }


    #region Base64
    public static string ByteArray_Base64String(byte[] b)
    {
        return Convert.ToBase64String(b);
    }

    public static byte[] String_ByteArray(string s)
    {
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    public static string String_Base64String(string s)
    {
        return ByteArray_Base64String(String_ByteArray(s));
    }//String_Base64String

    public static string Base64String_String(string b64)
    {
        try
        {
            return ByteArray_String(Base64String_ByteArray(b64));
        }
        catch
        {
            return b64;
        }

    }//Base64String_String

    public static byte[] Base64String_ByteArray(string s)
    {
        return Convert.FromBase64String(s);
    }//Base64String_ByteArray



    public static string ByteArray_String(byte[] b)
    {
        return new string(System.Text.Encoding.UTF8.GetChars(b));
    }//ByteArray_String
    #endregion


}
