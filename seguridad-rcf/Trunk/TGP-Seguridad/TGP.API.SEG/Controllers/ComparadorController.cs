using System;
using System.Web.Http;
using TGP.Seguridad.BussinessLogic.Api;
using TGP.Seguridad.BussinessLogic.APIRequests;
using TGP.Seguridad.BussinessLogic.APIResponses;

namespace TGP.API.SEG.Controllers
{
    public class ComparadorController : ApiController
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
        [HttpPost, Route("api/seguridad/getelementosdestino")]
        public ResponseGetEntidades GetElementosComparar([FromBody]RequestComparar requestComparar)
        {
            ResponseGetEntidades response = new ResponseGetEntidades();
            try
            {
                if (!apiService.SiValidaUsuario(requestComparar.NombreUsuario))
                {
                    response.Codigo = ResponseGetEntidades.CodigoUsuarioIncorrecto;
                    response.Mensaje = ResponseGetEntidades.MensajeUsuarioIncorrecto;
                    return response;
                }
                if (!apiService.SiValidaKeyWSS(requestComparar.KeyEncrypt))
                {
                    response.Codigo = ResponseGetEntidades.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseGetEntidades.MensajeKeyIncorrecta;
                    return response;
                }

                response.Elementos = apiService.GetElementosOrigen(requestComparar.TipoElemento, requestComparar.CodigoEstructuraFuncional);
                response.Codigo = ResponseGetEntidades.CodigoResponseSuccess;
                response.Mensaje = ResponseGetEntidades.MensajeRepsonseSuccess;
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
        /// Metodo para copiar un rol al ambiente destino
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("api/seguridad/copiarroldestino")]
        public ResponseAPI CopiarRolDestino([FromBody]RequestCopiarRolDestino request)
        {
            ResponseAPI response = new ResponseAPI(); 
            try
            {
                apiService.CopiarRolDestinoApiRaw(request);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseAPI.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo para copiar una actividad al ambiente destino
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("api/seguridad/copiaractividaddestino")]
        public ResponseAPI CopiarActividadDestino([FromBody]RequestCopiarActividadDestino request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {
                apiService.CopiarActividadDestino(request);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseAPI.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo para copiar un menu al ambiente destino
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("api/seguridad/copiarmenuesdestino")]
        public ResponseAPI CopiarMenuDestino([FromBody]RequestCopiarMenuDestino request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {
                apiService.CopiarMenuDestino(request);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseAPI.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo para copiar un nodo al ambiente destino
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("api/seguridad/copiarnodosdestino")]
        public ResponseAPI CopiarNodoDestino([FromBody]RequestCopiarNodoDestino request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {
                apiService.CopiarNodoDestino(request);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseAPI.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo para copiar un nodo al ambiente destino
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("api/seguridad/copiarestructurasdestino")]
        public ResponseAPI CopiarEstructuraDestino([FromBody]RequestCopiarEstructuraDestino request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {
                apiService.CopiarEstructuraDestino(request);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseAPI.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        /// <summary>
        /// Metodo para eliminar elmentos del destino
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/eliminarelementodestino")]
        public ResponseAPI EliminarElementoDestino([FromBody]RequestEliminar request)
        {
            ResponseAPI response = new ResponseAPI();
            string mensajeEliminar = apiService.EliminarDestinoRaw(request);
            response.Mensaje = mensajeEliminar;
            response.Codigo = ResponseAPI.CodigoResponseSuccess;
            return response;
        }



    }
}
