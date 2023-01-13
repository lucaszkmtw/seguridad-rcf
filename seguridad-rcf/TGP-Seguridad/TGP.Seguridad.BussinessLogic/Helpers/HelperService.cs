using System;
using System.Globalization;
using System.Xml;

namespace TGP.Seguridad.BussinessLogic.Helpers
{
    /// <summary>
    /// Clase de soporte que se utiliza para este caso exponer el metodo de Utils.DateUtilsService
    /// para obtener la fecha y hora de la base.
    /// </summary>
    public class HelperService
    {

        #region // Variables //

        /// <summary>
        /// Encapsulamiento interno del repositorio.
        /// </summary>
        private static DataAccess.Helpers.HelperService service = DataAccess.Helpers.HelperService.Instance;
        /// <summary>
        /// Instancia privada de la clase
        /// </summary>
        private static HelperService instance;
        //private Utils.DateUtilsService dateUtils;


        public static HelperService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HelperService();
                }
                return instance;
            }
        }



        #endregion

        #region // Constructores //
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        HelperService() { }
        #endregion
        /// <summary>
        /// Metodo el color css de la aplicacion
        /// </summary>
        /// <param name="codigoEstructuraFuncional">Codigo de la estructura funcional</param>
        /// <returns></returns>
        /// 

        internal static string ObtenerCssColorDeApliacion(string codigoEstructuraFuncional)
        {
            var c = codigoEstructuraFuncional;
            //Load the XML file in XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.Load(System.Configuration.ConfigurationManager.AppSettings["AplicacionXmlFuente"]);
            if (c != null)
            {


                //Recupero el nodo de la aplicacion
                var nodeSingle = doc.SelectSingleNode("/Aplicaciones/Aplicacion[@Id='" + codigoEstructuraFuncional.ToUpper() + "']");

                if (nodeSingle != null)
                {
                    return nodeSingle["CssColor"].InnerText;
                }
                else
                    return string.Empty;
            }
            else // es el portal
            {
                //Recupero el nodo de la aplicacion
                var nodeSingle = doc.SelectSingleNode("/Aplicaciones/Aplicacion[@Id='POR']");

                if (nodeSingle != null)
                {
                    return nodeSingle["CssColor"].InnerText;
                }
                else
                    return string.Empty;
            }

        }

        #region // Metodos DateUtils  //

        public static DateTime GetDateToday()
        {
            return service.GetDateToday();
        }

        public static String GetShortDateTodayStr()
        {
            return service.GetShortDate(GetDateToday());
        }

        public static DateTime GetDateTodayEndDay()
        {
            return service.GetDateTodayEndDay();
        }

        public static DateTime GetDateTodayStartDay()
        {
            return service.GetDateTodayStartDay();
        }

        public static DateTime GetDateEndDay()
        {
            return service.GetDateTodayEndDay();
        }

        public static String GetShortDate(DateTime date)
        {
            return service.GetShortDate(date);
        }
        #endregion
    }
}
