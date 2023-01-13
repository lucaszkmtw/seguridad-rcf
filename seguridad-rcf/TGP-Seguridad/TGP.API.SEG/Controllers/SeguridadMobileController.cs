
using System;
using System.Collections.Generic;
using System.Web.Http;
using TGP.Seguridad.BussinessLogic.Api;
using TGP.Seguridad.BussinessLogic.API.Models.DTO;

namespace TGP.API.SEG.Controllers
{
    public class SeguridadMobileController : ApiController
    {

        /// <summary>
        /// Service para la api
        /// </summary>
        protected ApiSegService apiService = ApiSegService.Instance;

        #region // Metodo Publico // 

        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/mobile/loginmobile")]
        public ResultLoginMobileDTO LoginMobile([FromBody]RequestLoginMobileDTO requestLoginDTO)
        {
            return apiService.LoginMobile(requestLoginDTO);
            
        }

        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/mobile/validartokensession")]
        public ResultValidarTokenSessionDTO ValidarTokenSession([FromBody]RequestMobileDTO requestValidarToken)
        {
            ResultValidarTokenSessionDTO result = new ResultValidarTokenSessionDTO();
            try
            {
                //Si el request no tiene todos los campos se estima q es invalido.
                if (!requestValidarToken.SiRequestValido())
                    throw new Exception("Todos los parámetros son requeridos.");

                result = apiService.ValidarTokenSession(requestValidarToken);

            }
            catch (Exception eValidarSession)
            {
                result.Codigo = "101";
                result.Mensaje = "Problema al validar la session:" + eValidarSession.Message;
            }
            return result;
        }

        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/mobile/logoffmobile")]
        public ResultLoginOffMobileDTO LogOffMobile([FromBody]RequestMobileDTO requestLogOff)
        {
            ResultLoginOffMobileDTO result = new ResultLoginOffMobileDTO();
            try
            {
                //Si el request no tiene todos los campos se estima q es invalido.
                if (!requestLogOff.SiRequestValido())
                    throw new Exception("Todos los parámetros son requeridos.");

                result = apiService.LogOffMobile(requestLogOff);

            }
            catch (Exception eValidarSession)
            {
                result.Codigo = "101";
                result.Mensaje = "Problema al validar la session:" + eValidarSession.Message;
            }
            return result;
        }


        



        #endregion

    }
}
