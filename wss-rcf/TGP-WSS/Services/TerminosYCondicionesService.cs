using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class TerminosYCondicionesService : RepositoryCRUDService<TerminosCondiciones>
    {
        public static TerminosYCondicionesService instance = null;

        public static TerminosYCondicionesService GetInstance()
        {
            if (instance == null)
                instance = new TerminosYCondicionesService();
            return instance;
        }
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public List<TerminosCondiciones> ObtenerPorCodigo(string codigo)
        {

            ICriteria cTerminos = getSession().CreateCriteria<TerminosCondiciones>();
            cTerminos.CreateAlias("EstructuraFuncional", "EstructuraFuncional");
            cTerminos.Add(Expression.Eq("EstructuraFuncional.CCodigo", codigo));
            cTerminos.Add(Expression.Le("FDesde", DateUtilsService.ObtenerFechaInicio()));
            cTerminos.Add(Restrictions.Disjunction().Add(Expression.IsNull("FHasta"))
                .Add(Expression.Ge("FHasta", DateUtilsService.ObtenerFechaFin())));

            return cTerminos.List<TerminosCondiciones>().Distinct<TerminosCondiciones>().OrderByDescending(n => n.FDesde).ToList();
            
        }     

    }
}