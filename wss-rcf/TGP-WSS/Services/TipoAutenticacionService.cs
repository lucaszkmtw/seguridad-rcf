using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class TipoAutenticacionService : RepositoryCRUDService<TipoAutenticacion>
    {
        public static TipoAutenticacionService instance = null;

        public static TipoAutenticacionService GetInstance()
        {
            if (instance == null)
                instance = new TipoAutenticacionService();
            return instance;
        }


        public TipoAutenticacion ObtenerPorCodigo(string codigo)
        {

            ICriteria tipo = getSession().CreateCriteria<TipoAutenticacion>();
            tipo.Add(Expression.Eq("CCodigo", codigo));
            return (TipoAutenticacion)tipo.UniqueResult();
        }

      

      

    }
}