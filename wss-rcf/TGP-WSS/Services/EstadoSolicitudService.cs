using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class EstadoSolicitudService : RepositoryCRUDService<EstadoSolicitud>
    {
        public static EstadoSolicitudService instance = null;

        public static EstadoSolicitudService GetInstance()
        {
            if (instance == null)
                instance = new EstadoSolicitudService();
            return instance;
        }


        public EstadoSolicitud getEstado(string codigo)
        {

            ICriteria activo = getSession().CreateCriteria<EstadoSolicitud>();
            activo.Add(Expression.Eq("Codigo", codigo));
            return (EstadoSolicitud)activo.UniqueResult();
        }

        public List<EstadoSolicitud> getEstadosNoArchivado()
        {

            ICriteria estados = getSession().CreateCriteria<EstadoSolicitud>();
            estados.Add(Expression.Not(Expression.Eq("Codigo", "5")));
            return (List<EstadoSolicitud>)estados.List<EstadoSolicitud>();
        }

      

      

    }
}