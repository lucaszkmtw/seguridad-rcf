using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using TGP.Seguridad.Models;

namespace TGP.Seguridad.Helpers
{
    public class XMLHelper
    {

        /// <summary>
        /// Metodo que retorna la ubicacion del archivo log
        /// </summary>
        /// <param name="codigoEstructuraFuncional">Codigo de la estructura funcional</param>
        /// <param name="tipoArchivo">Tipo de archivo si es BUG o Error</param>
        /// <returns></returns>
        internal static string ObtenerUbicacionArchivoFromXml(string codigoEstructuraFuncional, TipoArchivoAuditoriaAplicacion tipoArchivo)
        {
           
            //Load the XML file in XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.Load(System.Configuration.ConfigurationManager.AppSettings["AuditoriaAplicacionXmlFuente"]);

            //Recupero el nodo de la aplicacion
            var nodeSingle = doc.SelectSingleNode("/Aplicaciones/Aplicacion[@Id='" + codigoEstructuraFuncional.ToUpper() + "']");

            if (nodeSingle != null)
            {
                if (tipoArchivo == TipoArchivoAuditoriaAplicacion.Debug)
                    //Fetch the Node and Attribute values.
                    return nodeSingle["ArchivoDebug"].InnerText;
                else
                    return nodeSingle["ArchivoError"].InnerText;
            }
            else
                return null;
        }

        ///<summary>Metodo que obtiene .</summary>
        ///<param name="reader">The reader to read from.</param>
        ///<param name="cantidadLineas">The number of lines to return.</param>
        ///<returns>The last lneCount lines from the reader.</returns>
        internal static string[] ObtenerLineasLog(TextReader reader, int cantidadLineas)
        {
            var buffer = new List<string>(cantidadLineas);
            string line;
            for (int i = 0; i < cantidadLineas; i++)
            {
                line = reader.ReadLine();
                if (line == null) return buffer.ToArray();
                buffer.Add(line);
            }
            int lastLine = cantidadLineas - 1;
            while (null != (line = reader.ReadLine()))
            {
                lastLine++;
                if (lastLine == cantidadLineas) lastLine = 0;
                buffer[lastLine] = line;
            }
            if (lastLine == cantidadLineas - 1) return buffer.ToArray();
            var retVal = new string[cantidadLineas];
            buffer.CopyTo(lastLine + 1, retVal, 0, cantidadLineas - lastLine - 1);
            buffer.CopyTo(0, retVal, cantidadLineas - lastLine - 1, lastLine + 1);
            return retVal;
        }


    }
}