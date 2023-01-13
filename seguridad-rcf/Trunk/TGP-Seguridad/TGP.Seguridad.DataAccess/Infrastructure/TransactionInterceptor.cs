using Castle.DynamicProxy;
using System;
using System.Web;
using TGP.Seguridad.DataAccess.Helpers;

namespace TGP.Seguridad.DataAccess.Infrastructure
{
    /// <summary>
    /// Intercetor de llamadas a servicios que nos va a permitir ejecutarlos en un entorno transaccional.
    /// Este interceptor va a estar configurado en conjunto a un Proxy Dinamico que wrapea al servicio.
    /// </summary>
    public class TransactionInterceptor : IInterceptor
    {
        /// <summary>
        /// Encapsulamiento interno del repositorio.
        /// </summary>
        private NHibernateRepository repository = NHibernateRepository.Instance;

        /// <summary>
        /// Metodo que es invocado cuando se intenta llamar a un servicio que esta configurado con un proxy.
        /// </summary>
        /// <param name="invocation"> en este parametro viene toda la info del servicio que se esta invocando </param>
        public void Intercept(IInvocation invocation)
        {
            using (var trx = repository.BeginTransaction())
            {
                try
                {
                    // obtenemos del usuario logueado
                    string user = HttpContext.Current.Session["usuario"].ToString();
                    // seteamos en el contexto de la DB con un SP el usuario que hace la operacion - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    UserDBContextSPFactory.SetUserToDBContext(repository.Session, user);
                    // el proceed invoca al servicio de la logica de negocio
                    invocation.Proceed();
                    // hacemos flush para que la entidad tenga seteado el usuario - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    repository.Session.Flush();
                    // Limpiamos con un SP el usuario seteado en el contexto de la BD - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    UserDBContextSPFactory.CleanUserToDBContext(repository.Session);
                    // persistimos los cambios hechos durante la transaccion
                    trx.Commit();
                }
                catch (Exception ex)
                {
                    // Limpiamos con un SP el usuario seteado en el contexto de la BD - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    UserDBContextSPFactory.CleanUserToDBContext(repository.Session);
                    // hubo un error durante la transaccion, se deshacen los cambios
                    trx.Rollback();
                    throw ex;
                }
            }
        }
    }
}
