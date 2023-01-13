using System;
using System.Globalization;

namespace TGP.Seguridad.DataAccess.Helpers
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
        private NHibernateRepository repository = NHibernateRepository.Instance;
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


        #region // Metodos DateUtils  //

        public DateTime GetDateToday()
        {
            String dateStr = this.repository.Session.CreateSQLQuery("SELECT TO_CHAR(sysdate,'dd/MM/yyyy hh:mi:ss AM') from dual").UniqueResult<String>();
            return DateTime.Parse(dateStr);
        }

        public String GetShortDateTodayStr()
        {
            return GetShortDate(GetDateToday());
        }

        public DateTime GetDateTodayEndDay()
        {
            String dateStr = this.repository.Session.CreateSQLQuery("SELECT TO_CHAR(sysdate,'dd/MM/yyyy') from dual").UniqueResult<String>();
            return DateTime.Parse(dateStr).AddDays(1).AddSeconds(-1);
        }

        public DateTime GetDateTodayStartDay()
        {
            String dateStr = this.repository.Session.CreateSQLQuery("SELECT TO_CHAR(sysdate,'dd/MM/yyyy') from dual").UniqueResult<String>();
            return DateTime.Parse(dateStr);
        }

        public DateTime GetDateEndDay(DateTime date)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            DateTime fechaHasta = DateTime.Parse(date.ToShortDateString(), culture).AddDays(1).AddSeconds(-1);
            return fechaHasta;
        }

        public String GetShortDate(DateTime date)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            return date.ToShortDateString();
        }
        #endregion
    }
}
