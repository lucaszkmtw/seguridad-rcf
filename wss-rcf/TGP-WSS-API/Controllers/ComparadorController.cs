using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using TGP.WSS;
using TGP_APIS.Helpers;
using TGP_APIS.Models.DTO.Comparador;

namespace TGP_APIS.Controllers
{
    public class ComparadorController : ApiController
    {
        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/seguridad/getrolesdestino")]
        public ResponseGetRolesDTO GetRolesDestino([FromBody]RequestCompararDTO requestComparar)
        {
            ResponseGetRolesDTO response = new ResponseGetRolesDTO();
            try
            {
                if (!SiValidaUsuario(requestComparar.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidadesDTO.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidadesDTO.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!SiValidaKeyWSS(requestComparar.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidadesDTO.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidadesDTO.MensajeKeyIncorrecta;
                    return response;
                }

                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
                ICriteria criteriaRoles = session.CreateCriteria<Rol>();
                criteriaRoles.CreateAlias("EstructuraFuncional", "EstructuraFuncional");
                criteriaRoles.Add(Restrictions.Eq("EstructuraFuncional.CCodigo", requestComparar.CodigoEstructuraFuncional));
                IList<Rol> roles = criteriaRoles.List<Rol>();
                IList<ElementoComparar> rolesDTO = new List<ElementoComparar>();
                foreach(Rol r in roles)
                {
                    rolesDTO.Add(new ElementoComparar() {
                        Id = r.CId,
                        Codigo = r.Codigo,
                        Descripcion = r.DDescripcion,
                        DescripcionEstructura = r.EstructuraFuncional.DDescripcion,
                        CodigoEstructura = r.EstructuraFuncional.CCodigo,
                        Version = r.NVersionHibernate
                    });
                }
                response.Roles = rolesDTO;
                response.Codigo = ResponseGetEntidadesDTO.CodigoResponseSuccess;
                response.Mensaje = ResponseGetEntidadesDTO.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseGetEntidadesDTO.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo que valida la existencia de un usuario
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="contraseña"></param>
        /// <returns></returns>
        private bool SiValidaUsuario(string nombreUsuario)
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            Usuario usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == nombreUsuario).SingleOrDefault();
            return usuario != null;
        }

        private bool SiValidaKeyWSS(string keyEncrypt)
        {

            string key = Encriptador.Desencriptar(keyEncrypt);
            return key == ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString();
        }
        
    }
}
