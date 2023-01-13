using System;
using System.Web.Http;
using TGP.Conciliacion.Common.APIRequests;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.Common.APIRequests;

namespace TGP.API.SEG.Controllers
{
    public class AutorizanteController : ApiController
    {
        protected AutorizanteApplicationService autorizanteService = AutorizanteApplicationService.Instance;

        [HttpPost, Route("GetNominados")]
        public ResponseAPINominados GetNominados([FromBody]RequestNominados request)
        {
            ResponseAPINominados response = new ResponseAPINominados();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.GetNominados(request.CodigosRoles);
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

        [HttpPost, Route("GetNominadoAutocomplete")]
        public ResponseAPINominadoAutocomplete GetNominadoAutocomplete([FromBody]RequestNominadoAutocomplete request)
        {
            ResponseAPINominadoAutocomplete response = new ResponseAPINominadoAutocomplete();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.GetNominadoAutocomplete(request.Query);
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

        [HttpPost, Route("GetRoles")]
        public ResponseAPIRoles GetRoles([FromBody]RequestRoles request)
        {
            ResponseAPIRoles response = new ResponseAPIRoles();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.GetRoles(request.CodRoles, request.CodEstructuraFuncional);
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

        [HttpPost, Route("GetRolNodoUsuario")]
        public ResponseAPIRolNodoUsuario GetRolNodoUsuario([FromBody]RequestRolNodoUsuario request)
        {
            ResponseAPIRolNodoUsuario response = new ResponseAPIRolNodoUsuario();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.GetRolNodoUsuario(request.IdUsuario);
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

        [HttpPost, Route("GetNodos")]
        public ResponseAPINodos GetNodos([FromBody]RequestNodos request)
        {
            ResponseAPINodos response = new ResponseAPINodos();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.GetNodos(request.CodEstructuraFuncional, request.IdNominado);
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


        [HttpPost, Route("SetPermisos")]
        public ResponseAPI SetPermisos([FromBody]RequestRolNodoUsuario request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.SetPermisos(request.IdRol, request.IdNodos, request.IdUsuario);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                //response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception e)
            {
                response.Codigo = ResponseAPI.CodigoOtros;
                response.Mensaje = e.Message;
                return response;
            }
        }

        [HttpPost, Route("EliminarPermisos")]
        public ResponseAPI EliminarPermisos([FromBody]RequestNominadoAutocomplete request)
        {
            ResponseAPI response = new ResponseAPI();
            try
            {
                if (!autorizanteService.SiValidaKeyWSS(request.KeyEncrypt))
                {
                    response.Codigo = ResponseAPI.CodigoKeyIncorrecta;
                    response.Mensaje = ResponseAPI.MensajeKeyIncorrecta;
                    return response;
                }
                response = autorizanteService.EliminarPermisosAutorizante(request.Query);
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