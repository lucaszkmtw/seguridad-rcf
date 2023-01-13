using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class EstructuraFuncionalService : RepositoryCRUDService<EstructuraFuncional>
    {
        public static EstructuraFuncionalService instance = null;

        public static EstructuraFuncionalService GetInstance()
        {
            if (instance == null)
                instance = new EstructuraFuncionalService();
            return instance;
        }


        public EstructuraFuncional getByCodigo(string codigo)
        {

            ICriteria ruco = getSession().CreateCriteria<EstructuraFuncional>();
            ruco.Add(Expression.Eq("CCodigo", codigo));
            return (EstructuraFuncional) ruco.UniqueResult();
        }
     

    }
}