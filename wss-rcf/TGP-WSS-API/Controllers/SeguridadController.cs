using NHibernate;
using NHibernate.Criterion;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;
using TGP.WSS;
using TGP.WSS.Services;
using TGP.WSS.API.Models;
using TGP.WSS.API.Models.DTO;
using TGP.WSS.API.Models.DTO.Mobile;
using System.Net.Mail;

namespace TGP.WSS.API.Controllers
{
    public class SeguridadController : ApiController
    {
        
        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/seguridad/loginmobile")]
        public ResultLoginMobileDTO LoginMobile([FromBody]RequestLoginMobileDTO requestLoginDTO)
        {
            string ldapServer = ConfigurationManager.AppSettings["LDAPSERVER"];
            string targetOU = ConfigurationManager.AppSettings["TARGET_OU"];
            string ldapuser = "tesoreria" + @"\" + requestLoginDTO.NombreUsuario;
            string ldappass = requestLoginDTO.Contrasena;
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            List<VWUsuarioPermisos> permisosDelUsuario = null;


            Usuario usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == requestLoginDTO.NombreUsuario).SingleOrDefault();
            ResultLoginMobileDTO resultado = new ResultLoginMobileDTO();

            try
            {
                if (usuario != null)
                {
                    var pass = MD5Password.GetMd5Hash(requestLoginDTO.Contrasena);
                    usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == requestLoginDTO.NombreUsuario && u.DPassword == pass).SingleOrDefault();
                    if (usuario != null)
                    {
                        permisosDelUsuario = session.QueryOver<VWUsuarioPermisos>().Where(u => u.DUsuario == requestLoginDTO.NombreUsuario && u.CEstructuraFuncional == requestLoginDTO.CodigoEstructuraFuncional).List<VWUsuarioPermisos>().ToList();
                        var portal = session.QueryOver<EstructuraFuncional>().Where(p => p.CCodigo == "MPAGOS").SingleOrDefault();
                        if (permisosDelUsuario.Count > 0 || (permisosDelUsuario.Count == 0 && requestLoginDTO.CodigoEstructuraFuncional == portal.CCodigo))
                        {
                            resultado.SiValido = "true";
                            //Avatar
                            if (usuario.BAvatar != null)
                                resultado.Avatar = usuario.BAvatar;
                            resultado.UsuarioID = usuario.CId.ToString();
                            if (usuario.isAcreedor())//Es Acreedor
                            {
                                var usuarioAcreedor = session.QueryOver<Acreedor>().Where(a => a.CId == usuario.CId).SingleOrDefault();
                                resultado.EsAcreedor = true;
                                resultado.RazonSocial = usuarioAcreedor.DRazonSocial;
                                resultado.Cuit = usuarioAcreedor.DCuit;
                                resultado.Email = usuario.DMail;
                                if (!string.IsNullOrWhiteSpace(usuario.DTelefono))
                                    resultado.Telefono = usuario.DTelefono;
                                else
                                {
                                    string telefono = "";
                                    if (!string.IsNullOrWhiteSpace(usuario.CodArea))
                                        telefono += usuario.CodArea + "-";
                                    telefono += usuario.NumeroTel;
                                    if (!string.IsNullOrWhiteSpace(usuario.Interno))
                                        telefono += " interno " + usuario.Interno;
                                    resultado.Telefono = telefono;
                                }
                            }
                            else
                            {
                                resultado.Mensaje = "Su usuario no es acreedor.";
                                resultado.SiValido = "false";
                                resultado.Codigo = "103";
                            }

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
                        if (usuario.MBloqueado == true || usuario.MActivo != true)
                        {
                            resultado.Mensaje = "El usuario se encuentra bloqueado o inactivo, contactese con el administrador";
                            resultado.SiValido = "false";
                            resultado.Codigo = "100";
                        }
                        else
                        {
                            resultado.Identificacion = usuario.DUsuario;
                            resultado.Descripcion = usuario.getDescripcionUsuario();
                            var tokenSession = GetTokenSessionMobile(usuario);
                            resultado.TokenSession = tokenSession.Token;
                            resultado.FechaExpiracionTokenSession = tokenSession.FechaExpiracion.ToString();

                            ObtenerPermisosAsociados4Mobile(resultado, permisosDelUsuario);
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

        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/seguridad/validartokensession")]
        public ResultValidarTokenSessionDTO ValidarTokenSession([FromBody]ResquestValidarTokenSessionDTO requestValidarToken)
        {
            ResultValidarTokenSessionDTO result = new ResultValidarTokenSessionDTO();
            
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
                TokenSesionMobile token = session.QueryOver<TokenSesionMobile>().Where(x => x.UsuarioID == Convert.ToInt64(requestValidarToken.UsuarioID) && x.Token == requestValidarToken.TokenSession && x.FechaExpiracion > DateUtilsService.ObtenerFechaActual()).SingleOrDefault();
                if (token != null)
                {
                    if (token.FechaBaja == null)
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
                        result.Codigo = "101";
                        result.Mensaje = "Session Cerrada/Baja";
                    }
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

        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/seguridad/logoffmobile")]
        public ResultLoginOffMobileDTO LogOffMobile([FromBody]ResquestLoginOffMobileDTO requestLoginOff)
        {
            ResultLoginOffMobileDTO result = new ResultLoginOffMobileDTO();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
                //El ultimo token valido para el usuario correspondiente. Si no existe, null.
                TokenSesionMobile token = session.QueryOver<TokenSesionMobile>().Where(u => u.UsuarioID == Convert.ToInt64(requestLoginOff.UsuarioID) && u.Token == requestLoginOff.TokenSession).SingleOrDefault();
                if (token != null)
                {
                    token.FechaBaja = DateTime.Now;
                    token.FechaExpiracion = DateTime.Now;
                    session.Update(token);
                    session.Flush();
                    result.Codigo = "1";
                    result.Mensaje = "La session se cerro con exito";

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


        [HttpPost] //-> Defino el metodo como HttpPost.
        [Route("api/seguridad/sendemailmobile")]
        public ResultSendEMailMobileDTO SendEmailMobile([FromBody] RequestSendEmailMobileDTO requestSendEmail)
        {
            ResultSendEMailMobileDTO resultado = new ResultSendEMailMobileDTO();
            try
            {
                // Enviar informacion por mail
                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"]);
                message.To.Add(ConfigurationManager.AppSettings["EMAIL_TO"]);
                // Nombre del proyecto
                string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                // Fin Nombre del proyecto
                message.Subject = "[CDP Mobile - MENSAJE: " + requestSendEmail.Usuario + " ] " + requestSendEmail.Asunto.ToUpper();
                message.IsBodyHtml = true;
                message.Body = requestSendEmail.Mensaje;

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["EMAIL_SMPT"]);
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_USERNAME"], ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_PASSWORD"]);
                smtp.Send(message);
                resultado.Codigo = "1";
                resultado.Mensaje = "El mail se envio con exito";
            }
            catch (Exception eMail)
            {
                resultado.Codigo = "999";
                resultado.Mensaje = "Error:" + eMail.Message;
            }
            return resultado;
        }



        private TokenSesionMobile GetTokenSessionMobile(Usuario usuario)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
                TokenSesionMobile token = new TokenSesionMobile
                {
                    Token = Guid.NewGuid().ToString(),
                    UsuarioID = usuario.CId,
                    FechaRegistro = DateTime.Now,
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

        /// <summary>
        /// Metodo que fuerza a vencer el token session
        /// </summary>
        /// <param name="tokenOld"></param>
        private void VencerTokenSession(TokenSesionMobile tokenOld)
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            Convert.ToDateTime(tokenOld.FechaExpiracion).AddHours(-1);
            session.Update(tokenOld);
            session.Flush();
        }

        [HttpGet] //-> Defino el metodo como HttpPost.
        [Route("api/seguridad/getversion")]
        public string GetVersion()
        {
            return "1.1.0.0";
        }

        private void ObtenerPermisosAsociados4Mobile(ResultLoginMobileDTO resultado, List<VWUsuarioPermisos> usuariosPermisos)
        {
            resultado.ActividadesPermitidas = new List<string>();
            //EntitiesSeguridad db = new EntitiesSeguridad();
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            foreach (VWUsuarioPermisos usuarioPermiso in usuariosPermisos.OrderBy(u => u.DActividad))
            {
                resultado.ActividadesPermitidas.Add(usuarioPermiso.DActividad);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mires"></param>
        /// <param name="usuariosPermisos"></param>
        private void ObtenerPermisosAsociados(ResultadoLogin mires, List<VWUsuarioPermisos> usuariosPermisos)
        {
            //ya viene filtrado por usuario y estructura funcional
            PermisoUsuarioWS permiso = null;
            String actividad = "--";
            //EntitiesSeguridad db = new EntitiesSeguridad();
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            foreach (VWUsuarioPermisos usuarioPermiso in usuariosPermisos.OrderBy(u => u.DActividad))
            {
                if (actividad != usuarioPermiso.DActividad)
                {
                    permiso = new PermisoUsuarioWS(usuarioPermiso.DActividad, usuarioPermiso.DRol, usuarioPermiso.CodigoRol);//, usuarioPermiso.M_AUDITA);
                    //agrego el nodo                    
                    permiso.NodosAutorizados.Add(new NodoFuncionalWS(usuarioPermiso.CNodoFuncional, usuarioPermiso.DNodoFuncional));
                    //agrego el permiso al resultado de salida
                    mires.Permisos.Add(permiso);
                    //actualizo el corte de control
                    actividad = usuarioPermiso.DActividad;
                }
                else
                {
                    permiso.NodosAutorizados.Add(new NodoFuncionalWS(usuarioPermiso.CNodoFuncional, usuarioPermiso.DNodoFuncional));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mires"></param>
        /// <param name="usuariosPermisos"></param>
        private void ObtenerMenuOpcionAsociado(ResultadoLogin mires, List<VWUsuarioPermisos> usuariosPermisos)
        {
            var q = (from s in usuariosPermisos
                     group s by new { idMenu = s.MenuOpcion, idPadre = s.MenuOpcionPadre } into g
                     select new { g.Key.idMenu, g.Key.idPadre, descripcionMenu = g.Max(s => s.MenuOpcion), url = g.Max(s => s.DUrl), orden = g.Max(s => s.NOrden), icono = g.Max(s => s.DIcono) });


            TreeView treeView = new TreeView();
            List<MenuWS> menuList = new List<MenuWS>();
            foreach (var a in q.ToList())
            {
                if (a.idPadre != null)
                    menuList.Add(new MenuWS(a.idMenu.CId, a.descripcionMenu.DDescripcion, a.url, a.idPadre.CId, Convert.ToInt32(a.orden), a.icono));
                else
                    menuList.Add(new MenuWS(a.idMenu.CId, a.descripcionMenu.DDescripcion, a.url, null, Convert.ToInt32(a.orden), a.icono));
            }
            //necesito traer los menues padre, ya que al no estar relacionados a una actividad quedan fuera de los resultados de
            //la vista vUsuariosPermisos
            List<MenuWS> hijos = menuList.Where(m => m.IdPadre != null).ToList();
            foreach (MenuWS menuHijo in hijos)
            {
                crearMenuesPadres(menuList, menuHijo);
            }
            //resultado final
            mires.Menues = menuList.OrderBy(m => m.Orden).ThenBy(m => m.IdPadre).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuList"></param>
        /// <param name="menuHijo"></param>
        private void crearMenuesPadres(List<MenuWS> menuList, MenuWS menuHijo)
        {
            //A partir de la lista de menues relacionados con actividades para un usuario busca los menues padres (sin actividad)
            // y los agrega al parametro menuList
            //EntitiesSeguridad db = new EntitiesSeguridad();
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            MenuOpcion padre = session.QueryOver<MenuOpcion>().Where(m => m.CId == menuHijo.IdPadre).SingleOrDefault();
            if (padre != null)
            {
                //si el padre no esta en la lista, lo creo
                if (menuList.Where(ml => ml.Id == padre.CId).ToList().Count == 0)
                {
                    MenuWS menuPadre;
                    if (padre.MenuOpcionPadre != null)
                        menuPadre = new MenuWS(padre.CId, padre.DDescripcion, padre.DUrl, padre.MenuOpcionPadre.CId, padre.NOrden, padre.DIcono);
                    else
                        menuPadre = new MenuWS(padre.CId, padre.DDescripcion, padre.DUrl, null, padre.NOrden, padre.DIcono);
                    menuList.Add(menuPadre);
                    if (menuPadre.IdPadre != null)
                    {
                        //llamada recursiva
                        crearMenuesPadres(menuList, menuPadre);
                    }
                }
            }

        }

        /// <summary>
        /// Metodo que convierte una imagen a Thumbnails
        /// </summary>
        /// <param name="myImage"></param>
        /// <returns></returns>
        private byte[] GetThumbnails(byte[] myImage)
        {

            using (var ms = new System.IO.MemoryStream(myImage))
            {
                var image = System.Drawing.Image.FromStream(ms);

                var ratioX = (double)37 / image.Width;
                var ratioY = (double)37 / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var width = (int)(image.Width * ratio);
                var height = (int)(image.Height * ratio);

                var newImage = new System.Drawing.Bitmap(width, height);
                System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, width, height);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newImage);

                System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();

                myImage = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                return myImage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchResult"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        private static Object GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0];
            }
            else
            {
                return null;
            }
        }


    }
}
