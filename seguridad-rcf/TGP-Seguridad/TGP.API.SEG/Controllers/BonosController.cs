using System;
using System.Web.Http;
using TGP.Conciliacion.Common.APIRequests;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Api;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.Common.APIRequests;

namespace TGP.API.SEG.Controllers
{
    public class BonosController : ApiController
    {
        protected UsuarioApplicationService usuarioService = UsuarioApplicationService.Instance;
        protected ApiSegService apiService = ApiSegService.Instance;

        /// <summary>
        /// Metodo que trae los elementos del ambiente destino sergun su tipo
        /// </summary>
        /// <param name="requestComparar"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/getinfocontacto")]
        public ResponseAPI GetInfoContacto([FromBody]RequestGetInfoContactoUsuario request)
        {
            ResponseGetInfoContactoUsuario response = new ResponseGetInfoContactoUsuario();
            try
            {
                
                if (!apiService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }

                response = usuarioService.GetInfoContactoUsuario(request.Cuit);
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
        /// Metodo que trae los elementos del ambiente destino sergun su tipo
        /// </summary>
        /// <param name="requestComparar"></param>
        /// <returns></returns>
        [HttpPost, Route("api/seguridad/editarinfocontacto")]
        public ResponseAPI EditarInfoContactoAcreedor([FromBody]RequestEditarInfoAcreedor request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {

                if (!apiService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }

                usuarioService.EditarInfoContacto(request);
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
    }
}