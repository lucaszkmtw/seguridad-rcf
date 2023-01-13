using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic.Api;
using TGP.Seguridad.BussinessLogic.APIRequests;
using TGP.Seguridad.BussinessLogic.APIResponses;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace TGP.API.SEG.Controllers
{
    public class NovedadController : ApiController
    {
        /// <summary>
        /// Service para la api
        /// </summary>
        protected ApiSegService apiService = ApiSegService.Instance;


        /// <summary>
        /// Metodo que trae los elementos del ambiente destino sergun su tipo
        /// </summary>
        /// <param name="requestComparar"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/crearnovedad")]
        public ResponseAPI CrearNovedad([FromBody]RequestCrearNovedad requestNovedad)
        {
            ResponseGetEntidades response = new ResponseGetEntidades();
            try
            {
                if (!apiService.SiValidaUsuario(requestNovedad.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidades.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidades.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!apiService.SiValidaKeyWSS(requestNovedad.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidades.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidades.MensajeKeyIncorrecta;
                    return response;
                }

                apiService.GuardarNovedad(requestNovedad);
                response.Codigo = ResponseGetEntidades.CodigoResponseSuccess;
                response.Mensaje = ResponseGetEntidades.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseGetEntidades.CodigoOtros;
                Exception excepcionActual = e;
                while (excepcionActual.InnerException != null)
                    excepcionActual = excepcionActual.InnerException;

                string excepcionMsj = excepcionActual.Message;
                response.Mensaje = excepcionMsj;
                return response;
            }
        }

        /// <summary>
        /// Metodo que utilizan las aplicacion cliente para recuperar las novedades del usuario logueado
        /// </summary>
        /// <param name="requestObtenerNovedades"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost, Route("api/seguridad/obtenerNovedades")]
        public ResponseObtenerNovedades ObtenerNovedades([FromBody]RequestObtenerNovedades requestObtenerNovedades)
        {
            ResponseObtenerNovedades response = new ResponseObtenerNovedades();
            try
            {
                if (!apiService.SiValidaUsuario(requestObtenerNovedades.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidades.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidades.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!apiService.SiValidaKeyWSS(requestObtenerNovedades.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidades.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidades.MensajeKeyIncorrecta;
                    return response;
                }

                response = apiService.ObtenerNovedades(requestObtenerNovedades);
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseGetEntidades.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo que utilizan las aplicacion cliente para recuperar las novedades del usuario logueado
        /// </summary>
        /// <param name="requestObtenerNovedades"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/marcarcomoleida")]
        public ResponseMarcarComoLeida MarcarComoLeida([FromBody]RequestMarcarComoLeida requestMarcarComoLeida)
        {
            ResponseMarcarComoLeida response = new ResponseMarcarComoLeida();
            try
            {
                if (!apiService.SiValidaUsuario(requestMarcarComoLeida.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidades.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidades.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!apiService.SiValidaKeyWSS(requestMarcarComoLeida.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidades.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidades.MensajeKeyIncorrecta;
                    return response;
                }

                //Servicio que marcar como leida una novedad
                response = apiService.MarcarComoLeida(requestMarcarComoLeida);
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseGetEntidades.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo que utilizan las aplicacion cliente para guardar una novedad para un usuario especifico
        /// </summary>
        /// <param name="requestObtenerNovedades"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/guardarMiNovedad")]
        public ResponseGuardarNovedad GuardarNovedadPorUsuario([FromBody]RequestGuardarNovedad requestGuardarNovedad)
        {
            ResponseGuardarNovedad response = new ResponseGuardarNovedad();
            try
            {
                if (!apiService.SiValidaUsuario(requestGuardarNovedad.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidades.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidades.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!apiService.SiValidaKeyWSS(requestGuardarNovedad.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidades.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidades.MensajeKeyIncorrecta;
                    return response;
                }

                //Servicio que marcar como leida una novedad
                response = apiService.GuardarNovedadPorUsuario(requestGuardarNovedad);
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseGetEntidades.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo que utilizan las aplicacion cliente para guardar una novedad para un usuario especifico
        /// </summary>
        /// <param name="requestObtenerNovedades"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/borrarMiNovedad")]
        public ResponseBorrarNovedad BorrarNovedadPorUsuario([FromBody]RequestBorrarNovedad requestBorrarNovedad)
        {
            ResponseBorrarNovedad response = new ResponseBorrarNovedad();
            try
            {
                if (!apiService.SiValidaUsuario(requestBorrarNovedad.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidades.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidades.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!apiService.SiValidaKeyWSS(requestBorrarNovedad.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidades.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidades.MensajeKeyIncorrecta;
                    return response;
                }

                //Servicio que marcar como leida una novedad
                response = apiService.BorrarNovedadPorUsuario(requestBorrarNovedad);
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseGetEntidades.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }


    }
}