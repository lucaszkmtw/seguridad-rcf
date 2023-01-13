
using Mapster;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TGP.Seguridad.BussinessLogic.API.Models.DTO;
using TGP.Seguridad.BussinessLogic.APIRequests;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Mapping;
using Utils;

namespace TGP.Seguridad.BussinessLogic.Api
{
    /// <summary>
    /// Clase extendida del Comparador que tiene algunos metodos particulares para la api
    /// </summary>
    public class ApiSegService : SeguridadApplicationService
    {
        private static ApiSegService instance;

        /// <summary>
        /// Service para la api
        /// </summary>
        protected ComparadorApplicationService comparadorService = ComparadorApplicationService.Instance;

        /// <summary>
        /// Service para la api
        /// </summary>
        protected NovedadApplicationService novedadService = NovedadApplicationService.Instance;

        //Singleton
        public static new ApiSegService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiSegService();
                }
                return instance;
            }
        }

        public ResultLoginMobileDTO LoginMobile(RequestLoginMobileDTO requestLoginDTO)
        {

            ISession session = Session();
            List<VWUsuarioPermisos> permisosDelUsuario = null;


            Usuario usuario = session.QueryOver<Usuario>().Where(u => u.NombreUsuario == requestLoginDTO.NombreUsuario).SingleOrDefault();
            ResultLoginMobileDTO resultado = new ResultLoginMobileDTO();

            try
            {
                if (usuario != null)
                {
                    var pass = MD5Password.GetMd5Hash(requestLoginDTO.Contrasena);
                    usuario = session.QueryOver<Usuario>().Where(u => u.NombreUsuario == requestLoginDTO.NombreUsuario && u.Contrasena == pass).SingleOrDefault();
                    if (usuario != null)
                    {
                        //permisosDelUsuario = db.VW_USUARIO_PERMISOS.Where(u => u.D_USUARIO == usu && u.D_PASSWORD == pass && u.C_CODIGO_ESTFUNC == codigo_estruc_func).ToList();
                        permisosDelUsuario = session.QueryOver<VWUsuarioPermisos>().Where(u => u.Usuario == requestLoginDTO.NombreUsuario && u.CodigoEstructura == requestLoginDTO.CodigoEstructuraFuncional).List<VWUsuarioPermisos>().ToList();
                        var portal = session.QueryOver<EstructuraFuncional>().Where(p => p.Codigo == "POR").SingleOrDefault();
                        if (permisosDelUsuario.Count > 0 || (permisosDelUsuario.Count == 0 && requestLoginDTO.CodigoEstructuraFuncional == portal.Codigo))
                        {
                            resultado.SiValido = "true";
                            ////Avatar
                            //if (usuario.Avatar != null)
                            //    resultado.Avatar = GetThumbnails(usuario.BAvatar);
                            resultado.UsuarioID = usuario.Id.ToString();
                        }
                        else
                        {
                            resultado.Mensaje = "Su usuario no tiene los permisos mínimos para el acceso.";
                            resultado.SiValido = "false";
                            resultado.Codigo = "102";
                        }
                    }
                    else
                    {
                        resultado.Codigo = "101";
                        resultado.SiValido = "false";
                        resultado.Mensaje = "El nombre de usuario o la contraseña especificados son incorrectos";
                    }
                    if (resultado.SiValido == "true")
                    {
                        if (usuario.SiBloqueado == true || usuario.SiActivo != true)
                        {
                            resultado.Mensaje = "El usuario se encuentra bloqueado o inactivo, contactese con el administrador";
                            resultado.SiValido = "false";
                            resultado.Codigo = "100";
                        }
                        else
                        {
                            //resultado.Identificacion = usuario.Nombre;
                            resultado.Descripcion = usuario.NombreUsuario;
                            var tokenSession = GetTokenSessionMobile(usuario);
                            resultado.TokenSession = tokenSession.Token;
                            resultado.FechaExpiracionTokenSession = tokenSession.FechaExpiracion.ToString();

                            //ObtenerPermisosAsociados4Mobile(resultado, permisosDelUsuario);
                            resultado.Mensaje = "OK";
                            resultado.Codigo = "1";
                        }
                    }
                }
                else
                {
                    resultado.Mensaje = "El nombre de usuario o la contraseña especificados son incorrectos";
                    resultado.Codigo = "104";
                }

            }
            catch (Exception eLoginMobile)
            {
                resultado.SiValido = "false";
                resultado.Mensaje = eLoginMobile.Message;
                resultado.Codigo = "999";
            }
            return resultado;
        }



        public ResultValidarTokenSessionDTO ValidarTokenSession(RequestMobileDTO requestValidarToken)
        {
            ResultValidarTokenSessionDTO result = new ResultValidarTokenSessionDTO();

            try
            {
                var s = DateTime.Now;
                ISession session = Session();
                TokenMobile token = session.QueryOver<TokenMobile>().Where(x => x.IdUsuario == Convert.ToInt64(requestValidarToken.UserID) && x.Token == requestValidarToken.Token && x.FechaExpiracion > DateTime.Now).SingleOrDefault();

                if (token != null)
                {
                    Convert.ToDateTime(token.FechaExpiracion).AddHours(1);
                    session.Update(token);
                    session.Flush();
                    result.SiValido = "true";
                    result.Codigo = "1";
                    result.Mensaje = "Session Activa";
                }
                else
                {
                    result.SiValido = "false";
                    result.Codigo = "100";
                    result.Mensaje = "La Session no existe";
                }

            }
            catch (Exception eValidarSession)
            {

                result.SiValido = "false";
                result.Codigo = "101";
                result.Mensaje = "Problema al validar la session:" + eValidarSession.Message;
            }
            return result;
        }



        public ResultLoginOffMobileDTO LogOffMobile(RequestMobileDTO requestLoginOff)
        {
            ResultLoginOffMobileDTO result = new ResultLoginOffMobileDTO();
            try
            {
                ISession session = Session();
                //El ultimo token valido para el usuario correspondiente. Si no existe, null.
                TokenMobile token = session.QueryOver<TokenMobile>().Where(u => u.IdUsuario == Convert.ToInt64(requestLoginOff.UserID) && u.Token == requestLoginOff.Token).SingleOrDefault();
                if (token != null)
                    if (Convert.ToDateTime(token.FechaExpiracion).Ticks > DateTime.Now.Ticks)//quiere decir que no esta vencido
                    {
                        Convert.ToDateTime(token.FechaExpiracion).AddHours(-1);
                        session.Update(token);
                        session.Flush();
                        result.Codigo = "1";
                        result.Mensaje = "La session se cerro con exito";

                    }
                    else
                    {
                        result.Codigo = "1";
                        result.Mensaje = "Session vencida";
                    }
                else
                {
                    result.Codigo = "100";
                    result.Mensaje = "La Session no existe";
                }

            }
            catch (Exception eValidarSession)
            {
                result.Codigo = "101";
                result.Mensaje = "Problema al validar la session:" + eValidarSession.Message;
            }
            return result;
        }


        public void VencerTokenSession(TokenMobile tokenOld)
        {
            ISession session = Session();
            Convert.ToDateTime(tokenOld.FechaExpiracion).AddHours(-1);
            session.Update(tokenOld);
            session.Flush();
        }



        private TokenMobile GetTokenSessionMobile(Usuario usuario)
        {
            try
            {
                ISession session = Session();
                TokenMobile token = new TokenMobile
                {
                    Token = Guid.NewGuid().ToString(),
                    IdUsuario = usuario.Id,
                    //FechaRegistro = DateTime.Now,
                    FechaExpiracion = DateTime.Now.AddHours(1),
                    FechaAlta = DateTime.Now
                };
                session.Save(token);
                session.Flush();
                return token;
            }
            catch (Exception eToken)
            {
                throw new Exception("Problema al generar el token: " + eToken.Message);
            }

        }


        public IList<ElementoComparar> GetElementosOrigen(string tipoElemento, string codigoEstructuraFuncional)
        {
            try
            {
                return comparadorService.GetElementosOrigen(tipoElemento, codigoEstructuraFuncional);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para copiar roles a destino. Se llama desde aca para que el cliente no tenga conocimiento de los metodos protected del ComparadorService
        /// </summary>
        /// <param name="request"></param>
        public void CopiarRolDestinoApiRaw(RequestCopiarRolDestino request)
        {
            try
            {
                comparadorService.CopiarRolDestinoApi(request.Roles);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para copiar roles a destino. Se llama desde aca para que el cliente no tenga conocimiento de los metodos protected del ComparadorService
        /// </summary>
        /// <param name="request"></param>
        public void CopiarActividadDestino(RequestCopiarActividadDestino request)
        {
            try
            {
                comparadorService.CopiarActividadDestinoApi(request.Actividades);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para copiar roles a destino. Se llama desde aca para que el cliente no tenga conocimiento de los metodos protected del ComparadorService
        /// </summary>
        /// <param name="request"></param>
        public void CopiarMenuDestino(RequestCopiarMenuDestino request)
        {
            try
            {
                comparadorService.CopiarMenuDestinoApi(request.Menues);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para copiar nodos a destino. Se llama desde aca para que el cliente no tenga conocimiento de los metodos protected del ComparadorService
        /// </summary>
        /// <param name="request"></param>
        public void CopiarNodoDestino(RequestCopiarNodoDestino request)
        {
            try
            {
                comparadorService.CopiarNodosDestinoApi(request.Nodos);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para copiar nodos a destino. Se llama desde aca para que el cliente no tenga conocimiento de los metodos protected del ComparadorService
        /// </summary>
        /// <param name="request"></param>
        public void CopiarEstructuraDestino(RequestCopiarEstructuraDestino request)
        {
            try
            {
                comparadorService.CopiarEstructurasDestinoApi(request.Estructuras);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para copiar roles a destino
        /// </summary>
        /// <param name="request"></param>
        public string EliminarDestinoRaw(RequestEliminar request)
        {

            long id = request.Id; int version = request.Version;

            switch (request.TipoElemento)
            {
                case ComparadorGenerico.ROL:
                    comparadorService.EliminarRolDestino(id, version);
                    return "El Rol fue eliminado del destino.";
                case ComparadorGenerico.MENUOPCION:
                    comparadorService.EliminarMenuDestino(id, version);
                    return "El Menú fue eliminado del destino.";
                case ComparadorGenerico.ACTIVIDAD:
                    comparadorService.EliminarActividadDestino(id, version);
                    return "La Actividad fue eliminada del destino.";
                case ComparadorGenerico.NODO:
                    comparadorService.EliminarNodoDestino(id, version);
                    return "El Nodo fue eliminada del destino.";
                case ComparadorGenerico.ESTRUCTURA:
                    comparadorService.EliminarEstructuraDestino(id, version);
                    return "La Estructura fue eliminada del destino.";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Metodo que valida la existencia de un usuario
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="contraseña"></param>
        /// <returns></returns>
        public bool SiValidaUsuario(string nombreUsuario)
        {
            ISession session = Session();
            Usuario usuario = session.QueryOver<Usuario>().Where(u => u.NombreUsuario == nombreUsuario).SingleOrDefault();
            return usuario != null;
        }

        /// <summary>
        /// Metodo que valida que al key del llamado sea correcta
        /// </summary>
        /// <param name="keyEncrypt"></param>
        /// <returns></returns>
        public bool SiValidaKeyWSS(string keyEncrypt)
        {

            string key = Encriptador.Desencriptar(keyEncrypt);
            return key == ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString();
        }

        /// <summary>
        /// Metodo que inserta una nueva novedad
        /// </summary>
        /// <param name="novedad"></param>
        public void GuardarNovedad(RequestCrearNovedad novedad)
        {
            try
            {
                NovedadDTO novedadDTO = novedad.Adapt<NovedadDTO>();
                novedadService.GuardarNovedad(novedadDTO);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// Metodo que recupera las novedades de un usuario
        /// </summary>
        /// <param name="requestObtenerNovedades"></param>
        /// <returns></returns>
        public ResponseObtenerNovedades ObtenerNovedades(RequestObtenerNovedades requestObtenerNovedades)
        {
            ResponseObtenerNovedades response = new ResponseObtenerNovedades();
            try
            {
                //Novedades Guardadas
                var novedadesGuardadas = novedadService.GetNovedadesGuardadas(requestObtenerNovedades.NombreUsuario);
                response.NovedadesGuardadas = novedadesGuardadas;
                //Novedades no leida
                var novedades = novedadService.GetNovedadesNoLeidas(requestObtenerNovedades.NombreUsuario, TipoNovedadConstants.TIPO_NOVEDAD_NOTIFICACION);
                response.NovedadesNoLeidas = novedades;
                //Novedades Vigentes
                response.NovedadesVigentes = novedadService.GetNovedadesTotales(requestObtenerNovedades.NombreUsuario, TipoNovedadConstants.TIPO_NOVEDAD_NOTIFICACION);
                //Advertencias
                response.AdvertenciasTotalesVigentes = novedadService.GetAdvertenciasTotales(requestObtenerNovedades.NombreUsuario);
                //Advertencias no leidas
                response.AdvertenciasNoLeidas = novedadService.GetNovedadesNoLeidas(requestObtenerNovedades.NombreUsuario, TipoNovedadConstants.TIPO_NOVEDAD_ADVERTENCIA);
                //Respuesta del servicio
                response.Codigo = ResponseObtenerNovedades.CodigoResponseSuccess;
                response.Mensaje = ResponseObtenerNovedades.MensajeRepsonseSuccess;
            }
            catch (Exception ex)
            {
                response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                response.Mensaje = "Error en el servicio de ObtenerNovedades: " + ex.Message;
            }
            //Respuesta del servicio
            return response;
        }


        public ResponseBorrarNovedad BorrarNovedadPorUsuario(RequestBorrarNovedad requestBorrarNovedad)
        {
            ResponseBorrarNovedad response = new ResponseBorrarNovedad();
            try
            {
                if (novedadService.SiNovedadGuardada(requestBorrarNovedad.NovedadId, requestBorrarNovedad.NombreUsuario))
                {
                    novedadService.BorrarNovedadPorUsuario(requestBorrarNovedad.NovedadId, requestBorrarNovedad.NombreUsuario);
                    //Respuesta del servicio
                    response.Codigo = ResponseObtenerNovedades.CodigoResponseSuccess;
                    response.Mensaje = ResponseObtenerNovedades.MensajeRepsonseSuccess;
                }
                else
                {
                    response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                    response.Mensaje = "La novedad NO esta guardada por el usuario, por ende no se puede borrar la relacion";
                }
            }
            catch (Exception ex)
            {
                response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                response.Mensaje = "Error en el servicio de BorrarNovedadPorUsuario: " + ex.Message;
            }
            //Respuesta del servicio
            return response;
        }

        public ResponseGuardarNovedad GuardarNovedadPorUsuario(RequestGuardarNovedad requestGuardarNovedad)
        {
            ResponseGuardarNovedad response = new ResponseGuardarNovedad();
            try
            {
                //if (!novedadService.SiNovedadGuardada(requestGuardarNovedad.NovedadId, requestGuardarNovedad.NombreUsuario))
                //{
                    novedadService.GuardarNovedadPorUsuario(requestGuardarNovedad.NovedadId, requestGuardarNovedad.NombreUsuario, requestGuardarNovedad.Comentario);
                    //Respuesta del servicio
                    response.Codigo = ResponseObtenerNovedades.CodigoResponseSuccess;
                    response.Mensaje = ResponseObtenerNovedades.MensajeRepsonseSuccess;
                //}
                //else
                //{
                //    response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                //    response.Mensaje = "La novedad ya ha sido guardada por el usuario.";
                //}
            }
            catch (Exception ex)
            {
                response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                response.Mensaje = "Error en el servicio de GuardarNovedadPorUsuario: " + ex.Message;
            }
            //Respuesta del servicio
            return response;
        }

        public ResponseMarcarComoLeida MarcarComoLeida(RequestMarcarComoLeida requestMarcarComoLeida)
        {
            ResponseMarcarComoLeida response = new ResponseMarcarComoLeida();
            try
            {
                if (!novedadService.EsNovedadLeida(requestMarcarComoLeida.NovedadId, requestMarcarComoLeida.NombreUsuario))
                {
                    novedadService.MarcarNovedadNotificacion(requestMarcarComoLeida.NovedadId, requestMarcarComoLeida.NombreUsuario);
                    //Respuesta del servicio
                    response.Codigo = ResponseObtenerNovedades.CodigoResponseSuccess;
                    response.Mensaje = ResponseObtenerNovedades.MensajeRepsonseSuccess;
                }
                else
                {
                    response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                    response.Mensaje = "La novedad ya ha sido leída por el usuario.";
                }
            }
            catch (Exception ex)
            {
                response.Codigo = ResponseObtenerNovedades.CodigoOtros;
                response.Mensaje = "Error en el servicio de MarcarComoLeida: " + ex.Message;
            }
            //Respuesta del servicio
            return response;

        }

        #region Auditoria

        public ResponseAPIAppsMasUsadas GetAplicacionesMasUsadas(string NombreUsuario, string CodigoEstructiura)
        {
            string[] appsNoIncluidas = new string[] { $"'{CodigoEstructiura}'", "'POR'" };

            ResponseAPIAppsMasUsadas response = new ResponseAPIAppsMasUsadas();
            ISession session = repository.Session;
            string sqlQueryString = $"select x.c_estructura_funcional, d_descripcion, cant," +
                $"(select max(ac1.f_conexion) from seg_auditoria_conexion ac1 where ac1.c_estructura_funcional = x.c_estructura_funcional and ac1.c_id_usuario = x.c_id_usuario) as f_conexion " +
                $"from(select s.c_estructura_funcional, e.d_descripcion, s.c_id_usuario, count(*) as cant " +
                $"from seg_auditoria_conexion s " +
                $"inner join seg_usuario u on u.c_id = s.c_id_usuario " +
                $"inner join seg_estructura_funcional e on e.c_codigo = s.c_estructura_funcional " +
                $"where s.c_estructura_funcional not in ({string.Join(",", appsNoIncluidas)}) and u.d_usuario = '{NombreUsuario}' " +
                $"group by s.c_estructura_funcional, e.d_descripcion, s.c_id_usuario " +
                $"order by cant desc) x " +
                $"order by f_conexion desc";
            IQuery query = session.CreateSQLQuery(sqlQueryString);
            IList<object> queryAppsMasUsadas = query.List<object>();

            if (queryAppsMasUsadas.Count > 4)
                queryAppsMasUsadas = queryAppsMasUsadas.Take(4).ToList();

            foreach (object var in queryAppsMasUsadas)
            {
                ResponseAppsMasUsadas responseApp = new ResponseAppsMasUsadas()
                {
                    CodigoEstructura = ((object[])var)[0].ToString(),
                    DescripcionEstructura = ((object[])var)[1].ToString(),
                    CantApariciones = Convert.ToInt32(((object[])var)[2]),
                    Url = ConfigurationManager.AppSettings[((object[])var)[0].ToString()]
                };
                response.Apps.Add(responseApp);
            }
            return response;
        }

        #endregion
    }

}
