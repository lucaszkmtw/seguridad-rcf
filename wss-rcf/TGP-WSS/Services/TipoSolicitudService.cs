using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class TipoSolicitudService : RepositoryCRUDService<TipoSolicitud>
    {
        public static TipoSolicitudService instance = null;

        public static TipoSolicitudService GetInstance()
        {
            if (instance == null)
                instance = new TipoSolicitudService();
            return instance;
        }


        public Object getConsulta(string codigo)
        {

            ICriteria consultaExp = getSession().CreateCriteria<TipoSolicitud>();
            consultaExp.Add(Expression.Eq("Codigo", codigo));
            return consultaExp.UniqueResult();
        }

      

      

    }
}