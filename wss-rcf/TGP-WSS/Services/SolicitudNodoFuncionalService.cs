using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class SolicitudNodoFuncionalService : RepositoryCRUDService<SolicitudNodo>
    {
        public static SolicitudNodoFuncionalService instance = null;

        /// <summary>
        /// Metodo que obtiene una instancia del obtejo
        /// </summary>
        /// <returns></returns>
        public static SolicitudNodoFuncionalService GetInstance()
        {
            if (instance == null)
                instance = new SolicitudNodoFuncionalService();
            return instance;
        }

        /// <summary>
        /// Metodo que recuepera los nodos funcionales desde el ID de una Solicitud
        /// </summary>
        /// <param name="solicitudId"></param>
        /// <returns></returns>
        public List<SolicitudNodo> ObtenerNodosPorSolicitud(long solicitudId)
        {

            ICriteria solNodo = getSession().CreateCriteria<SolicitudNodo>();
            solNodo.CreateAlias("Solicitud", "Solicitud");
            solNodo.Add(Expression.Eq("Solicitud.Id", solicitudId));
            return solNodo.List<SolicitudNodo>().ToList();
        }

        /// <summary>
        /// Metodo que recuepera los nodos funcionales desde el codigo de negocio del nodo y la Estructura funcional
        /// </summary>
        /// <param name="solicitudId"></param>
        /// <returns></returns>
        public List<NodoFuncional> ObtenerNodosPorCodigoYEstructuraFuncional(string codigoNegocioNodo, string codigoEstructura)
        {

            ICriteria solNodo = getSession().CreateCriteria<NodoFuncional>();
            solNodo.CreateAlias("Nodo", "Nodo");
            solNodo.Add(Expression.Eq("NodoFuncional.EstructuraFuncional.CCodigo", codigoEstructura));
            solNodo.Add(Expression.Eq("NodoFuncional.CNegocio", codigoNegocioNodo));
            return solNodo.List<NodoFuncional>().ToList();
        }

      

    }
}