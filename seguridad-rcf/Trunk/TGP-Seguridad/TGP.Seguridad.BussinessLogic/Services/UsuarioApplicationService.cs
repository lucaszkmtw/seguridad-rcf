using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.BussinessLogic.Dto.Usuario;
using TGP.Seguridad.Common.APIRequests;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;
using Utils;
using HelperService = TGP.Seguridad.DataAccess.Helpers.HelperService;

namespace TGP.Seguridad.BussinessLogic
{
    public class UsuarioApplicationService : SeguridadApplicationService
    {
        private static UsuarioApplicationService instance;

        //Singleton
        public static new UsuarioApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UsuarioApplicationService();
                }
                return instance;
            }
        }

        private Usuario GetUsuario(long id, string tipo)
        {
            if (tipo.Equals(UsuarioDTO.USUARIONOMINADO))
                return GetById<Nominado>(id);
            else if (tipo.Equals(UsuarioDTO.USUARIOACREEDOR))
                return GetById<Acreedor>(id);
            else
                throw new Exception("El tipo de usuario no existe");
        }

        /// <summary>
        /// Metodo que devuelve el nombre de usuario segun id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNombreUsuario(long id)
        {
            return GetById<Usuario>(id, new string[] { "NombreUsuario" }).NombreUsuario;
        }


        /// <summary>
        /// Metodo que devuelve el id y nombre de todos los usuarios
        /// </summary>
        /// <returns></returns>
        public IList<UsuarioDTO> GetIdNombreUsuarios()
        {

            string[] propiedades = new string[]
            {
                "Id",
                "NombreUsuario",
            };

            IList<Usuario> nominados = GetAll<Usuario>(propiedades);
            IList<UsuarioDTO> viewModel = nominados.Adapt<IList<UsuarioDTO>>();
            return viewModel;
        }

        /// <summary>
        /// Metodo que devuelve el id y nombre de todos los usuarios con marca DGA
        /// </summary>
        /// <returns></returns>
        public IList<UsuarioDTO> GetUsuariosDGA()
        {
            //Propiedades que necesito del usuario
            string[] propiedadesUsuario = new string[]
            {
                "Id",
                "NombreUsuario"
            };

            Search searchUsuarios = new Search(typeof(Usuario));
            searchUsuarios.AddExpression(Restrictions.Eq("SiDGA", true));
            IList<Usuario> listaUsuarios = GetByCriteria<Usuario>(searchUsuarios, propiedadesUsuario).ToList();
            IList<UsuarioDTO> viewModel = listaUsuarios.Adapt<IList<UsuarioDTO>>();
            return viewModel;
        }

        /// <summary>
        /// Metodo que devuelve un listado de usuarios nominados para la vista
        /// </summary>
        /// <param name="draw"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="search"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public IList<ListadoNominadoViewModel> GetListadoNominados(string draw, string start, string length, string search, string sortColumn, string sortColumnDir, string marcaActivo, string marcaBloqueado, string marcaTipoAuth)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "NombreUsuario",
                "DescripcionUsuario",
                "EMail",
                "SiActivo",
                "SiBloqueado",
                "DescripcionTipoAutenticacion",
                "Version",
                "CantidadIntentos"
            };

            //Armamos un criteria sobre el nominado
            ICriteria criteriaPaginado = Session().CreateCriteria<Nominado>();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;


            //Aplicamos un sort segun la columna especificada en el datatable
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "asc")
                {
                    criteriaPaginado.AddOrder(Order.Asc(sortColumn));
                }
                else
                {
                    criteriaPaginado.AddOrder(Order.Desc(sortColumn));
                }
            }
            if (!marcaActivo.Equals("-1"))
                criteriaPaginado.Add(Expression.Eq("SiActivo", bool.Parse(marcaActivo)));

            if (!marcaBloqueado.Equals("-1"))
                criteriaPaginado.Add(Expression.Eq("SiBloqueado", bool.Parse(marcaBloqueado)));

            criteriaPaginado.CreateAlias("TipoAutenticacion", "TipoAutenticacion");

            if (!marcaTipoAuth.Equals("-1"))
            {
                criteriaPaginado.Add(Expression.Eq("TipoAutenticacion.Codigo", marcaTipoAuth));
            }

            //Si el search no es vacio significa que debemos filtrar por los campos que mostramos en la lista
            if (!string.IsNullOrEmpty(search))
            {
                string searchLike = "%" + search + "%";
                criteriaPaginado
                    .Add(new Disjunction()
                        .Add(Restrictions.InsensitiveLike("NombreNominado", searchLike))
                        .Add(Restrictions.InsensitiveLike("Apellido", searchLike))
                        .Add(Restrictions.InsensitiveLike("TipoAutenticacion.Descripcion", searchLike))
                        .Add(Restrictions.InsensitiveLike("NombreUsuario", searchLike))
                        .Add(Restrictions.InsensitiveLike("EMail", searchLike)));
            }
            ICriteria criteriaCount = (ICriteria)criteriaPaginado.Clone();

            recordsTotal = criteriaCount.SetProjection(Projections.RowCount()).UniqueResult<int>();

            IList<Nominado> nominados = criteriaPaginado.SetFirstResult(skip).SetMaxResults(pageSize).List<Nominado>();


            IList<ListadoNominadoViewModel> nominadosViewModel = nominados.Adapt<IList<ListadoNominadoViewModel>>();
            if (nominadosViewModel.Count > 0)
                nominadosViewModel.First().RecordsTotal = recordsTotal;
            return nominadosViewModel;
        }



        /// <summary>
        /// Metodo que devuelve un listado de usuarios acreedores para la vista
        /// </summary>
        /// <param name="draw"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="search"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public IList<ListadoAcreedoresViewModel> GetListadoAcreedores(string draw, string start, string length, string search, string sortColumn, string sortColumnDir)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "NombreUsuario",
                "RazonSocial",
                "EMail",
                "SiActivo",
                "SiBloqueado",
                "Version"
            };

            //Armamos un criteria sobre el acreedor
            ICriteria criteriaPaginado = Session().CreateCriteria<Acreedor>();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;


            //Aplicamos un sort segun la columna especificada en el datatable
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "asc")
                {
                    criteriaPaginado.AddOrder(Order.Asc(sortColumn));
                }
                else
                {
                    criteriaPaginado.AddOrder(Order.Desc(sortColumn));
                }
            }

            //Si el search no es vacio significa que debemos filtrar por los campos que mostramos en la lista
            if (!string.IsNullOrEmpty(search))
            {
                string searchLike = "%" + search + "%";
                criteriaPaginado
                    .Add(new Disjunction()
                        .Add(Restrictions.InsensitiveLike("RazonSocial", searchLike))
                        .Add(Restrictions.InsensitiveLike("NumeroCuit", searchLike))
                        .Add(Restrictions.InsensitiveLike("NombreUsuario", searchLike))
                        .Add(Restrictions.InsensitiveLike("EMail", searchLike)));

            }
            ICriteria criteriaCount = (ICriteria)criteriaPaginado.Clone();

            recordsTotal = criteriaCount.SetProjection(Projections.RowCount()).UniqueResult<int>();

            IList<Acreedor> acreedores = criteriaPaginado.SetFirstResult(skip).SetMaxResults(pageSize).List<Acreedor>();


            IList<ListadoAcreedoresViewModel> acreedoresViewModel = acreedores.Adapt<IList<ListadoAcreedoresViewModel>>();
            if (acreedoresViewModel.Count > 0)
                acreedoresViewModel.First().RecordsTotal = recordsTotal;
            return acreedoresViewModel;
        }


        /// <summary>
        /// Metodo que marca un usuaio como inactivo y devuelve el nombre del action al que se debe redireccionar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipo">Codigo de tipo de usuario</param>
        /// <returns></returns>
        public string MarcarUsuarioInactivo(long id, string tipo)
        {

            DateTime fechaHoy = HelperService.Instance.GetDateToday();
            if (tipo.Equals(UsuarioDTO.USUARIOACREEDOR))
            {
                Acreedor acreedor = GetById<Acreedor>(id);
                acreedor.SiActivo = false;
                acreedor.SiBloqueado = true;
                acreedor.FechaBaja = fechaHoy;
                Update(acreedor);
                return "ListadoAcreedores";
            }
            else if (tipo.Equals(UsuarioDTO.USUARIONOMINADO))
            {
                Nominado nominado = GetById<Nominado>(id);
                nominado.SiActivo = false;
                nominado.SiBloqueado = true;
                nominado.FechaBaja = fechaHoy;
                Update(nominado);
                return "ListadoNominados";
            }
            else
                throw new Exception("El tipo de usuario no es correcto");


        }

        /// <summary>
        /// Metodo que elimina el usuario que corresponde al ID
        /// </summary>
        /// <param name="id"></param>
        public void DeleteUsuario(long id, string tipo, int version)
        {
            try
            {
                int versionBase = GetById<Usuario>(id, new string[] { "Version" }).Version;
                if (versionBase != version)
                    throw new Exception("El usuario fue modificado por otro usuario.");

                if (tipo.Equals(UsuarioDTO.USUARIONOMINADO))
                    this.Delete<Nominado>(id);
                else if (tipo.Equals(UsuarioDTO.USUARIOACREEDOR))
                    this.Delete<Acreedor>(id);
                else
                    throw new Exception("El tipo de usuario no es correcto");
            }
            catch (ADOException)
            {
                throw new Exception("ADO");
            }
        }


        /// <summary>
        /// Metodo que elimina el usuario acreedor que corresponde al ID
        /// </summary>
        /// <param name="idNominado"></param>
        public void DeleteAcreedor(long idAcreedor)
        {
            try
            {
                this.Delete<Acreedor>(idAcreedor);
            }
            catch (ADOException)
            {
                throw new Exception("ADO");
            }
        }

        /// <summary>
        /// Metodo que envia un mail con una clave nueva para el usuario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="version">Chequeo de concurrencia</param>
        /// <returns></returns>
        public string MailFT(long id, string tipoUsuario, int version, string tipoAutenticacion)
        {
            GeneradorPassword pass = new GeneradorPassword();
            pass.LongitudPassword = 8;
            pass.PorcentajeSimbolos = 0;
            string passNueva = pass.GetNewPassword();

            using (var trx = BeginTransaction())
            {
                try
                {
                    if (tipoUsuario.Equals(UsuarioDTO.USUARIOACREEDOR))
                    {
                        Acreedor acreedor = GetById<Acreedor>(id);

                        //Si el version no coincide elevamos una excepcion
                        if (acreedor.Version != version)
                            throw new StaleObjectStateException("Acreedor", acreedor);

                        acreedor.Contrasena = MD5Password.GetMd5Hash(passNueva);
                        acreedor.SiBloqueado = false;
                        Update(acreedor);
                        MailMessage message = new MailMessage();
                        message.To.Add(acreedor.EMail);
                        message.Bcc.Add(ConfigurationManager.AppSettings["CopiaMailAcreedor"].ToString());
                        message.Subject = "Activación de Usuario - SIGAF PBA";
                        message.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"].ToString());
                        message.IsBodyHtml = true;
                        message.Body = GetBodyMessage(acreedor.NombreUsuario, passNueva, acreedor.RazonSocial.ToUpper(), tipoAutenticacion);
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["EMAIL_SMPT"].ToString());
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_USERNAME"].ToString(), ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_PASSWORD"].ToString());
                        smtp.Send(message);

                        trx.Commit();
                        return string.Empty;
                    }
                    else if (tipoUsuario.Equals(UsuarioDTO.USUARIONOMINADO))
                    {
                        Nominado nominado = GetById<Nominado>(id);
                        //Si el version no coincide elevamos una excepcion
                        if (nominado.Version != version)
                            throw new StaleObjectStateException("Nominado", nominado);

                        //Update del usuario
                        nominado.Contrasena = MD5Password.GetMd5Hash(passNueva);
                        nominado.SiBloqueado = false;
                        //cambio la autenticacion en caso de que haya sido de tipo SIGAF 
                        if (tipoAutenticacion != null && tipoAutenticacion == "SIGAF")
                            nominado.TipoAutenticacion = GetTipoAutenticacionByCodigo(UsuarioDTO.INTERNA);

                        Update(nominado);

                        //Mail
                        MailMessage message = new MailMessage();
                        message.To.Add(nominado.EMail);
                        message.Subject = "Activación de Usuario - SIGAF PBA";
                        message.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"].ToString());
                        message.IsBodyHtml = true;
                        message.Body = GetBodyMessage(nominado.NombreUsuario, passNueva, nominado.Apellido.ToUpper() + " " + nominado.NombreNominado.ToUpper(), tipoAutenticacion);
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["EMAIL_SMPT"].ToString());
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_USERNAME"].ToString(), ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_PASSWORD"].ToString());
                        smtp.Send(message);

                        trx.Commit();
                        return string.Empty;
                    }
                    else
                    {
                        trx.Rollback();
                        throw new Exception("El tipo de usuario es incorrecto");
                    }


                }
                catch (Exception e)
                {
                    trx.Rollback();
                    throw;
                }
            }

        }

        private string GetBodyMessage(string usuario, string contraseña, string nombreCompleto, string tipoAutenticacion)
        {
            string msjTipoAutenticacion = tipoAutenticacion == "SIGAF" ?
                "<p><b>El tipo de autenticación ha cambiado a INTERNA. Si quiere volver a autenticarse por SIGAF, comunicarse con el depto. de usuarios. </b></p>" : "";
            string bodyMessage =
                                "<head>" +
                                    "<meta name='viewport' content='width=device-width' />" +
                                    "<title>Solicitud de Cambio de Contraseña </title>" +
                                    "<meta charset='utf-8'>" +
                                    "<meta http-equiv='X-UA-Compatible' content='IE=Edge'>" +
                                    "<meta name='description' content='Portal'>  " +
                                    "<link rel='shortcut icon' href='~/Content/img/icoprov.ico' /> " +
                                    "<link href='https://fonts.googleapis.com/css?family=Roboto:100,300,500,900' rel='stylesheet'>" +
                                    "<style type='text/css'>article{font-family: Arial}</style>" +
                                "</head>" +
                                "<body style='padding: 0px; margin: auto auto; font-family: Arial; color: #595959;width: 820px;'>" +
                                    "<div style='background-color: white; padding-bottom:15px; padding-top:15px;'>" +
                                        "<article style='margin: 0px 50px;'><p style='font-size:24px; font-weight:normal; color:#4a4949'>" +
                                            "<p style='font-size:24px; font-weight:normal; color:#4a4949'>" +
                                                "<span style='font-size:18px; font-weight:normal; font-style:italic; color:#4a4949'>" +
                                                   "TESORERÍA GENERAL DE LA PROVINCIA DE BUENOS AIRES" +
                                                "</span> <br />" +
                                                   "Le da la bienvenida al Portal <span style='color:#04AEC5'>SIGAF-PBA Web</span>" +
                                            "</p>" +
                                            "<p>Para acceder utilice el enlace en la página de Tesorería General, Menú SIGAF-PBA, Botón de enlace Módulo Web, o a través de la siguiente URL <a href='" + @ConfigurationManager.AppSettings["PortalURL"].ToString() + "'>" + @ConfigurationManager.AppSettings["PortalURL"].ToString() + "</a></p>" +
                                            "<p>Se ha generado el acceso para  <b>" + nombreCompleto + "</b> con las siguientes credenciales</p>" +
                                            "<p>Usuario: <b>" + usuario + "</b></p>" +
                                            "<p>Contraseña: <b>" + contraseña + "</b></p>" +
                                                msjTipoAutenticacion +
                                           "<p>" +
                                           "<small><i><b>NOTA: Recuerde cambiar la contraseña luego del primer inicio de sesión, a través de la opción disponible en el Perfil de usuario.</b></i></small>" +
                                           "</p>" +
                                        "</article>" +
                                        "<footer style='background-color: white;display: block;min-height: 70px; margin-bottom:15px;'>" +
                                            "<div style='position:relative;  min-height: 1px;  float:left;'>" +
                                                "<img alt='TGP' src='" + ConfigurationManager.AppSettings["Imagenes"] + "firma-mail.png" + "'style='padding-left: 50px;padding-right: 50px;'>" +
                                            "</div>" +
                                        "</footer>" +
                                    "</div>" +
                                "</body>";

            return bodyMessage;
        }

        /// <summary>
        /// Metodo que trae los datos que se necesitan para el dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoUsuario"></param>
        /// <returns></returns>
        public UsuarioDashboardViewModel GetUsuarioDashboard(long id, string tipoUsuario)
        {
            //Propiedades que necesito del usuario
            string[] propiedadesUsuario = new string[]
            {
                "Id",
                "NombreUsuario",
                "Avatar",
                "TipoUsuario.Descripcion",
                "TipoUsuario.Codigo",
                "CodigoAreaTelefono",
                "NumeroTelefono",
                "NumeroInterno",
                "EMail",
                "TipoAutenticacion.Codigo",
                "Version"
            };

            //Propiedades que necesito de la estructura
            string[] propiedadesLista = new string[]
            {
                "Rol.EstructuraFuncional.DescripcionEstructura",
                "Rol.EstructuraFuncional.Codigo",
                "Rol.EstructuraFuncional.Id"
            };

            //get del usuario 
            Usuario usuario = GetUsuario(id, tipoUsuario);

            //Adapt a usuario dashboard
            UsuarioDashboardViewModel usuarioDashboard = usuario.Adapt<UsuarioDashboardViewModel>();

            //listado de estructuras dash
            IList<ComboGenerico> estructuras = new List<ComboGenerico>();

            //Get de Roles Nodo Usuario
            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAliasLeftJoin(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", usuario.Id));
            IList<RolNodoUsuario> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario, propiedadesLista, null, true);
            if (rolesNodoUsuario != null)
            {
                estructuras = rolesNodoUsuario.Select(x => x.Rol.EstructuraFuncional).ToList().Adapt<IList<ComboGenerico>>();
            }
            //Adapt a estructura

            usuarioDashboard.Estructuras = estructuras;
            return usuarioDashboard;
        }

        /// <summary>
        /// Metodo que recupera el avatar del usuario y lo devuelve como fileresult
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoUsuario"></param>
        /// <returns></returns>
        public byte[] GetAvatar(long id)
        {
            Usuario usuario = GetById<Usuario>(id, new string[] { "Avatar" });

            return usuario.Avatar;

        }


        /// <summary>
        /// Metodo que retorna un viewmodel de nominado para visualizar el detalle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NominadoViewModel GetNominadoDetalle(long id)
        {
            string[] propiedadesUsuario = new string[]
            {
                "Id",
                "NumeroDni",
                "Apellido",
                "NombreNominado",
                "SiActivo",
                "SiBloqueado",
                "TipoTelefono",
                "CantidadIntentos",
                "NombreUsuario",
                "Avatar",
                "TipoAutenticacion.Codigo",
                "TipoUsuario.Codigo",
                "TipoUsuario.Descripcion",
                "CodigoAreaTelefono",
                "NumeroTelefono",
                "NumeroInterno",
                "EMail",
                "NivelJerarquico",
                "Version",
                "CodigoMsaf",
                "SiDGA",
            };

            Nominado nominado = GetById<Nominado>(id, propiedadesUsuario);
            NominadoViewModel nominadoDetalle = nominado.Adapt<NominadoViewModel>();
            return nominadoDetalle;
        }

        /// <summary>
        /// Metodo que obtiene el detalle de un acreedor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AcreedorViewModel GetAcreedorDetalle(long id)
        {
            string[] propiedadesUsuario = new string[]
            {
                "Id",
                "NumeroCuit",
                "RazonSocial",
                "SiActivo",
                "SiBloqueado",
                "TipoTelefono",
                "CantidadIntentos",
                "NombreUsuario",
                "Avatar",
                "TipoUsuario.Codigo",
                "TipoUsuario.Descripcion",
                "CodigoAreaTelefono",
                "NumeroTelefono",
                "NumeroInterno",
                "EMail",
                "Version"
            };

            Acreedor acreedor = GetById<Acreedor>(id, propiedadesUsuario);
            AcreedorViewModel acreedorDetalle = acreedor.Adapt<AcreedorViewModel>();
            return acreedorDetalle;
        }


        /// <summary>
        /// Metodo que hace update sobre el nominado con las propiedades que son editables
        /// </summary>
        /// <param name="usuario"></param>
        public void EditarNominado(NominadoViewModel usuario)
        {
            Nominado nominadoBase = GetById<Nominado>(usuario.Id);
            //Buscamos el nominado, si el version es distinto, alguien lo modificó
            if (nominadoBase.Version != usuario.Version)
                throw new Exception("El usuario ya fue modificado por otro usuario.");

            nominadoBase.Apellido = usuario.Apellido;
            nominadoBase.CodigoAreaTelefono = usuario.CodigoAreaTelefono;
            nominadoBase.EMail = usuario.EMail;
            nominadoBase.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
            if (nominadoBase.NivelJerarquico.Codigo != long.Parse(usuario.NivelJerarquico.Codigo))
                nominadoBase.NivelJerarquico = GetNivelJerarquicoByCodigo(usuario.NivelJerarquico.Codigo);
            nominadoBase.NombreNominado = usuario.NombreNominado;
            nominadoBase.NombreUsuario = usuario.NombreUsuario.ToLower();
            nominadoBase.NumeroDni = usuario.NumeroDni;
            nominadoBase.NumeroInterno = usuario.NumeroInterno;
            nominadoBase.NumeroTelefono = usuario.NumeroTelefono;
            nominadoBase.SiActivo = usuario.SiActivo;
            nominadoBase.SiBloqueado = usuario.SiBloqueado;
            nominadoBase.CantidadIntentos = usuario.CantidadIntentos;//
            nominadoBase.TelefonoCelular = usuario.TelefonoCelular;
            nominadoBase.TelefonoFijo = usuario.TelefonoFijo;
            nominadoBase.TipoTelefono = usuario.TipoTelefono;
            nominadoBase.CodigoMsaf = usuario.CodigoMsaf;
            nominadoBase.SiDGA = usuario.SiDGA;
            Update(nominadoBase);

        }

        /// <summary>
        /// Metodo que desactiva a un usuario quitandole los roles asociados
        /// </summary>
        /// <param name="nominado"></param>
        public void DesactivarUsuario(NominadoViewModel nominado)
        {
            EditarNominado(nominado);
            var rolnodoUsuario = GetRolesNodoUsuario(nominado.Id).ToList();
            foreach (var rn in rolnodoUsuario)
            {
                DesasociarRolUsuario(nominado.Id, rn.IdRol);
            }
        }


        /// <summary>
        /// Metodo que guarda la edicion de un acreedor
        /// </summary>
        /// <param name="usuario"></param>
        public void EditarAcreedor(AcreedorViewModel usuario)
        {

            Acreedor acreedorBase = GetById<Acreedor>(usuario.Id);
            //Buscamos el nominado, si el version es distinto, alguien lo modificó
            if (acreedorBase.Version != usuario.Version)
                throw new Exception("El usuario ya fue modificado por otro usuario.");

            acreedorBase.NumeroCuit = usuario.NumeroCuit;
            acreedorBase.CodigoAreaTelefono = usuario.CodigoAreaTelefono;
            acreedorBase.EMail = usuario.EMail;
            acreedorBase.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
            acreedorBase.RazonSocial = usuario.RazonSocial;
            acreedorBase.NombreUsuario = usuario.NombreUsuario.ToLower();
            acreedorBase.NumeroInterno = usuario.NumeroInterno;
            acreedorBase.NumeroTelefono = usuario.NumeroTelefono;
            acreedorBase.SiActivo = usuario.SiActivo;
            acreedorBase.SiBloqueado = usuario.SiBloqueado;
            acreedorBase.CantidadIntentos = usuario.CantidadIntentos;//
            acreedorBase.TelefonoCelular = usuario.TelefonoCelular;
            acreedorBase.TelefonoFijo = usuario.TelefonoFijo;
            acreedorBase.TipoAutenticacion = GetTipoAutenticacionByCodigo(UsuarioDTO.INTERNA);
            acreedorBase.TipoTelefono = usuario.TipoTelefono;
            Update(acreedorBase);

        }

        /// <summary>
        /// Metodo que guarda una nueva imagen como avatar si las validaciones en el cliente fueron correctas
        /// </summary>
        /// <param name="imagefile"></param>
        /// <param name="idUsuario"></param>
        /// <param name="tipoUsuario"></param>
        public void GuardarAvatarNuevo(HttpPostedFileBase imagefile, long idUsuario, string tipoUsuario)
        {
            byte[] imageByte = new byte[imagefile.ContentLength];
            imagefile.InputStream.Read(imageByte, 0, imageByte.Length);
            Usuario usuario = GetUsuario(idUsuario, tipoUsuario);
            usuario.Avatar = imageByte;
            Update(usuario);
        }

        /// <summary>
        /// Metodo que hace un update sobre la clave de un usuario
        /// </summary>
        /// <param name="cambioClave"></param>
        public void GuardarNuevaClave(CambioPasswordViewModel cambioClave)
        {
            Usuario usuario = GetUsuario(cambioClave.Id, cambioClave.CodigoTipoUsuario);
            if (usuario.Version != cambioClave.Version)
                throw new Exception("El registro ya fue modificado por otro usuario");

            usuario.Contrasena = MD5Password.GetMd5Hash(cambioClave.NuevaContraseña);
            Update(usuario);
        }

        /// <summary>
        /// Metodo que hace update sobre el tipo de autenticacion de un usuario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoAutenticacion"></param>
        /// <param name="tipoUsuario"></param>
        public void CambiarTipoAutenticacion(long id, string tipoAutenticacion, string tipoUsuario)
        {
            Search search = new Search(typeof(TipoAutenticacion));
            search.AddExpression(Restrictions.Eq("Codigo", tipoAutenticacion));
            TipoAutenticacion nuevaAutenticacion = GetByCriteria<TipoAutenticacion>(search).SingleOrDefault();
            Usuario usuario = GetUsuario(id, tipoUsuario);
            usuario.TipoAutenticacion = nuevaAutenticacion;
            Update(usuario);
        }

        /// <summary>
        /// Metodo que obtiene los roles nodo usuario segun id de usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public IList<PermisosViewModel> GetRolesNodoUsuario(long idUsuario)
        {

            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", idUsuario));
            IList<RolNodoUsuario> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario).ToList();
            //si no es admin, quito seguridad
            if (!bool.Parse(HttpContext.Current.Session["EsAdmin"].ToString()))
            {
                rolesNodoUsuario = rolesNodoUsuario.Where(rnu => rnu.Rol.EstructuraFuncional.Codigo != "SEG").ToList();
            }
            IList<PermisosViewModel> permisosUsuario = rolesNodoUsuario.GroupBy(a => new { a.Usuario, a.Rol, a.FechaUltimaOperacion }).Select(g => new PermisosViewModel
            {
                IdRol = g.Key.Rol.Id,
                CodigoRol = g.Key.Rol.Codigo,
                IdUsuario = g.Key.Usuario.Id,
                NombreUsuario = g.Key.Usuario.NombreUsuario,
                DescripcionRol = g.Key.Rol.Descripcion,
                UsuarioAlta = g.Select(X => X.UsuarioAlta.NombreUsuario).First(),
                DescripcionEstructura = g.Select(e => e.Rol.EstructuraFuncional).First().DescripcionEstructura,
                CodigoEstructura = g.Select(ec => ec.Rol.EstructuraFuncional).First().Codigo,
                NodosAsignados = g.Select(n => n.NodoFuncional.Descripcion).ToList(),
                CodigoNodosAsignados = g.Select(nc => nc.NodoFuncional.Codigo).ToList(),
                FechaUltimaOperacion = (g.Key.FechaUltimaOperacion != null) ? g.Select(fh => fh.FechaUltimaOperacion).First().Value : new DateTime(2018, 01, 01)
            }).ToList();

            return permisosUsuario;
        }

        /// <summary>
        /// Metodo que obtiene los roles nodos usuario segun usuario y codigo de estructura y los transforma en view mdoel de permisos
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public IList<PermisosViewModel> GetRolesNodoUsuario(long idUsuario, string codigoEstructura)
        {

            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol.EstructuraFuncional", "EstructuraFuncional"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", idUsuario));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
            IList<RolNodoUsuario> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario).ToList();
            IList<PermisosViewModel> permisosUsuario = rolesNodoUsuario.GroupBy(a => new { a.Usuario, a.Rol, a.FechaUltimaOperacion }).Select(g => new PermisosViewModel
            {
                IdRol = g.Key.Rol.Id,
                CodigoRol = g.Key.Rol.Codigo,
                IdUsuario = g.Key.Usuario.Id,
                NombreUsuario = g.Key.Usuario.NombreUsuario,
                DescripcionRol = g.Key.Rol.Descripcion,
                UsuarioAlta = g.Select(X => X.UsuarioAlta.NombreUsuario).First(),
                DescripcionEstructura = g.Select(e => e.Rol.EstructuraFuncional).First().DescripcionEstructura,
                CodigoEstructura = g.Select(ec => ec.Rol.EstructuraFuncional).First().Codigo,
                NodosAsignados = g.Select(n => n.NodoFuncional.Descripcion).ToList(),
                CodigoNodosAsignados = g.Select(nc => nc.NodoFuncional.Codigo).ToList(),
                FechaUltimaOperacion = (g.Key.FechaUltimaOperacion != null) ? g.Select(fh => fh.FechaUltimaOperacion).First().Value : new DateTime(2018, 01, 01)
            }).ToList();

            return permisosUsuario;
        }

        /// <summary>
        /// Metodo que devuelve un rolUsuario para la edicion
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public RolUsuarioViewModel GetRolUsuarioEditar(long idUsuario, string codRol)
        {
            //Buscamos los roles nodo usuario pertenecientes al usuario y al codigo de rol que vamos a editar
            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", idUsuario));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Rol.Codigo", codRol));
            IList<RolNodoUsuario> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario).ToList();
            RolUsuarioViewModel rolUsuario = null;
            //Creamos el objeto
            if (rolesNodoUsuario != null && rolesNodoUsuario.Count > 0)
            {
                rolUsuario = new RolUsuarioViewModel()
                {
                    IdUsuario = rolesNodoUsuario.First().Usuario.Id,
                    Estructura = rolesNodoUsuario.First().Rol.EstructuraFuncional.Codigo,
                    Nodos = rolesNodoUsuario.Select(x => x.NodoFuncional.Codigo).ToList(),
                    Rol = rolesNodoUsuario.First().Rol.Codigo,
                    RolEsMultinodo = rolesNodoUsuario.First().Rol.EsMultinodo == 1
                };
            }
            else
            {
                Search searchRol = new Search(typeof(Rol));
                searchRol.AddExpression(Restrictions.Eq("Codigo", codRol));
                Rol rol = GetByCriteria<Rol>(searchRol).FirstOrDefault();

                rolUsuario = new RolUsuarioViewModel()
                {
                    IdUsuario = idUsuario,
                    Estructura = rol.EstructuraFuncional.Codigo,
                    Rol = codRol,
                    RolEsMultinodo = rol.EsMultinodo == 1
                };
            }
            return rolUsuario;
        }



        /// <summary>
        /// Metodo que desasocia un rol de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idRol"></param>
        public void DesasociarRolUsuario(long idUsuario, long idRol)
        {

            Search searchRolNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolNodoUsuario.AddExpression(Restrictions.Eq("Rol.Id", idRol));
            searchRolNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", idUsuario));
            IList<long> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolNodoUsuario, new string[] { "Id" }).Select(r => r.Id).ToList();
            DeleteAll<RolNodoUsuario>(rolesNodoUsuario);
        }

        /// <summary>
        /// Metodo para la asociacion de un nuevo rol
        /// </summary>
        /// <param name="rolUsuario"></param>
        /// <returns></returns>
        public List<string> AsociarRolUsuario(RolUsuarioViewModel rolUsuario)
        {
            //Busco el rol a asginar
            Search searchRol = new Search(typeof(Rol));
            searchRol.AddExpression(Restrictions.Eq("Codigo", rolUsuario.Rol));
            Rol rolAsignar = GetByCriteria<Rol>(searchRol).Single();

            //Checkeo si existen actividades en conflico
            List<string> actividadesEnConflicto = SiActividadesEnConflicto(rolAsignar, rolUsuario);
            if (actividadesEnConflicto.Any())
                return actividadesEnConflicto;

            //TOmo el usuario del rol y el usuario activo en la sesion
            Usuario usuario = GetById<Usuario>(rolUsuario.IdUsuario);
            Usuario usuarioLogin = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"]);

            //Busco los nodos a asignar
            Search searchNodos = new Search(typeof(NodoFuncional));
            searchNodos.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", rolUsuario.Estructura));
            searchNodos.AddExpression(Restrictions.In("Codigo", rolUsuario.Nodos.ToArray()));
            IList<NodoFuncional> nodosAsignar = GetByCriteria<NodoFuncional>(searchNodos);

            //Creo los rolnodousuario segun los datos obtenidos
            IList<RolNodoUsuario> rolesNodousuario = new List<RolNodoUsuario>();
            foreach (NodoFuncional n in nodosAsignar)
            {
                rolesNodousuario.Add(new RolNodoUsuario() { Rol = rolAsignar, NodoFuncional = n, Usuario = usuario, UsuarioAlta = usuarioLogin, FechaUltimaOperacion = HelperService.Instance.GetDateToday() });

            }

            InsertAll(rolesNodousuario);
            return null;

        }

        /// <summary>
        /// Metodo para la edicion de un rol nodo usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        /// <param name="rolViejo"></param>
        /// <returns></returns>
        public List<string> EditarRolusuario(RolUsuarioViewModel rolUsuario, string rolViejo)
        {

            //Buscamos el rol nuevo que vamos a asignar
            Search searchRol = new Search(typeof(Rol));
            searchRol.AddExpression(Restrictions.Eq("Codigo", rolUsuario.Rol));
            Rol rolAsignar = GetByCriteria<Rol>(searchRol).Single();

            //Chequeamos si tiene actividades en coinflicto con algun otro rol que el usuario tenga asignado en esta estructura
            //Usamos el rol viejo para poder ir a buscar todos los roles menos el que estamos editando, ese nunca va a estar en conflicto
            List<string> actividadesEnConflicto = SiActividadesEnConflicto(rolAsignar, rolUsuario, true, rolViejo);
            if (actividadesEnConflicto.Any())
                return actividadesEnConflicto;

            //Obtenemos los usuarios, tanto al q asignamos el rol como el que esta logueado
            Usuario usuario = GetById<Usuario>(rolUsuario.IdUsuario);
            Usuario usuarioLogin = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"]);

            IList<RolNodoUsuario> rolesNodoUsuario = new List<RolNodoUsuario>();
            if (rolUsuario.Nodos != null)
            {
                //Buscamos todos los nodos que vamos a asignar
                Search searchNodos = new Search(typeof(NodoFuncional));
                searchNodos.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
                searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", rolUsuario.Estructura));
                searchNodos.AddExpression(Restrictions.In("Codigo", rolUsuario.Nodos.ToArray()));
                IList<NodoFuncional> nodosAsignar = GetByCriteria<NodoFuncional>(searchNodos);

                //Tomamos todos los nodos y creamos los RolNodoUsuario correspondientes

                foreach (NodoFuncional n in nodosAsignar)
                {
                    rolesNodoUsuario.Add(new RolNodoUsuario()
                    {
                        Rol = rolAsignar,
                        NodoFuncional = n,
                        Usuario = usuario,
                        UsuarioAlta = usuarioLogin,
                        FechaUltimaOperacion = HelperService.Instance.GetDateToday()
                    });
                }
            }

            //Busco los roles nodo usuario q hay q eliminar
            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", rolUsuario.IdUsuario));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Rol.Codigo", rolViejo));
            IList<RolNodoUsuario> rolesNodoUsuarioEliminar = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario).ToList();
            using (var trx = BeginTransaction())
            {
                try
                {
                    //Lo hacemos en una transaccion, con el parametro false indicamos que no queremos que el delete ni el insert
                    //realicen el commit sino que lo hacemos nosotros desde aca
                    //DeleteAll<RolNodoUsuario>(rolesNodoUsuarioEliminar.Select(x => x.Id).ToList());
                    foreach (RolNodoUsuario rnu in rolesNodoUsuarioEliminar)
                    {
                        Delete<RolNodoUsuario>(rnu.Id);
                    }
                    foreach (RolNodoUsuario rn in rolesNodoUsuario)
                    {
                        Insert(rn);
                    }
                    //InsertAll(rolesNodoUsuario);
                    trx.Commit();
                }
                catch (Exception e)
                {
                    trx.Rollback();
                    throw e;
                }
            }
            return null;
        }



        /// <summary>
        /// Metodo para guardar un nuevo nominado matcheando las propiedades del viewmodel
        /// </summary>
        /// <param name="nuevoNominado"></param>
        public void GuardarNuevoNominado(NominadoViewModel nuevoNominado)
        {
            Nominado nominadoGuardar = new Nominado()
            {
                Apellido = nuevoNominado.Apellido,
                CodigoAreaTelefono = nuevoNominado.CodigoAreaTelefono,
                Contrasena = MD5Password.GetMd5Hash(nuevoNominado.Contrasena),
                EMail = nuevoNominado.EMail,
                FechaAlta = HelperService.Instance.GetDateToday(),
                FechaUltimaOperacion = HelperService.Instance.GetDateToday(),
                NivelJerarquico = GetNivelJerarquicoByCodigo(nuevoNominado.NivelJerarquico.Codigo),
                NombreNominado = nuevoNominado.NombreNominado,
                NombreUsuario = nuevoNominado.NombreUsuario.ToLower(),
                NumeroDni = nuevoNominado.NumeroDni,
                NumeroInterno = nuevoNominado.NumeroInterno,
                NumeroTelefono = nuevoNominado.NumeroTelefono,
                SiActivo = nuevoNominado.SiActivo,
                SiBloqueado = nuevoNominado.SiBloqueado,
                TelefonoCelular = nuevoNominado.TelefonoCelular,
                TelefonoFijo = nuevoNominado.TelefonoFijo,
                TipoAutenticacion = GetTipoAutenticacionByCodigo(nuevoNominado.TipoAutenticacion.Codigo),
                TipoTelefono = nuevoNominado.TipoTelefono,
                TipoUsuario = GetTipoUsuarioByCodigo(UsuarioDTO.USUARIONOMINADO),
                UsuarioAlta = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"]),
                CodigoMsaf = nuevoNominado.CodigoMsaf,
                SiDGA = nuevoNominado.SiDGA
            };
            if (nuevoNominado.SiActivo == false)
                nominadoGuardar.FechaBaja = HelperService.Instance.GetDateToday();

            Insert(nominadoGuardar);

        }

        /// <summary>
        /// Metodo para guardar un nuevo acreedor matcheando las propiedades del viewmodel
        /// </summary>
        /// <param name="nuevoAcreedor"></param>
        public void GuardarNuevoAcreedor(AcreedorViewModel nuevoAcreedor)
        {
            Acreedor acreedorGuardar = new Acreedor()
            {
                NumeroCuit = nuevoAcreedor.NumeroCuit,
                CodigoAreaTelefono = nuevoAcreedor.CodigoAreaTelefono,
                Contrasena = MD5Password.GetMd5Hash(nuevoAcreedor.Contrasena),
                EMail = nuevoAcreedor.EMail,
                FechaAlta = HelperService.Instance.GetDateToday(),
                FechaUltimaOperacion = HelperService.Instance.GetDateToday(),
                RazonSocial = nuevoAcreedor.RazonSocial,
                NombreUsuario = nuevoAcreedor.NombreUsuario.ToLower(),
                NumeroInterno = nuevoAcreedor.NumeroInterno,
                NumeroTelefono = nuevoAcreedor.NumeroTelefono,
                SiActivo = nuevoAcreedor.SiActivo,
                SiBloqueado = nuevoAcreedor.SiBloqueado,
                TelefonoCelular = nuevoAcreedor.TelefonoCelular,
                TelefonoFijo = nuevoAcreedor.TelefonoFijo,
                TipoAutenticacion = GetTipoAutenticacionByCodigo(UsuarioDTO.INTERNA),
                TipoTelefono = nuevoAcreedor.TipoTelefono,
                TipoUsuario = GetTipoUsuarioByCodigo(UsuarioDTO.USUARIOACREEDOR),
                UsuarioAlta = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"])
            };
            if (nuevoAcreedor.SiActivo == false)
                acreedorGuardar.FechaBaja = HelperService.Instance.GetDateToday();

            Insert(acreedorGuardar);

        }

        /// <summary>
        /// Metodo que dado un codigo de usuario sigaf nos devuelve un VIew Model de nominado
        /// </summary>
        /// <param name="cUser"></param>
        /// <returns></returns>
        public NominadoViewModel CargarUsuarioSIGAF(string cUser)
        {
            Search searchUsuarioSIGAF = new Search(typeof(UsuarioSIGAFAutocomplete));
            searchUsuarioSIGAF.AddExpression(Restrictions.Eq("CUser", cUser));
            UsuarioSIGAF usuarioSIGAF = GetByCriteria<UsuarioSIGAF>(searchUsuarioSIGAF).Single();
            Char charSeparator = ' ';
            Char charSeparator2 = ',';
            int startApellido = 0;
            int startNombres = usuarioSIGAF.XcUser.IndexOf(charSeparator) + 1;
            if (startNombres <= 0)
                startNombres = usuarioSIGAF.XcUser.IndexOf(charSeparator2) + 1;
            int lengthApellido = startNombres - 1;
            int lengthNombres = usuarioSIGAF.XcUser.Length - startNombres;
            NominadoViewModel nominadoSIGAF = new NominadoViewModel()
            {

                Apellido = usuarioSIGAF.XcUser.Substring(startApellido, lengthApellido),
                NombreNominado = usuarioSIGAF.XcUser.Substring(startNombres, lengthNombres),
                NumeroDni = usuarioSIGAF.NDocumento,
                TipoTelefono = "F",
                CodigoAreaTelefono = "",
                NumeroTelefono = usuarioSIGAF.XlTelefono,
                NumeroInterno = "",
                EMail = usuarioSIGAF.XlEmail,
                TipoAutenticacion = GetTipoAutenticacionByCodigo(UsuarioDTO.SIGAF).Adapt<ComboGenerico>(),
                NivelJerarquico = GetNivelJerarquicoByCodigo(UsuarioDTO.EMPLEADO).Adapt<ComboGenerico>(),
                Contrasena = cUser + usuarioSIGAF.NDocumento.ToString(),
                Contrasena2 = cUser + usuarioSIGAF.NDocumento.ToString(),
                SiActivo = true,
                SiBloqueado = false,
                NombreUsuario = cUser.ToUpper(),
                CodigoMsaf = usuarioSIGAF.MSaf
            };
            return nominadoSIGAF;
        }

        /// <summary>
        /// Obtiene usuarios SIGAF para el combo autocomplete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList<UsuarioSIGAFAutocomplete> GetUsuariosSIGAFAutocomplete(string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return GetAll<UsuarioSIGAF>().OrderBy(x => x.XcUser).Adapt<IList<UsuarioSIGAFAutocomplete>>();
            }
            else
            {
                Search searchUsuarioSIGAF = new Search(typeof(UsuarioSIGAF));
                searchUsuarioSIGAF.AddExpression(Restrictions.Or(Restrictions.InsensitiveLike("XcUser", "%" + query + "%"), Restrictions.InsensitiveLike("CUser", "%" + query + "%")));
                IList<UsuarioSIGAF> usuariosSIGAF = GetByCriteria<UsuarioSIGAF>(searchUsuarioSIGAF);
                return (usuariosSIGAF != null && usuariosSIGAF.Count > 0) ? usuariosSIGAF.OrderBy(x => x.XcUser).Adapt<IList<UsuarioSIGAFAutocomplete>>() : new List<UsuarioSIGAFAutocomplete>();
            }
        }

        public UsuarioSigafDTO GetUsuariosSIGAF(string nombreUsuarioSigaf)
        {
            try
            {
                UsuarioSigafDTO uDto = new UsuarioSigafDTO();
                Search searchUsuarioSIGAF = new Search(typeof(UsuarioSIGAFAutocomplete));
                searchUsuarioSIGAF.AddExpression(Restrictions.Eq("CUser", nombreUsuarioSigaf.ToUpper()));
                var listusers = GetByCriteria<UsuarioSIGAF>(searchUsuarioSIGAF);
                if (listusers != null)
                {
                    var usuarioSIGAF = listusers.Single();
                    if (usuarioSIGAF != null)
                    {
                        uDto.NombreUsuario = usuarioSIGAF.CUser;
                        if (usuarioSIGAF.FechaBaja != null)
                            uDto.FechaBaja = usuarioSIGAF.FechaBaja;
                        else
                            uDto.FechaBaja = null;
                        if (usuarioSIGAF.SiBloqueado != null)
                            uDto.SiBloqueado = usuarioSIGAF.SiBloqueado;
                        else
                            uDto.SiBloqueado = false;
                    }
                    else
                        throw new Exception("No existe un Usuario SIGAF con el nombre de usuario:" + nombreUsuarioSigaf);
                    return uDto;
                }
                else
                {
                    throw new Exception("No existe un Usuario SIGAF con el nombre de usuario:" + nombreUsuarioSigaf);
                }

            }
            catch (Exception es)
            {

                throw new Exception("No existe un Usuario SIGAF con el nombre de usuario:" + nombreUsuarioSigaf + es.Message);
            }


        }


        public IList<AsignacioUsuariosViewModel> GetListConsultaAsignaciones(string usuario, string codigoEstructura, string rol)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "DescripcionEstructuraFuncional",
                "RazonSocial",
                "NombreUsuario",
                "DescripcionNodo",
                "DescripcionRol"
            };

            Search searchRolNodoUsuarios = new Search(typeof(VWRolNodoUsuario));
            if (!string.IsNullOrEmpty(codigoEstructura) && !codigoEstructura.Equals("0"))
            {
                searchRolNodoUsuarios.AddExpression(Restrictions.Eq("CodigoEstructuraFuncional", codigoEstructura));
            }
            if (!string.IsNullOrEmpty(rol) && !rol.Equals("0"))
            {
                searchRolNodoUsuarios.AddExpression(Restrictions.Eq("CodigoRol", rol));
            }
            if (!string.IsNullOrEmpty(usuario))
            {
                searchRolNodoUsuarios.AddExpression(Restrictions.Eq("IdUsuario", long.Parse(usuario)));
            }

            IList<VWRolNodoUsuario> rolesNodoUsuario = GetByCriteria<VWRolNodoUsuario>(searchRolNodoUsuarios, propiedades);

            IList<AsignacioUsuariosViewModel> resultado = rolesNodoUsuario.Adapt<IList<AsignacioUsuariosViewModel>>().GroupBy(a => new { NombreUsuario = a.NombreUsuario, Estructura = a.DescripcionEstructuraFuncional, DescripcionRol = a.DescripcionRol }).Select(g => new AsignacioUsuariosViewModel
            {
                DescripcionEstructuraFuncional = g.Key.Estructura,
                ListaDescripcionNodo = g.Select(a => a.DescripcionNodo).ToList(),
                RazonSocial = g.First().RazonSocial,
                NombreUsuario = g.Key.NombreUsuario,
                RecordsTotal = g.First().RecordsTotal,
                DescripcionRol = g.Key.DescripcionRol
            }).ToList();

            return resultado;
        }




        /// <summary>
        /// Obtiene un listado de usuarios asignados segun rol
        /// </summary>
        /// <param name="draw"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="search"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <param name="id"></param>
        /// <param name="cod"></param>
        /// <returns></returns>
        public IList<AsignacioUsuariosViewModel> GetListadoConsultaAsignaciones(string draw, string start, string length, string search, string sortColumn, string sortColumnDir, string id, string cod)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "DescripcionEstructuraFuncional",
                "RazonSocial",
                "NombreUsuario",
                "DescripcionNodo",
            };

            Search searchRolNodoUsuarios = new Search(typeof(VWRolNodoUsuario));
            if (!string.IsNullOrEmpty(cod) && !cod.Equals("0"))
            {
                searchRolNodoUsuarios.AddExpression(Restrictions.Eq("CodigoEstructuraFuncional", cod));
            }
            if (!string.IsNullOrEmpty(id))
            {
                searchRolNodoUsuarios.AddExpression(Restrictions.Eq("IdUsuario", long.Parse(id)));
            }



            IList<VWRolNodoUsuario> rolesNodoUsuario = GetByCriteria<VWRolNodoUsuario>(searchRolNodoUsuarios, propiedades);

            IList<AsignacioUsuariosViewModel> resultado = rolesNodoUsuario.Adapt<IList<AsignacioUsuariosViewModel>>().GroupBy(a => new { a.DescripcionEstructuraFuncional }).Select(g => new AsignacioUsuariosViewModel
            {
                DescripcionEstructuraFuncional = g.Key.DescripcionEstructuraFuncional,
                ListaDescripcionNodo = g.Select(a => a.DescripcionNodo).ToList(),
                RazonSocial = g.First().RazonSocial,
                NombreUsuario = g.First().NombreUsuario,
                RecordsTotal = g.First().RecordsTotal,
                DescripcionRol = g.First().DescripcionRol
            }).ToList();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            //search
            if (!(string.IsNullOrEmpty(search)))
            {
                resultado = resultado.Where(x => x.RazonSocial.IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1 || x.DescripcionNodo.IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1 ||
                                            x.DescripcionEstructuraFuncional.IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1 || x.NombreUsuario.IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1).ToList();

            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                var propertyInfo = typeof(AsignacioUsuariosViewModel).GetProperty(sortColumn);
                if (sortColumnDir == "asc")
                {
                    resultado = resultado.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                }
                else
                {
                    resultado = resultado.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                }
            }
            recordsTotal = resultado.Count();
            resultado = resultado.Skip(skip).Take(pageSize).ToList();
            if (resultado.Count > 0)
            {
                resultado.First().RecordsTotal = recordsTotal;
            }

            return resultado;
        }

        public string GetNombreDelOrganismoDelUsuario(long id)
        {
            Usuario usuario = GetById<Usuario>(id);
            if (usuario.CodigoMsaf == null || usuario.CodigoMsaf == 0)//es organo rector
            {
                return "ORGANO RECTOR";
            }
            else
            {
                try
                {
                    Search searchOrganismo = new Search(typeof(BServicio));
                    searchOrganismo.AddExpression(Expression.Eq("C_SERVICIO", usuario.CodigoMsaf));
                    BServicio bServicio = GetByCriteria<BServicio>(searchOrganismo).FirstOrDefault();
                    return bServicio.XL_SERVICIO;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
        }

        public static bool EsUsuarioAdmin()
        {
            long iduser = long.Parse(HttpContext.Current.Session["usuarioId"].ToString());
            try
            {
                Search searchRol = new Search(typeof(RolNodoUsuario));
                searchRol.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
                searchRol.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
                searchRol.AddExpression(Expression.Eq("Usuario.Id", iduser));
                searchRol.AddExpression(Expression.Eq("Rol.Codigo", "SEG-ADM"));
                List<RolNodoUsuario> rolNodoUsuario = UsuarioApplicationService.Instance.GetByCriteria<RolNodoUsuario>(searchRol).ToList();
                return (rolNodoUsuario.Count > 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Metodo que obtiene un usuario segun su nombre de usuario
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<UsuarioAutoComplete> GetUsuarioByNombreUsuarioAutoComplete(string query)
        {
            string[] propiedadesNominado = new string[]
            {
                "Id",
                "NombreUsuario",
                "Apellido",
                "NombreNominado"
            };

            string[] propiedadesAcreedor = new string[]
            {
                "Id",
                "NombreUsuario",
                "NumeroCuit",
                "RazonSocial"
            };

            Search searchUsuariosNominados = new Search(typeof(Nominado));
            searchUsuariosNominados.AddExpression(Restrictions.InsensitiveLike("NombreUsuario", "%" + query + "%"));
            IList<Nominado> nominados = GetByCriteria<Nominado>(searchUsuariosNominados, propiedadesNominado);

            Search searchUsuariosAcreedores = new Search(typeof(Acreedor));
            searchUsuariosAcreedores.AddExpression(Restrictions.InsensitiveLike("NombreUsuario", "%" + query + "%"));
            IList<Acreedor> acreedores = GetByCriteria<Acreedor>(searchUsuariosAcreedores, propiedadesAcreedor);

            List<UsuarioAutoComplete> usuarios = nominados.Adapt<List<UsuarioAutoComplete>>();
            usuarios.AddRange(acreedores.Adapt<List<UsuarioAutoComplete>>());
            return usuarios;
        }

        /// <summary>
        /// Metodo que utiliza bonos desde la api para obtener los datos de contacto de un acreedor
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ResponseGetInfoContactoUsuario GetInfoContactoUsuario(string cuit)
        {


            string[] propiedades = new string[]
            {
                "NombreUsuario",
                "EMail",
                "NumeroInterno",
                "NumeroTelefono",
                "CodigoAreaTelefono",
                "TipoTelefono"
            };


            Search searchUsuariosAcreedores = new Search(typeof(Acreedor));
            searchUsuariosAcreedores.AddExpression(Restrictions.Eq("NombreUsuario", cuit));
            IList<Acreedor> acreedores = GetByCriteria<Acreedor>(searchUsuariosAcreedores, propiedades);

            if (acreedores.Count == 0)
                throw new Exception("No existe un usuario con el cuit ingresado.");

            return new ResponseGetInfoContactoUsuario()
            {
                Email = acreedores.First().EMail,
                TipoTelefono = acreedores.First().TipoTelefono,
                CodigoAreaTelefono = acreedores.First().CodigoAreaTelefono,
                NumeroTelefono = acreedores.First().NumeroTelefono,
                NumeroInterno = acreedores.First().NumeroInterno,
            };
        }

        public void EditarInfoContacto(RequestEditarInfoAcreedor request)
        {
            Search searchUsuariosAcreedores = new Search(typeof(Acreedor));
            searchUsuariosAcreedores.AddExpression(Restrictions.Eq("NombreUsuario", request.Cuit));
            IList<Acreedor> acreedores = GetByCriteria<Acreedor>(searchUsuariosAcreedores);

            if (acreedores.Count == 0)
                throw new Exception("No existe un usuario con el cuit ingresado.");
            Acreedor acreedor = acreedores.Single();

            if (request.Email != null && request.Email != "")
                acreedor.EMail = request.Email;

            acreedor.TipoTelefono = request.TipoTelefono;
            acreedor.CodigoAreaTelefono = request.CodigoAreaTelefono;
            acreedor.NumeroTelefono = request.NumeroTelefono;
            acreedor.NumeroInterno = request.NumeroInterno;

            Update(acreedor);

        }

        /// <summary>
        /// Metodo que obtiene un ususario segun su nombre y apellido
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<UsuarioAutoComplete> GetUsuarioByNombreApellidoRazonSocial(string query)
        {
            string[] propiedadesNominado = new string[]
            {
                "Id",
                "NombreUsuario",
                "Apellido",
                "NombreNominado"
            };

            string[] propiedadesAcreedor = new string[]
            {
                "Id",
                "NombreUsuario",
                "NumeroCuit",
                "RazonSocial"
            };

            Search searchUsuariosNominados = new Search(typeof(Nominado));

            searchUsuariosNominados.AddExpression(Restrictions.Or(
                Restrictions.InsensitiveLike("Apellido", "%" + query + "%"),
                Restrictions.InsensitiveLike("NombreNominado", "%" + query + "%")));

            IList<Nominado> nominados = GetByCriteria<Nominado>(searchUsuariosNominados, propiedadesNominado);

            Search searchUsuariosAcreedores = new Search(typeof(Acreedor));
            searchUsuariosAcreedores.AddExpression(Restrictions.Or(
                Restrictions.InsensitiveLike("NumeroCuit", "%" + query + "%"),
                Restrictions.InsensitiveLike("RazonSocial", "%" + query + "%")));
            IList<Acreedor> acreedores = GetByCriteria<Acreedor>(searchUsuariosAcreedores, propiedadesAcreedor);

            List<UsuarioAutoComplete> usuarios = nominados.Adapt<List<UsuarioAutoComplete>>();
            usuarios.AddRange(acreedores.Adapt<List<UsuarioAutoComplete>>());
            return usuarios;
        }

        public IList<ComboGenerico> GetCombosRol(string codEstructura)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Descripcion"
            };
            Search searchRoles = new Search(typeof(Rol));
            searchRoles.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codEstructura));
            List<ComboGenerico> roles = new List<ComboGenerico>();
            roles.Add(new ComboGenerico()
            {
                Id = 0,
                Codigo = "0",
                Descripcion = "-- TODOS --"
            });

            roles.AddRange(GetByCriteria<Rol>(searchRoles, propiedades).Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return roles;
        }

        #region Privados
        /// <summary>
        /// Metodo que obtiene un nivel jerarquico segun codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        private NivelJerarquico GetNivelJerarquicoByCodigo(string codigo)
        {
            Search searchNivelJerarquico = new Search(typeof(NivelJerarquico));
            searchNivelJerarquico.AddExpression(Restrictions.Eq("Codigo", long.Parse(codigo)));
            return GetByCriteria<NivelJerarquico>(searchNivelJerarquico).Single();
        }

        /// <summary>
        /// Metodo que obtiene un tipo de autenticacion segun codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        private TipoAutenticacion GetTipoAutenticacionByCodigo(string codigo)
        {
            Search searchTipoAutenticacion = new Search(typeof(TipoAutenticacion));
            searchTipoAutenticacion.AddExpression(Restrictions.Eq("Codigo", codigo));
            return GetByCriteria<TipoAutenticacion>(searchTipoAutenticacion).Single();
        }

        /// <summary>
        /// Metodo que obtiene un tipo de usuario segun codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        private TipoUsuario GetTipoUsuarioByCodigo(string codigo)
        {
            Search searchTipoUsuario = new Search(typeof(TipoUsuario));
            searchTipoUsuario.AddExpression(Restrictions.Eq("Codigo", codigo));
            return GetByCriteria<TipoUsuario>(searchTipoUsuario).Single();
        }

        /// <summary>
        /// Metodo que chequea si un usuario ya tiene actividades asociadas que se repitan en el nuevo rol
        /// </summary>
        /// <param name="rolAsignar"></param>
        /// <param name="rolUsuario"></param>
        /// <returns></returns>
        private List<string> SiActividadesEnConflicto(Rol rolAsignar, RolUsuarioViewModel rolUsuario, bool esEditar = false, string rolViejo = null)
        {
            //Traemos todos los roles asignados al usuario en esta estructura
            Search searchRolesAsignados = new Search(typeof(RolNodoUsuario));
            searchRolesAsignados.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchRolesAsignados.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesAsignados.AddAlias(new KeyValuePair<string, string>("Rol.EstructuraFuncional", "EstructuraFuncional"));
            searchRolesAsignados.AddExpression(Restrictions.Eq("Usuario.Id", rolUsuario.IdUsuario));
            searchRolesAsignados.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", rolUsuario.Estructura));


            IList<RolNodoUsuario> rolesAsignadosUsuario = GetByCriteria<RolNodoUsuario>(searchRolesAsignados);
            //Si estamos verificando esto para la edicion, entonces debemos dejar fuera de la comparacion al rol que estamos editando
            if (esEditar)
                rolesAsignadosUsuario = rolesAsignadosUsuario.Where(x => x.Rol.Codigo != rolViejo).ToList();


            List<string> actividadesEnConflicto = new List<string>();
            List<RolActividad> rolesActividades = new List<RolActividad>();
            //Por cada rol nodo usuario asignado
            foreach (RolNodoUsuario rnu in rolesAsignadosUsuario)
            {
                //Por cada rol actividad que tenga ese rol
                foreach (RolActividad ra in rnu.Rol.Actividades)
                {
                    //Si el rol nuevo tiene alguna actividad que tambien tienen los otros roles asignados
                    if (rolAsignar.Actividades.Any(x => x.Actividad.Codigo.Equals(ra.Actividad.Codigo)))
                    {
                        //Lo sumamos al listado, significa q ya tenemos un conflicto
                        rolesActividades.Add(ra);
                    }
                }
            }
            //Si tenemos algun conflicto
            if (rolesActividades.Any())
            {
                //Por cada actividad en el conflicto creamos un mensaje
                foreach (RolActividad ra in rolesActividades)
                {
                    string error = "No se puede asignar el ROL " + ra.Rol.Descripcion + " ya que la actividad <strong>" + ra.Actividad.Descripcion + "</strong> se encuentra en otro rol.";
                    actividadesEnConflicto.Add(error);

                }
            }
            return actividadesEnConflicto;
        }
        #endregion

        /// <summary>
        /// Metodo que trae activiades segun un array de codigos de actividad
        /// </summary>
        /// <param name="codigos"></param>
        /// <returns></returns>
        private IList<Actividad> GetActividadesPorCodigo(string[] codigos)
        {
            Search searchActividades = new Search(typeof(Actividad));
            searchActividades.AddExpression(Restrictions.In("Codigo", codigos));
            return GetByCriteria<Actividad>(searchActividades);
        }

        /// <summary>
        /// Metodo que trae una actividad segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        private Actividad GetActividadesPorCodigo(string codigo)
        {
            Search searchActividades = new Search(typeof(Actividad));
            searchActividades.AddExpression(Restrictions.Eq("Codigo", codigo));
            return GetByCriteria<Actividad>(searchActividades).SingleOrDefault();
        }

        /// <summary>
        /// Metodo que trae una estructura segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        private EstructuraFuncional GetEstructuraPorCodigo(string codigo)
        {
            Search searchEstructura = new Search(typeof(EstructuraFuncional));
            searchEstructura.AddExpression(Restrictions.Eq("Codigo", codigo));
            return GetByCriteria<EstructuraFuncional>(searchEstructura).SingleOrDefault();
        }

    }
}
