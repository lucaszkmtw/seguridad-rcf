using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    /// <summary>
    /// Clase que contiene los metodos de recuperacion de CRUD de la Clase ROL
    /// </summary>
    public class RolService : RepositoryCRUDService<Rol>
    {
        public static RolService instance = null;

        /// <summary>
        /// Metodo que devuelve la instancia del objeto
        /// </summary>
        /// <returns></returns>
        public static RolService GetInstance()
        {
            if (instance == null)
                instance = new RolService();
            return instance;
        }

        /// <summary>
        /// Metodo que obtiene el Rol por codigo de Rol y Codigo de Estructura Funcional
        /// </summary>
        /// <param name="codigoRol"></param>
        /// <param name="codEstructura"></param>
        /// <returns></returns>
        public Rol ObtenerRolPorCodigoYEstructura(string codigoRol, string codigoEstructura)
        {

            ICriteria rol = getSession().CreateCriteria<Rol>();
            rol.CreateAlias("EstructuraFuncional", "EstructuraFuncional");
            rol.Add(Expression.Eq("EstructuraFuncional.CCodigo", codigoEstructura));
            rol.Add(Expression.Eq("Codigo", codigoRol));

            return (Rol) rol.UniqueResult();
        }

    }
}