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
    public class AuditoriaController : ApiController
    {
        /// <summary>
        /// Service para la api
        /// </summary>
        protected ApiSegService apiService = ApiSegService.Instance;


        /// <summary>
        /// Obtiene las estructuras funcionales usadas ultimamente por el usuario logueado
        /// </summary>
        /// <param name="requestObtenerNovedades"></param>
        /// <returns></returns>

        [HttpPost, Route("api/seguridad/aplicacionesMasUsadas")]
        public ResponseAPIAppsMasUsadas GetAplicacionesMasUsadas([FromBody]RequestAppsMasUsadas request)
        {
            ResponseAPIAppsMasUsadas response = new ResponseAPIAppsMasUsadas();
            try
            {
                response = apiService.GetAplicacionesMasUsadas(request.NombreUsuario, request.CodigoEstructura);
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