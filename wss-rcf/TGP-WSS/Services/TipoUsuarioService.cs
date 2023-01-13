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
    /// Clase que gestiona y recupera el Tipo de Usuario
    /// </summary>
    public class TipoUsuarioService : RepositoryCRUDService<TipoUsuario>
    {
        public static TipoUsuarioService instance = null;

        /// <summary>
        /// Metodo que obtiene una instancia del objeto
        /// </summary>
        /// <returns></returns>
        public static TipoUsuarioService GetInstance()
        {
            if (instance == null)
                instance = new TipoUsuarioService();
            return instance;
        }

        /// <summary>
        /// Metodo que obtiene el tipo de usuario por el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public TipoUsuario ObtenerPorCodigo(string codigo)
        {

            ICriteria tipo = getSession().CreateCriteria<TipoUsuario>();
            tipo.Add(Expression.Eq("CCodigo", codigo));
            return (TipoUsuario)tipo.UniqueResult();
        }

    }
}