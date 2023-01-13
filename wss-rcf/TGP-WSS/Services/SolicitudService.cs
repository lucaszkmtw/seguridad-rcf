using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class SolicitudService : RepositoryCRUDService<Solicitud>
    {
        public static SolicitudService instance = null;

        public static SolicitudService GetInstance()
        {
            if (instance == null)
                instance = new SolicitudService();
            return instance;
        }
 


        public List<Solicitud> getAllSolicitudes()
        {
            return this.GetAll().Where(s => s.TipoSolicitud.Codigo == "1").ToList();

        }

        public List<Solicitud> getByEstado(string estado)
        {
            ICriteria solicitudes = getSession().CreateCriteria<Solicitud>();
            solicitudes.CreateAlias("TipoSolicitud", "tipo");
            solicitudes.CreateAlias("EstadoSolicitud", "estado");
            solicitudes.Add(Expression.Eq("tipo.Codigo", "1"));
            solicitudes.Add(Expression.Eq("estado.Codigo", estado));  
            return (List<Solicitud>)solicitudes.List<Solicitud>();
        }


        public List<Solicitud> getByCuit(string cuit)
        {
            return  getSession().QueryOver<Solicitud>().Where(s => s.Cuit == long.Parse(cuit)).List<Solicitud>().ToList();
        }
  
     

    }
}