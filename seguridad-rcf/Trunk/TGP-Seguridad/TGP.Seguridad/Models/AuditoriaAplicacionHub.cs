using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.IO;

namespace TGP.Seguridad.Models
{
    public enum TipoArchivoAuditoriaAplicacion {
        Error, Debug
    };


    public class AuditoriaAplicacion
    {
        public string Descripcion { get; set; }
        //public int MyProperty { get; set; }
    }

    [HubName("auditoriaAplicacionHub")]
    public class AuditoriaAplicacionHub:Hub
    {

        public void ObtenerLog(string tipoArchivo, string codigoEstructura)
        {
            string filename = string.Empty;
            //Busco el archivo de log a leer por su tipo
            if (tipoArchivo == "ERROR")
                filename = Helpers.XMLHelper.ObtenerUbicacionArchivoFromXml(codigoEstructura, TipoArchivoAuditoriaAplicacion.Error);
            else if (tipoArchivo == "DEBUG")
                filename = Helpers.XMLHelper.ObtenerUbicacionArchivoFromXml(codigoEstructura, TipoArchivoAuditoriaAplicacion.Debug);

            Start:

            try
            {
                using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    //start at the end of the file
                    long lastMaxOffset = reader.BaseStream.Length;

                    while (true)
                    {
                        System.Threading.Thread.Sleep(100);

                        //if the file size has not changed, idle
                        if (reader.BaseStream.Length == lastMaxOffset)
                            continue;

                        //seek to the last max offset
                        reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                        //read out of the file until the EOF
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                            this.Clients.Caller.logEvent(DateTime.Now.ToLongTimeString(), "info", line);

                        //update the last max offset
                        lastMaxOffset = reader.BaseStream.Position;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                //prompt user to restart
                Console.Write("Would you like to try re-opening the file? Y/N:");
                if (Console.ReadLine().ToUpper() == "Y")
                    goto Start;
            }
        }

    }
}