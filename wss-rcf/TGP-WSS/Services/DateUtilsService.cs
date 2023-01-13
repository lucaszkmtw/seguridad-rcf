using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class DateUtilsService
    {
        public static DateTime ObtenerFechaActual()
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            String dateStr = session.CreateSQLQuery("SELECT TO_CHAR(sysdate,'dd/MM/yyyy hh:mi:ss AM') from dual").UniqueResult<String>();
            return DateTime.Parse(dateStr);
        }

        public static String ObtenerFechaActualStr()
        {
            return ObtenerFechaCortaFormatoArg(ObtenerFechaActual());
        }     
      
        public static DateTime ObtenerFechaFin()
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            String dateStr = session.CreateSQLQuery("SELECT TO_CHAR(sysdate,'dd/MM/yyyy') from dual").UniqueResult<String>();
            return DateTime.Parse(dateStr).AddDays(1).AddSeconds(-1);
        }

        public static DateTime ObtenerFechaInicio()
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            String dateStr = session.CreateSQLQuery("SELECT TO_CHAR(sysdate,'dd/MM/yyyy') from dual").UniqueResult<String>();
            return DateTime.Parse(dateStr);
        }

        public static DateTime ObtenerFechaFinFormatoArg(DateTime date)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            DateTime fechaHasta = DateTime.Parse(date.ToShortDateString(), culture).AddDays(1).AddSeconds(-1);
            return fechaHasta;
        }

        public static String ObtenerFechaCortaFormatoArg(DateTime date)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            return date.ToShortDateString();            
        }
    }
}