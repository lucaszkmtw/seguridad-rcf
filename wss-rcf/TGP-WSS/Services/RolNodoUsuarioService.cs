using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    public class RolNodoUsuarioService : RepositoryCRUDService<RolNodoUsuario>
    {
        public static RolNodoUsuarioService instance = null;

        /// <summary>
        /// Metodo que obtiene la instancia del objeto
        /// </summary>
        /// <returns></returns>
        public static RolNodoUsuarioService GetInstance()
        {
            if (instance == null)
                instance = new RolNodoUsuarioService();
            return instance;
        }
    }
}