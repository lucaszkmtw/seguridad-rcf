using Microsoft.AspNet.SignalR.Client;
using NHibernate;
using NHibernate.Criterion;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Web.UI.WebControls;
using TGP.WSS.Models.DTO;
using TGP.WSS.Models.Requerimiento;
using TGP.WSS.Models.Resultado;


namespace TGP.WSS
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Seguridad : LibreriaSeguridad
    {
        #region // Metodos Publicos //

        /// <summary>
        /// Metodo utilizado para probar el servicio
        /// </summary>
        /// <returns>Nro de version del Servicio</returns>
        public string Eco()
        {
            return ConfigurationManager.AppSettings["version"].ToString() + " - " + ConfigurationManager.AppSettings["dateVersion"].ToString();
        }

        #region // Metodos destinados favoritear Items //
        public ResultadoDTO Favoritear(RequerimientoFavoritearDTO requerimiento, string token)
        {
            ResultadoDTO resultado = new ResultadoDTO();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);
                
                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        //Este metodo deberia marcar como favorito a un item del sistema
                        //el item es identificado por nombre de clase y objetoID
                        //Si existe la marca tenemos que borrar
                        //sino la generamos
                        //con esto hacemos dos acciones en una
                        var nombreClase = session.QueryOver<NombreClase>().Where(n => n.Nombre == requerimiento.NombreClase).SingleOrDefault();
                        Services.UsuarioService usuarioService = new Services.UsuarioService();
                        var usuario = usuarioService.ObtenerPorNombreUsuario(requerimiento.NombreUsuario);
                        var marca = session.QueryOver<Marca>().Where(m => m.NombreClase.Id == nombreClase.Id && m.ObjetoId == requerimiento.ObjetoID && m.Usuario.CId == usuario.CId).SingleOrDefault();
                        if (marca != null) //Si hay marca la borro
                        {
                            session.Delete(marca);
                            session.Flush();
                            resultado.Descripcion = "OK - Marca quitada";
                        }
                        else //Sino hay la genero
                        {
                            Marca m = new Marca();
                            m.NombreClase = nombreClase;
                            m.ObjetoId = requerimiento.ObjetoID;
                            m.FechaRegistro = Services.DateUtilsService.ObtenerFechaActual();
                            m.Usuario = usuario;
                            session.Save(m);
                            session.Flush();
                            resultado.Descripcion = "OK - Marca agregada";
                        }
                        resultado.Codigo = "1";


                    }
                    catch (Exception eGeneral)
                    {
                        resultado.Codigo = "999";
                        resultado.Descripcion = "Error General del Servicio, más detalle: " + eGeneral.Message;
                    }

                    return resultado;
                }
                else
                {
                    resultado.Descripcion = "Error inesperado comuniquese con el administrador. Token invalido.";
                    return resultado;
                }
            }
            catch (Exception e)
            {
                resultado.Codigo = "";
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                resultado.Descripcion = "Error inesperado comuniquese con el administrador. " + error;
                return resultado;
            }
        }

        /// <summary>
        /// Metodo que devuelve los items favoritedados por Clase
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        public ResultadoObtenerFavoritosDTO ObtenerFavoritos(RequerimientoObtenerFavoritosDTO requerimiento, string token)
        {
            ResultadoObtenerFavoritosDTO resultado = new ResultadoObtenerFavoritosDTO();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        var nombreClase = session.QueryOver<NombreClase>().Where(n => n.Nombre == requerimiento.NombreClase).SingleOrDefault();
                        Services.UsuarioService usuarioService = new Services.UsuarioService();
                        var usuario = usuarioService.ObtenerPorNombreUsuario(requerimiento.NombreUsuario);
                        List<Marca> marcas = session.QueryOver<Marca>().Where(m => m.NombreClase.Id == nombreClase.Id && m.Usuario.CId == usuario.CId).List<Marca>().ToList();
                        if (marcas.Count > 0) //Si hay marca la borro
                        {
                            foreach (Marca m in marcas)
                            {
                                FavoritoDTO f = new FavoritoDTO();
                                f.NombreClase = m.NombreClase.Nombre;
                                f.ObjetoID = m.ObjetoId;
                                resultado.Favoritos.Add(f);
                            }

                        }
                        else
                            resultado.Favoritos = null;
                        resultado.Codigo = "1";
                        resultado.Descripcion = "OK";
                    }
                    catch (Exception eGeneral)
                    {
                        resultado.Codigo = "999";
                        resultado.Descripcion = "Error General del Servicio, más detalle: " + eGeneral.Message;
                    }
                    return resultado;
                }
                else
                {
                    resultado.Codigo = "";
                    resultado.Descripcion = "Error inesperado comuniquese con el administrador. Token invalido.";
                    return resultado;
                }
            }
            catch (Exception e)
            {
                resultado.Codigo = "";
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                resultado.Descripcion = "Error inesperado comuniquese con el administrador. " + error;
                return resultado;
            }
        }

        #endregion

        /// <summary>
        /// Metodo que obtiene los datos de un Usuario registrado ebn TGP mediante el envio de su ID
        /// Este servicio es de usuario Interno
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario autneticado en la aplicacion solicitante</param>
        /// <param name="claveServicio">clave de conexion con el servicio</param>
        /// <param name="usuarioConsultaID">Id del usuario a consultar</param>
        /// <returns></returns>
        public ResultadoObtenerUsuarioDTO ObtenerUsuarioNominado(RequerimientoObtenerUsuarioDTO requerimiento, string token)
        {
            ResultadoObtenerUsuarioDTO resultado = new ResultadoObtenerUsuarioDTO();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        //Valido que los parametros de entrada no sean nulos
                        if (SiParametroValidoObtenerUsuario(requerimiento))
                        {
                            //Valido la credencial de servicio
                            if (SiCredencialesValidas(requerimiento.NombreUsuario, requerimiento.ClaveServicio) == true)
                            {
                                try
                                {
                                    try
                                    {
                                        var usuarioNominadoDTO = Services.UsuarioService.ObtenerUsuarioNominado(requerimiento.UsuarioConsultaID, requerimiento.EstructuraFuncional);
                                        resultado.Codigo = "1";
                                        resultado.Descripcion = "OK";
                                        resultado.UsuarioNominadoDTO = usuarioNominadoDTO;
                                    }
                                    catch (Exception eObtenerUsuario)
                                    {
                                        resultado.Codigo = "100";
                                        resultado.Descripcion = eObtenerUsuario.Message;
                                        resultado.UsuarioNominadoDTO = null;
                                    }
                                }
                                catch (Exception eCargaUsuarioNominado)
                                {
                                    resultado.Codigo = "101";
                                    resultado.Descripcion = "Se produjo un problema en la carga del UsuarioNominado: Más Detalle " + eCargaUsuarioNominado.Message;
                                    resultado.UsuarioNominadoDTO = null;
                                }
                            }
                            else
                            {
                                resultado.Codigo = "102";
                                resultado.Descripcion = "El nombre de usuario o clave del servicio NO son válidas.";
                                resultado.UsuarioNominadoDTO = null;
                            }
                        }
                        else
                        {
                            resultado.Codigo = "103";
                            resultado.Descripcion = "Parámetros NO válidos. Verifique haber ingresado todos los parámetros solicitados.";
                            resultado.UsuarioNominadoDTO = null;
                        }
                    }
                    catch (Exception eGeneral)
                    {
                        resultado.Codigo = "999";
                        resultado.Descripcion = "Error General del Servicio, más detalle: " + eGeneral.Message;
                        resultado.UsuarioNominadoDTO = null;
                    }
                    return resultado;
                }
                else
                {
                    resultado.Descripcion = "Error inesperado comuniquese con el administrador. Token invalido.";
                    return resultado;
                }
            }
            catch (Exception e)
            {
                resultado.Codigo = "";
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                resultado.Descripcion = "Error inesperado comuniquese con el administrador. " + error;
                return resultado;
            }
        }

        /// <summary>
        /// Metodo que recupera todos los usuarios de un rol especifico
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        public ResultadoObtenerUsuariosNominadosPorRolDTO ObtenerUsuariosNominadosPorRol(RequerimientoObtenerUsuariosNominadosPorRolDTO requerimiento, string token)
        {
            ResultadoObtenerUsuariosNominadosPorRolDTO resultado = new ResultadoObtenerUsuariosNominadosPorRolDTO();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        //Valido que los parametros de entrada no sean nulos
                        if (SiParametroValidoObtenerUsuariosNominadosPorRol(requerimiento))
                        {
                            //Valido la credencial de servicio
                            if (SiCredencialesValidas(requerimiento.NombreUsuario, requerimiento.ClaveServicio) == true)
                            {
                                try
                                {

                                    var usuariosNominadosDTO = Services.UsuarioService.ObtenerUsuariosNominadoPorRol(requerimiento.CodigoRol, requerimiento.EstructuraFuncional);
                                    if (usuariosNominadosDTO != null)
                                    {
                                        resultado.Codigo = "1";
                                        resultado.Descripcion = "OK";
                                        resultado.UsuariosNominadosDTO = usuariosNominadosDTO;
                                    }
                                    else
                                    {
                                        resultado.Codigo = "100";
                                        resultado.Descripcion = "No existe un usuarios nominados para el codigo de rol enviado";
                                        resultado.UsuariosNominadosDTO = null;
                                    }
                                }
                                catch (Exception eCargaUsuarioNominado)
                                {
                                    resultado.Codigo = "101";
                                    resultado.Descripcion = "Se produjo un problema en la carga del UsuarioNominado: Más Detalle " + eCargaUsuarioNominado.Message;
                                    resultado.UsuariosNominadosDTO = null;
                                }
                            }
                            else
                            {
                                resultado.Codigo = "102";
                                resultado.Descripcion = "El nombre de usuario o clave del servicio NO son válidas.";
                                resultado.UsuariosNominadosDTO = null;
                            }
                        }
                        else
                        {
                            resultado.Codigo = "103";
                            resultado.Descripcion = "Parámetros NO válidos. Verifique haber ingresado todos los parámetros solicitados.";
                            resultado.UsuariosNominadosDTO = null;
                        }
                    }
                    catch (Exception eGeneral)
                    {
                        resultado.Codigo = "999";
                        resultado.Descripcion = "Error General del Servicio, más detalle: " + eGeneral.Message;
                        resultado.UsuariosNominadosDTO = null;
                    }

                    return resultado;
                }
                else
                {
                    resultado.Descripcion = "Error inesperado comuniquese con el administrador. Token invalido.";
                    return resultado;
                }
            }
            catch (Exception e)
            {
                resultado.Codigo = "";
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                resultado.Descripcion = "Error inesperado comuniquese con el administrador. " + error;
                return resultado;
            }

        }


        /// <summary>
        /// Metodo de autenticacion de aplicaciones
        /// </summary>
        /// <param name="usu">Nombre de usuario</param>
        /// <param name="pass">Contraseña</param>
        /// <param name="codigo_estruc_func">Codigo de la estructura funcional a acceder</param>
        /// <returns></returns>
        public ResultadoLogin Login(string usu, string pass, string codigo_estruc_func, string token)
        {
            ResultadoLogin mires = new ResultadoLogin();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);
                
                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {                    
                    List<VWUsuarioPermisos> permisosDelUsuario = null;
                    var usuarioSearchLower = usu.ToLower();
                    var usuarioSearchUpper = usu.ToUpper();
                    Usuario usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == usuarioSearchUpper || u.DUsuario == usuarioSearchLower).SingleOrDefault();
                    try
                    {
                        if (usuario != null)
                        {
                            //lo reasigno pq no se si esta con mayusculas o minusculas, pq luego se va a usar en otras consultas
                            usu = usuario.DUsuario;

                            if (usuario.isNominado())
                            {
                                mires.CuitNominado = ((Nominado)usuario).Cuit;
                                mires.NombreNominado = ((Nominado)usuario).DNombre;
                                mires.ApellidoNominado = ((Nominado)usuario).DApellido;
                            }
                            if (usuario.isAcreedor())
                                mires.RazonSocialAcreedor = ((Acreedor)usuario).DRazonSocial;
                            //String cod_autenticacion = db.SEG_TIPO_AUTENTICACION.Where(t => t.C_ID == usuario.C_ID_TIPO_AUTENTICACION).Select(t => t.C_CODIGO).SingleOrDefault();
                            var tipoAutenticacion = session.QueryOver<TipoAutenticacion>().Where(t => t.CId == usuario.TipoAutenticacion.CId).SingleOrDefault();
                            mires.CodTipoAutenticacion = tipoAutenticacion.CCodigo;
                            switch (mires.CodTipoAutenticacion)
                            {    
                                case "1": //TIPO DE AUTENTICACION INTERNA
                                    pass = MD5Password.GetMd5Hash(pass);
                                    //vuelvo a buscar el usuario para poder chequear la pass
                                    usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu && u.DPassword == pass).SingleOrDefault();
                                    if (usuario != null)
                                    {
                                        //permisosDelUsuario = db.VW_USUARIO_PERMISOS.Where(u => u.D_USUARIO == usu && u.D_PASSWORD == pass && u.C_CODIGO_ESTFUNC == codigo_estruc_func).ToList();
                                        permisosDelUsuario = session.QueryOver<VWUsuarioPermisos>().Where(u => u.DUsuario == usu && u.CEstructuraFuncional == codigo_estruc_func).List<VWUsuarioPermisos>().ToList();
                                        if (permisosDelUsuario.Count > 0 || (permisosDelUsuario.Count == 0 && codigo_estruc_func == "POR"))
                                        {
                                            mires.Valido = true;
                                            //Avatar
                                            //if (usuario.BAvatar != null)
                                            //    //mires.Avatar = GetThumbnails(usuario.BAvatar);
                                            mires.UsuarioId = usuario.CId;
                                        }
                                        else
                                        {
                                            mires.Msj = "Su usuario no tiene los permisos mínimos para el acceso.";
                                            mires.Valido = false;
                                        }
                                    }
                                    else
                                    {
                                        mires.Valido = false;
                                        mires.Msj = "El nombre de usuario o la contraseña especificados son incorrectos";
                                    }
                                    break;

                                case "2": //TIPO DE AUTENTACION SIGAF
                                          //usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu.ToLower()).SingleOrDefault();
                                    if (usuario != null)
                                    {
                                        if (usuario.MActivo == true)
                                        {
                                            //SiUSuarioSIGAF Desbloqueado?
                                            var respuestaValidacionUsuarioSigaf = SiUsuarioSIGAFValido(usuario, session);
                                            if (respuestaValidacionUsuarioSigaf.Codigo == CodigoRespuestaValidarUsuarioSIGAF.OK)
                                            {
                                                //validar conexion
                                                string stringConn = ConfigurationManager.AppSettings["conOracle"];

                                                string testConn = "Data Source=" + stringConn + ";Persist Security Info=True;User ID=" + usu + "; Password=" + pass + ";";// +"; Unicode=True";

                                                using (OracleConnection oConn = new OracleConnection(testConn))
                                                {
                                                    try
                                                    {

                                                        oConn.Open();
                                                        if ((oConn.State & System.Data.ConnectionState.Open) > 0)
                                                        {
                                                            permisosDelUsuario = session.QueryOver<VWUsuarioPermisos>().Where(u => u.DUsuario == usu.ToLower() && u.CEstructuraFuncional == codigo_estruc_func).List<VWUsuarioPermisos>().ToList();
                                                            var portal = session.QueryOver<EstructuraFuncional>().Where(p => p.CCodigo == "POR").SingleOrDefault();
                                                            if (permisosDelUsuario.Count <= 0 && codigo_estruc_func != portal.CCodigo)
                                                            {
                                                                mires.Msj = "Su usuario no tiene los permisos mínimos para el acceso." + permisosDelUsuario.Count;
                                                                mires.Valido = false;
                                                            }
                                                            else
                                                            {

                                                                if (usuario.BAvatar != null)
                                                                    mires.Avatar = GetThumbnails(usuario.BAvatar);
                                                                mires.UsuarioId = usuario.CId;
                                                                mires.Valido = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //modificar intentos
                                                            mires.Valido = false;
                                                            mires.Msj = "El nombre de usuario o la contraseña especificados son incorrectos";
                                                        }

                                                    }
                                                    catch (OracleException ex)
                                                    {
                                                        mires.Valido = false;
                                                        mires.Msj = "El nombre de usuario o la contraseña especificados son incorrectos";
                                                    }
                                                    finally
                                                    {
                                                        oConn.Close();
                                                    }
                                                }
                                            }
                                            else //
                                            {
                                                mires.Msj = respuestaValidacionUsuarioSigaf.Descripcion;
                                                mires.Valido = false;
                                            }
                                        }
                                        else
                                        {
                                            mires.Msj = "El usuario se encuentra inactivo, contactese con el administrador";
                                            mires.Valido = false;
                                        }
                                    }
                                    else
                                    {
                                        mires.Valido = false;
                                        mires.Msj = "El nombre de usuario no existe";
                                    }

                                    break;
                            }
                            if (mires.Valido)
                            {
                                if (usuario.MBloqueado == true || usuario.MActivo != true)
                                {
                                    mires.Msj = "El usuario se encuentra bloqueado o inactivo, contactese con el administrador";
                                    mires.Valido = false;
                                }
                                else
                                {
                                    ObtenerPermisosAsociados(mires, permisosDelUsuario);
                                    ObtenerMenuOpcionAsociado(mires, permisosDelUsuario.Where(p => p.MenuOpcion != null).ToList());
                                }
                            }
                        }
                        else
                        {
                            mires.Msj = "El nombre de usuario o la contraseña especificados son incorrectos";
                        }
                        return (mires);
                    }
                    catch (Exception ex)
                    {
                        mires.Msj = "Error inesperado comuniquese con el administrador";
                        mires.Exception = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        return (mires);
                    }
                }
                else
                {
                    mires.Msj = "Error inesperado comuniquese con el administrador. Token invalido.";
                    mires.Exception = "El token recibido por parametro en el WS es incorrecto.";
                    return mires;
                }
            }
            catch (Exception e)
            {
                mires.Msj = "Error inesperado comuniquese con el administrador";
                mires.Exception = e.InnerException != null ? e.InnerException.Message : e.Message;
                return mires;
            }


        }

        private RespuestaValidarUsuarioSIGAFDTO SiUsuarioSIGAFValido(Usuario usuario, ISession session)
        {

            RespuestaValidarUsuarioSIGAFDTO respuesta = new RespuestaValidarUsuarioSIGAFDTO();
            UsuarioSIGAF usuarioSIGAF = session.QueryOver<UsuarioSIGAF>().Where(uSigaf => uSigaf.CUser == usuario.DUsuario.ToUpper()).SingleOrDefault();
            if (usuarioSIGAF != null)
            {
                if (usuarioSIGAF.FechaBaja != null)
                {
                    respuesta.Codigo = CodigoRespuestaValidarUsuarioSIGAF.DadoBaja;
                    respuesta.Descripcion = "Su usuario se encuentra dado de baja en SIGAF y en el Portal SIGAF TGP";
                    //Deberia chequear que este inactivo aca tambien
                    if (usuario.MActivo == true)
                    {
                        MarcarComoInactivoUsuario(usuario, session);
                    }
                    return respuesta;
                }
                else if (usuarioSIGAF.SiBloqueado == true)
                {
                    respuesta.Codigo = CodigoRespuestaValidarUsuarioSIGAF.Bloqueado;
                    respuesta.Descripcion = "Su usuario se encuentra bloqueado en SIGAF, comuníquese con Mesa de Ayuda SIGAF";
                    return respuesta;
                }

            }
            else
            {
                respuesta.Codigo = CodigoRespuestaValidarUsuarioSIGAF.Error;
                respuesta.Descripcion = "Su usuario no se encuentra registrado en SIGAF, comuníquese con Mesa de Ayuda SIGAF";
                return respuesta;
            }
            respuesta.Codigo = CodigoRespuestaValidarUsuarioSIGAF.OK;
            return respuesta;
        }

        /// <summary>
        /// Metodo que marca como inactivo a un usuario quitando ademas los roles asociados
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="session"></param>
        private void MarcarComoInactivoUsuario(Usuario usuario, ISession session)
        {
            var roleNodos = session.QueryOver<RolNodoUsuario>().Where(u => u.Usuario.CId == usuario.CId).List<RolNodoUsuario>().ToList();
            if (roleNodos.Count > 0)
            {
                foreach (var rn in roleNodos)
                {
                    session.Delete(rn);
                    session.Flush();
                }
            }
            usuario.MActivo = false;
            session.Update(usuario);
            session.Flush();
        }

        /// <summary>
        /// Metodo de autenticacion externa
        /// </summary>
        /// <param name="usu">Nombre de usuario</param>
        /// <param name="codigo_estruc_func">Codigo de la estructura funcional a acceder</param>
        /// <returns></returns>
        public ResultadoLogin ExternalLogin(string usu, string codigo_estruc_func, string token)
        {
            ResultadoLogin mires = new ResultadoLogin();

            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    List<VWUsuarioPermisos> permisosDelUsuario = null;
                    try
                    {
                        Usuario usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu).SingleOrDefault();
                        if (usuario != null)
                        {

                            if (usuario.isNominado())
                            {
                                mires.CuitNominado = ((Nominado)usuario).Cuit;
                                mires.NombreNominado = ((Nominado)usuario).DNombre;
                                mires.ApellidoNominado = ((Nominado)usuario).DApellido;
                            }
                            if (usuario.isAcreedor())
                                mires.RazonSocialAcreedor = ((Acreedor)usuario).DRazonSocial;
                            //permisosDelUsuario = db.VW_USUARIO_PERMISOS.Where(u => u.D_USUARIO == usu && u.C_CODIGO_ESTFUNC == codigo_estruc_func).ToList();
                            permisosDelUsuario = session.QueryOver<VWUsuarioPermisos>().Where(u => u.DUsuario == usu && u.CEstructuraFuncional == codigo_estruc_func).List<VWUsuarioPermisos>().ToList();
                            if (permisosDelUsuario.Count > 0 || (permisosDelUsuario.Count == 0 && codigo_estruc_func == "POR"))
                            {
                                mires.Valido = true;
                                //Avatar
                                //if (usuario.BAvatar != null)
                                //    mires.Avatar = GetThumbnails(usuario.BAvatar);
                                mires.UsuarioId = usuario.CId;
                            }
                            else
                            {
                                mires.Msj = "Su usuario no tiene los permisos mínimos para el acceso.";
                                mires.Valido = false;
                            }
                            if (mires.Valido)
                            {
                                if (usuario.MBloqueado == true || usuario.MActivo != true)
                                {
                                    mires.Msj = "El usuario se encuentra bloqueado o inactivo, contactese con el administrador";
                                    mires.Valido = false;
                                }
                                else
                                {
                                    ObtenerPermisosAsociados(mires, permisosDelUsuario);
                                    ObtenerMenuOpcionAsociado(mires, permisosDelUsuario.Where(p => p.MenuOpcion != null).ToList());
                                }
                            }
                        }
                        else
                        {
                            mires.Msj = "El nombre de usuario es incorrecto";
                        }
                        return (mires);
                    }
                    catch (Exception ex)
                    {
                        mires.Msj = "Error inesperado comuniquese con el administrador";
                        mires.Exception = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        return (mires);
                    }
                }
                else
                {
                    mires.Msj = "Error inesperado comuniquese con el administrador. Token invalido.";
                    mires.Exception = "El token recibido por parametro en el WS es incorrecto.";
                    return mires;
                }
            }
            catch (Exception e)
            {
                mires.Msj = e.InnerException != null ? e.InnerException.Message : e.Message;
                mires.Exception = e.InnerException != null ? e.InnerException.Message : e.Message;
                return mires;
            }




        }

        /// <summary>
        /// Metodo que asocia la contraseña nueva generada desde el portal a un usuario en Active Directory
        /// </summary>
        /// <param name="usu"></param>
        /// <param name="passNueva"></param>
        /// <returns></returns>
        public ResultadoSetNewPasswordByActiveDirectory SetNewPasswordByActiveDirectory(string usu, string passNueva, string token)
        {
            ResultadoSetNewPasswordByActiveDirectory mires = new ResultadoSetNewPasswordByActiveDirectory();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    if (passNueva.Length >= 8 && passNueva.Length <= 20)
                    {
                        string ldapServer = ConfigurationManager.AppSettings["LDAPSERVER"];
                        string targetOU = ConfigurationManager.AppSettings["TARGET_OU"];

                        DirectoryEntry userEntry = null;
                        //el cambio debe ser con un usuario administrador
                        DirectoryEntry uEntry = new DirectoryEntry("LDAP://" + ldapServer + "/" + targetOU, "portal", "tgpadmin", AuthenticationTypes.None);

                        DirectorySearcher searcher = new DirectorySearcher(uEntry);
                        searcher.Filter = String.Format("sAMAccountName={0}", usu);
                        searcher.SearchScope = SearchScope.Subtree;
                        searcher.CacheResults = false;
                        SearchResult searchResult = searcher.FindOne();
                        if (searchResult != null)
                        {
                            userEntry = searchResult.GetDirectoryEntry();

                            try
                            {
                                userEntry.Invoke("SetPassword", passNueva);
                                userEntry.CommitChanges();
                                //mensaje de respuesta
                                mires.Msj = "OK";
                                mires.Valido = true;
                                return (mires);
                            }
                            catch (TargetInvocationException t)
                            {
                                mires.Msj = "La contraseña no ha podido ser modificada. Las causas pueden ser: </br> <li><ul>Contraseña actual incorrecta. </ul><ul> La nueva contraseña no cumple con la politica de seguridad.</ul></li>";
                                mires.Valido = false;
                                return (mires);
                            }
                        }
                        else
                        {
                            mires.Msj = "No se encontro el usuario en Active Directory";
                            mires.Valido = false;
                            return (mires);
                        }
                    }
                    else
                    {
                        mires.Valido = false;
                        mires.Msj = "La contraseña nueva especificada tiene menos de 8 o mas de 20 caracteres";
                        return (mires);
                    }
                }
                else
                {
                    mires.Valido = false;
                    mires.Msj = "Error inesperado comuniquese con el administrador. Token invalido.";
                    mires.Exception = "El token recibido por parametro en el WS es incorrecto.";
                    return mires;
                }
            }
            catch (Exception e)
            {
                mires.Msj = "Error inesperado comuniquese con el administrador.";
                mires.Exception = e.InnerException != null ? e.InnerException.Message : e.Message;
                return mires;
            }

        }

        /// <summary>
        /// Metodo que permite el cambio de contraseña
        /// </summary>
        /// <param name="usu">Nombre de usuario</param>
        /// <param name="passActualMD5">Contraseña actual</param>
        /// <param name="passNueva">Nueva Contraseña</param>
        /// <returns></returns>
        public ResultadoChangePassword ChangePassword(string usu, string passActual, string passNueva, string token)
        {
            ResultadoChangePassword mires = new ResultadoChangePassword();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    if (passNueva.Length >= 8 && passNueva.Length <= 20)
                    {
                        try
                        {
                            Usuario usuario = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu).List().FirstOrDefault();
                            Usuario res = null;
                            if (usuario != null && usuario.TipoAutenticacion.CCodigo == "1")
                            {
                                string passActualMD5 = MD5Password.GetMd5Hash(passActual);
                                res = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu).Where(u => u.DPassword == passActualMD5).SingleOrDefault();
                                if (res != null)
                                {
                                    res.DPassword = MD5Password.GetMd5Hash(passNueva);
                                    session.Save(res);
                                    session.Flush();
                                    mires.Msj = "La contraseña se cambio correctamente, ingrese con la nueva contraseña";
                                    mires.Valido = true;
                                }
                                else
                                {
                                    mires.Msj = "La contraseña actual especificada es incorrecta";
                                }
                            }
                            else if (usuario != null && usuario.TipoAutenticacion.CCodigo == "3")
                            {
                                string ldapServer = ConfigurationManager.AppSettings["LDAPSERVER"];
                                string targetOU = ConfigurationManager.AppSettings["TARGET_OU"];

                                DirectoryEntry userEntry = null;
                                //el cambio debe ser con un usuario administrador
                                DirectoryEntry uEntry = new DirectoryEntry("LDAP://" + ldapServer + "/" + targetOU, "portal", "tgpadmin", AuthenticationTypes.None);

                                DirectorySearcher searcher = new DirectorySearcher(uEntry);
                                searcher.Filter = String.Format("sAMAccountName={0}", usu);
                                searcher.SearchScope = SearchScope.Subtree;
                                searcher.CacheResults = false;

                                SearchResult searchResult = searcher.FindOne();
                                if (searchResult == null)
                                {
                                    mires.Msj = "El usuario/contraseña no coinciden.";
                                    mires.Valido = false;
                                    return mires;
                                }
                                userEntry = searchResult.GetDirectoryEntry();
                                try
                                {
                                    userEntry.Invoke("ChangePassword", new object[] { passActual, passNueva });
                                    userEntry.CommitChanges();
                                }
                                catch (TargetInvocationException t)
                                {
                                    mires.Msj = "La contraseña no ha podido ser modificada. Las causas pueden ser: </br></br> <ul>- Contraseña actual incorrecta.</ul> <ul>- La nueva contraseña no cumple con la politica de seguridad.</ul>";
                                    mires.Valido = false;
                                    return (mires);
                                }
                                mires.Msj = "La contraseña se ha modificado correctamente.";
                                mires.Valido = true;
                                return (mires);
                            }

                            return (mires);
                        }
                        catch (Exception ex)
                        {
                            mires.Msj = "Error inesperado comuniquese con el administrador";
                            mires.Exception = ex.Message;
                            return (mires);
                        }
                    }
                    else
                    {
                        mires.Msj = "La contraseña nueva especificada tiene menos de 8 o mas de 20 caracteres";
                        return (mires);
                    }
                }
                else
                {
                    mires.Valido = false;
                    mires.Msj = "Error inesperado comuniquese con el administrador. Token invalido.";
                    mires.Exception = "El token recibido por parametro en el WS es incorrecto.";
                    return mires;
                }
            }
            catch (Exception e)
            {
                mires.Msj = "Error inesperado comuniquese con el administrador.";
                mires.Exception = e.InnerException != null ? e.InnerException.Message : e.Message;
                return mires;
            }

        }

        /// <summary>
        /// Metodo que reseta la password de un usuario
        /// </summary>
        /// <param name="usu"></param>
        /// <returns></returns>
        public ResultadoResetPassword ResetPassword(string usu, string token)
        {
            ResultadoResetPassword resultado = new ResultadoResetPassword();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);


                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    Usuario user = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu).SingleOrDefault();

                    if (user == null)
                    {
                        resultado.Valido = false;
                        resultado.Msj = "Usuario inexistente";
                        resultado.Exception = "Usuario inexistente";
                    }
                    else if (user.MActivo == false)
                    {
                        resultado.Valido = false;
                        resultado.Msj = "Su usuario no se encuentra Activo";
                        resultado.Exception = "Su usuario no se encuentra Activo";
                    }
                    else if (user.DMail == null)
                    {
                        resultado.Valido = false;
                        resultado.Msj = "Su usuario no tiene una cuenta de e-mail cargada.";
                        resultado.Exception = "Su usuario no tiene una cuenta de e-mail cargada.";
                    }
                    else if ((user.TipoAutenticacion.CCodigo == TipoAutenticacion.Interna) || (user.TipoAutenticacion.CCodigo == TipoAutenticacion.ActiveDirectory))
                    {
                        user.DHashResetClave = GenerarClaveAleatoria(user);
                        user.MBloqueado = false;
                        user.NCantIntentos = 0;
                        session.SaveOrUpdate(user);
                        session.Flush();
                        try
                        {
                            EnviarMailCambioContraseña(user);
                            resultado.Valido = true;
                            resultado.Msj = "OK";
                        }
                        catch (Exception eMail)
                        {
                            resultado.Valido = false;
                            resultado.Msj = "Fallo el envio de correo";
                            resultado.Exception = "Fallo el envio de correo: " + eMail.Message;
                        }
                    }
                    else
                    {
                        resultado.Valido = false;
                        resultado.Msj = "Su usuario no posee el tipo de autenticacion correcta";
                        resultado.Exception = "Su usuario no posee el tipo de autenticacion correcta";
                    }
                    return resultado;
                }
                else
                {
                    resultado.Valido = false;
                    resultado.Msj = "Error inesperado comuniquese con el administrador. Token invalido.";
                    resultado.Exception = "El token recibido por parametro en el WS es incorrecto.";
                    return resultado;
                }
            }
            catch (Exception e)
            {
                resultado.Valido = false;
                resultado.Msj = "Error inesperado comuniquese con el administrador.";
                resultado.Exception = e.InnerException != null ? e.InnerException.Message : e.Message;
                return resultado;
            }


        }

        /// <summary>
        /// Metodo que genera una clave aleatoria
        /// </summary>
        /// <returns></returns>
        private static string GenerarClaveAleatoria(Usuario user)
        {
            //genero el hash reset y lo guardo en la tabla usuario
            GeneradorPassword code = new GeneradorPassword();
            code.LongitudPassword = 16;
            code.PorcentajeSimbolos = 0;
            string clave = string.Empty;
            //Mientras la clave aleatoria no este ya asignada a un usuario
            //se regenera
            do
            {
                clave = code.GetNewPassword().ToString();
            } while (SiClaveAleatoriaExiste(clave, user) == true);
            //retorno la clave
            return clave;
        }

        /// <summary>
        /// Metodo que verifica si la clave aleatoria ya existe
        /// </summary>
        /// <param name="clave"></param>
        /// <returns></returns>
        private static bool SiClaveAleatoriaExiste(string clave, Usuario user)
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            //string claveMD5 = MD5Password.GetMd5Hash(clave);
            //var usuariosRequest = session.QueryOver<Usuario>().Where(u => u.DUsuario == user.DUsuario && u.DPassword == claveMD5).List<Usuario>().ToList();
            var usuariosRequest = session.QueryOver<Usuario>().Where(u => u.DUsuario == user.DUsuario && u.DHashResetClave == clave).List<Usuario>().ToList();
            if (usuariosRequest.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Envio de contraseña de usuario
        /// </summary>
        /// <param name="usu">Nombre de usuario</param>
        /// <returns></returns>
        public ResultadoSendPasswordFT SendPasswordFT(string usu, string token)
        {
            ResultadoSendPasswordFT mires = new ResultadoSendPasswordFT();
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        GeneradorPassword pass = new GeneradorPassword();
                        pass.LongitudPassword = 8;
                        pass.PorcentajeSimbolos = 0;
                        String passNueva = pass.GetNewPassword();

                        var res = session.QueryOver<Usuario>().Where(u => u.DUsuario == usu).SingleOrDefault();
                        res.DPassword = MD5Password.GetMd5Hash(passNueva);
                        session.Save(res);
                        session.Flush();
                        //db.SaveChanges();

                        var desc = session.QueryOver<Acreedor>().Where(u => u.CId == res.CId).SingleOrDefault();

                        MailMessage message = new MailMessage();
                        message.To.Add(res.DMail);
                        message.Bcc.Add("info@tesoreria.gba.gov.ar");
                        message.Subject = "Activación de Usuario - TGP Consulta de Pagos";
                        message.From = new MailAddress("info@tesoreria.gba.gov.ar");
                        message.IsBodyHtml = true;
                        message.Body = "<b>TESORERÍA GENERAL DE LA PROVINCIA DE BUENOS AIRES </b>" +
                        "<br><br><p>Le da la bienvenida al Portal de Servicios Web, a través del módulo de 'Consulta de Pagos'.</p>" +
                        "<p>Para aceder utilice el enlace identificado como Portal de Servicios en la página de inicio del sitio oficial o a través de la siguiente URL http://www.tesoreria.gba.gov.ar/portal </p>" +
                        "<br><p>Se ha generado el acceso para <b>" + desc.DRazonSocial + "</b> con las siguientes credenciales</p>" +
                        "<p>USUARIO -> <b>" + res.DUsuario + "</b></p>" +
                        "<p>CONTRASEÑA -><b> " + passNueva + " </b></p>" +
                        "<br><p>NOTA: Recuerde cambiar la contraseña luego del primer inicio de sesión a través de la opción disponible en el Perfil de usuario.</p>";

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("mailserver");
                        smtp.Send(message);
                        mires.Msj = "El correo electrónico se envio correctamente";
                        mires.Valido = true;
                        return (mires);
                    }
                    catch (Exception ex)
                    {
                        mires.Msj = "Error inesperado comuniquese con el administrador";
                        mires.Exception = ex.Message;
                        mires.Valido = false;
                        return (mires);
                    }
                }
                else
                {
                    mires.Valido = false;
                    mires.Msj = "Error inesperado comuniquese con el administrador. Token invalido.";
                    mires.Exception = "El token recibido por parametro en el WS es incorrecto.";
                    return mires;
                }
            }
            catch (Exception e)
            {
                mires.Valido = false;
                mires.Msj = "Error inesperado comuniquese con el administrador.";
                mires.Exception = e.InnerException != null ? e.InnerException.Message : e.Message;
                return mires;
            }

        }

        /// <summary>
        /// Metodo de inserción de aditoria
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="fecha"></param>
        /// <param name="codEstrutura"></param>
        /// <param name="codActividad"></param>
        /// <param name="observaciones"></param>
        public void addAuditoria(string usuario, DateTime fecha, string codEstrutura, string codActividad, string observaciones, string token)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    AuditoriaUsuario auditoria = new AuditoriaUsuario();
                    auditoria.EstructuraFuncional = codEstrutura;
                    auditoria.Accion = fecha;
                    auditoria.Usuario = usuario;
                    auditoria.Actividad = codActividad;
                    auditoria.Observacion = observaciones;
                    session.SaveOrUpdate(auditoria);
                    session.Flush();
                }
                else
                {
                    throw new Exception("Token invalido.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error inesperado comuniquese con el administrador. " + e.Message);
            }
        }

        /// <summary>
        /// Metodo que registra la conexion de usuario que ingreso a una aplicacion de TGP
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="aplicacion"></param>
        /// <param name="fechaRegistro"></param>
        public void RegistarAuditoriaConexion(byte[] avatar, string nombreUsuario, string aplicacion, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion, string token)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        //Enviar mensaje al monitor
                        //EnviarMensajeConexion(avatar, nombreUsuario, aplicacion, usuarioID, IPCliente, browserCliente, serverDescripcion);
                        //Registro en la auditoria
                        RegistrarAuditoriaConexionUsuario(nombreUsuario, aplicacion, usuarioID, IPCliente, browserCliente, serverDescripcion);
                        // return "Operacion realizada con exito.";
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    throw new Exception("Token invalido.");
                }
            }
            catch (Exception e)
            {
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                throw new Exception("Error inesperado comuniquese con el administrador. " + error);
            }

        }

        /// <summary>
        /// Metodo que registra cuando un usuario se desconecta de la aplicacion de TGP
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="aplicacion"></param>
        /// <param name="fechaRegistro"></param>
        public void RegistarAuditoriaDesconexion(byte[] avatar, string nombreUsuario, string aplicacion, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion, string token)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        //Enviar mensaje al monitor
                        EnviarMensajeDesconexion(avatar, nombreUsuario, aplicacion, usuarioID, IPCliente, browserCliente, serverDescripcion);
                        //Registro en la auditoria
                        RegistrarAuditoriaConexionUsuario(nombreUsuario, aplicacion, usuarioID, IPCliente, browserCliente, serverDescripcion);
                        // return "Operacion realizada con exito.";
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    throw new Exception("Token invalido.");
                }
            }
            catch (Exception e)
            {
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                throw new Exception("Error inesperado comuniquese con el administrador. " + error);
            }

        }

        /// <summary>
        /// Metodo que recupera los nodos funcionales y los usuarios que contiene el mismo.
        /// Este metodo tiene como funcionalidad que mediante la Estructura Funcional recuperar
        /// los nodos (que funcionan como Area) y los usuarios que estan relacionados a ella.
        /// </summary>
        /// <param name="ObtenerSeguridadUsuarioDTO"></param>
        /// <returns name="RespuestaSeguridadUsuarioDTO"></returns>
        public Models.DTO.ResultadoObtenerNodosFuncionalesYUsuariosDTO ObtenerNodosFuncionalesYUsuarios(Models.DTO.ObtenerNodosFuncionalesYUsuariosDTO request, string token)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    try
                    {
                        Models.DTO.ResultadoObtenerNodosFuncionalesYUsuariosDTO resultado = new Models.DTO.ResultadoObtenerNodosFuncionalesYUsuariosDTO();
                        return null;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    throw new Exception("Token invalido.");
                }
            }
            catch (Exception e)
            {
                string error = e.InnerException != null ? e.InnerException.Message : e.Message;
                throw new Exception("Error inesperado comuniquese con el administrador. " + error);
            }

        }

        /// <summary>
        /// Metodo que recupera todos los datos de auditoria para un conjunto de datos que vienen en SolicitudGetAuditoria
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Models.DTO.ResultadoGetAuditoriaDTO GetAuditoria(Models.DTO.SolicitudGetAuditoria request, string token)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    Conjunction condiciones = Restrictions.Conjunction();
                    if (request.Actividad != null)
                    {
                        condiciones.Add(Restrictions.Where<AuditoriaUsuario>(e => e.Actividad == request.Actividad));
                    }
                    if (request.EstructuraFuncional != null)
                    {
                        condiciones.Add(Restrictions.Where<AuditoriaUsuario>(e => e.EstructuraFuncional == request.EstructuraFuncional));
                    }
                    if (request.Usuario != null)
                    {
                        condiciones.Add(Restrictions.Where<AuditoriaUsuario>(e => e.Usuario == request.Usuario));
                    }
                    if (request.Observacion != null)
                    {
                        condiciones.Add(Restrictions.Where<AuditoriaUsuario>(e => e.Observacion == request.Observacion));
                    }
                    if (request.FechaDesde != null)
                    {
                        condiciones.Add(Restrictions.Where<AuditoriaUsuario>(e => e.Accion >= request.FechaDesde));
                    }
                    if (request.FechaHasta != null)
                    {
                        condiciones.Add(Restrictions.Where<AuditoriaUsuario>(e => e.Accion <= request.FechaHasta));
                    }

                    List<AuditoriaUsuario> auditoria = (List<AuditoriaUsuario>)session.QueryOver<AuditoriaUsuario>().Where(condiciones).List<AuditoriaUsuario>().ToList<AuditoriaUsuario>();

                    return ConstruirResultadoAuditoriaDTO(auditoria);
                }
                else
                {
                    ResultadoGetAuditoriaDTO auditoria = new ResultadoGetAuditoriaDTO();
                    auditoria.Codigo = "";
                    auditoria.Descripcion = "Error inesperado comuniquese con el administrador. Token invalido.";
                    return auditoria;
                }
            }
            catch (Exception e)
            {
                ResultadoGetAuditoriaDTO auditoria = new ResultadoGetAuditoriaDTO();
                auditoria.Codigo = "";
                auditoria.Descripcion = "Error inesperado comuniquese con el administrador. " + e.Message;
                return auditoria;
            }


        }

        /// <summary>
        /// Metodo privado que construye la respuesta de la consulta de auditoria.
        /// </summary>
        /// <param name="auditoria"></param>
        /// <returns></returns>
        private ResultadoGetAuditoriaDTO ConstruirResultadoAuditoriaDTO(List<AuditoriaUsuario> auditoria)
        {
            ResultadoGetAuditoriaDTO resultadoDTO = new ResultadoGetAuditoriaDTO();
            resultadoDTO.Items = new List<AuditoriaDTO>();
            foreach (var item in auditoria)
            {
                AuditoriaDTO aud = new AuditoriaDTO();
                aud.Actividad = item.Actividad;
                aud.EstructuraFuncional = item.EstructuraFuncional;
                aud.FechaAccion = item.Accion;
                aud.Observacion = item.Observacion;
                aud.Usuario = item.Usuario;
                resultadoDTO.Items.Add(aud);
            }
            return resultadoDTO;
        }


        public IList<UsuarioSTGP> UsuariosXEstructura(string codEstructura, string token)
        {
            try
            {
                ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);

                string valorToken = BuscarTokenEnBD(session);

                if (token != null && token != "" && Desencriptar(token) == valorToken)
                {
                    List<UsuarioSTGP> usuariosSTGP = new List<UsuarioSTGP>();
                    EstructuraFuncional estructura = session.QueryOver<EstructuraFuncional>().Where(e => e.CCodigo == codEstructura).List().FirstOrDefault();
                    if (estructura != null)
                    {
                        List<Rol> roles = session.QueryOver<Rol>().Where(r => r.EstructuraFuncional.CId == estructura.CId).List<Rol>().ToList<Rol>();
                        ICriteria cUsers = session.CreateCriteria<RolNodoUsuario>();
                        cUsers.CreateAlias("Rol", "Rol");
                        cUsers.Add(Expression.In("Rol.CId", roles.Select(r => r.CId).ToArray()));
                        List<Usuario> usuarios = cUsers.List<RolNodoUsuario>().Select(rnu => rnu.Usuario).Distinct<Usuario>().ToList();
                        foreach (Usuario usu in usuarios)
                        {
                            UsuarioSTGP usuTgp = new UsuarioSTGP();
                            usuTgp.Id = usu.CId;
                            usuTgp.Mail = usu.DMail;
                            usuTgp.Username = usu.DUsuario;
                            usuariosSTGP.Add(usuTgp);
                        }
                    }
                    return usuariosSTGP;
                }
                else
                {
                    throw new Exception("Token invalido.");
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error inesperado comuniquese con el administrador. " + e.Message);
            }

        }


        #endregion

        #region // Metodos Privados //
        

        /// <summary>
        /// Metodo que desencripta 
        /// </summary>
        private string Desencriptar(string token)
        {
            try
            {
                return Utils.Encriptador.Desencriptar(token);
            }
            catch (Exception e)
            {
                throw new Exception("Error al desencriptar token recibo por parametro. " + e.Message);
            }
        }

        /// <summary>
        /// Metodo que validas las credenciales enviadad
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="claveServicio"></param>
        /// <returns></returns>
        private bool SiCredencialesValidas(string nombreUsuario, string claveServicio)
        {
            var usuarioService = new Services.UsuarioService();
            var usuario = usuarioService.ObtenerPorNombreUsuario(nombreUsuario);
            if (usuario != null)
            {
                if (claveServicio == ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString())
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// Metodo que valida los parametrios de entrada del metodo Obtener Usuario
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        private bool SiParametroValidoObtenerUsuario(RequerimientoObtenerUsuarioDTO requerimiento)
        {
            if ((string.IsNullOrWhiteSpace(requerimiento.NombreUsuario) == false) && (string.IsNullOrWhiteSpace(requerimiento.ClaveServicio) == false) && (string.IsNullOrWhiteSpace(requerimiento.UsuarioConsultaID.ToString()) == false))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Metodo que valida los parametros del metodo ObtenerUsuariosNominadosPorRol
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        private bool SiParametroValidoObtenerUsuariosNominadosPorRol(RequerimientoObtenerUsuariosNominadosPorRolDTO requerimiento)
        {
            if ((string.IsNullOrWhiteSpace(requerimiento.NombreUsuario) == false) && (string.IsNullOrWhiteSpace(requerimiento.ClaveServicio) == false) && (string.IsNullOrWhiteSpace(requerimiento.CodigoRol) == false))
                return true;
            else
                return false;
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


        /// <summary>
        /// Metodo que envia Email con un codigo hash para el cambio de la contraseña de un usuario
        /// </summary>
        /// <param name="user"></param>
        private void EnviarMailCambioContraseña(Usuario user)
        {
            //genero el mail con la url para la la pantalla de la carga de la nueva contraseña
            MailMessage message = new MailMessage();
            message.To.Add(user.DMail);
            message.Subject = "Solicitud de Cambio de Contraseña - PORTAL TGP";
            message.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_TO"]);

            message.IsBodyHtml = true;
            string body = "<div style='background-color: white; padding-bottom:15px; padding-top:15px;'>" +
                "<article style='margin: 0px 50px;'><p style='font-size:24px; font-weight:normal; color:#4a4949'>" +
                "<span style='font-size:18px; font-weight:normal; font-style:italic; color:#4a4949'>Solicitud de Cambio de Contraseña" +
                "</span> <br>Portal <span style='color:#00B1C7'>SIGAF-PBA Web</span></p><p style='color: #595959;'></br>" +
                "Se ha solicitado a través del PORTAL SIGAF-PBA de la Tesoreria General de la Provincia de Bs. As. el cambio de contraseña para el usuario: <b>" + user.DUsuario + "</b>.</p></br>" +
                "<p style='color: #595959;'>Para cambiar la contraseña utilice el siguiente enlace: <a style='font-size:15px' href='" + ConfigurationManager.AppSettings["PortalURL"] + "Seguridad/ChangePassExternal?h=" + user.DHashResetClave + "'>Modificar Contraseña</a><a>.</a></p><a>" +
                "<p style='color: #595959;'>Si usted no ha solicitado esta acción desestime este mail.</p></br><p>" +
                "<small><i><b>**Esta dirección de correo electrónico no admite respuestas. Para obtener más información consultar al depto-usuarios@tesoreria.gba.gov.ar</b></i></small>" +
                "</p></a></article></div>";
            string head = "<head>" +
                                    "<meta charset='utf-8' />" +
                                    "<meta http-equiv='X-UA-Compatible' content='IE=Edge'>" +
                                    "<meta name='description' content='PFI'>  " +
                                    "<link rel='shortcut icon' href='~/Content/img/icoprov.ico' /> " +
                                    "<link href='https://fonts.googleapis.com/css?family=Roboto:100,300,400,500,700,900' rel='stylesheet'> " +
                                    "<style type='text/css'>article{font-family: Arial}</style>" +
                                "</head><body style='padding: 0px; margin: auto auto; font-family: Arial; color: #595959;width: 720px;'>";
            string footer = "<div style='min-height: 1px;float:left; margin-top:15px'>" + "<img alt='firma mail' src='" + ConfigurationManager.AppSettings["imagenes"] + "pie-mail-tesoreria-2020.png" + "'style='padding-left: 1px;padding-right: 50px;'></div></body>";
            string html = head + body + footer;
            message.Body = html;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("mailserver");
            smtp.Credentials = new System.Net.NetworkCredential("portal", "tgpadmin");
            smtp.Send(message);
        }


        private static AlternateView getEmbeddedImage(String filePath, string body)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = body + @"<img src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }


        /// <summary>
        /// Metodo que envia un mensaje al HUB de Conexion de usuario
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="aplicacion"></param>
        /// <param name="usuarioID"></param>
        /// <param name="IPCliente"></param>
        /// <param name="browserCliente"></param>
        /// <param name="serverDescripcion"></param>
        private static void EnviarMensajeConexion(byte[] avatar, string nombreUsuario, string aplicacion, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //Creo la coneccion.
                var hubConnection = new Microsoft.AspNet.SignalR.Client.HubConnection(ConfigurationManager.AppSettings["URL_SIGNAL_HUB_SERVER01"] + "signalr", useDefaultUrl: false);

                //Creo el proxy con el nombre de la clase
                var hubProxy = hubConnection.CreateHubProxy("AuditoriaUsuarioHub");

                //Credenciales
                hubConnection.Credentials = CredentialCache.DefaultNetworkCredentials;

                //Inicia la coneccion.
                hubConnection.Start().Wait();

                //Envio el mensaje.
                hubProxy.Invoke("RecibeDataConnect", avatar, nombreUsuario, aplicacion, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), usuarioID, IPCliente, browserCliente, serverDescripcion);


                //Cierro la coneccion.
                hubConnection.Stop();

                #region Proceso de balanceo de comunicacion al monitor
                ////Si esta banalceado envio el mensaje al servidor 2
                //if (ConfigurationManager.AppSettings["SIGNAL_HUB_SI_BALANCEADO"] == "true")
                //{


                //    var hubConnection2 = new HubConnection(ConfigurationManager.AppSettings["URL_SIGNAL_HUB_SERVER02"] + "signalr", useDefaultUrl: false);

                //    //Creo el proxy con el nombre de la clase
                //    var hubProxy2 = hubConnection2.CreateHubProxy("AuditoriaUsuarioHub");

                //    //Credenciales
                //    hubConnection2.Credentials = CredentialCache.DefaultNetworkCredentials;

                //    //Inicia la coneccion.
                //    hubConnection2.Start().Wait();

                //    //Envio el mensaje.
                //    hubProxy2.Invoke("RecibeDataConnect", avatar, nombreUsuario, aplicacion, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), usuarioID, IPCliente, browserCliente, serverDescripcion);

                //    //Cierro la coneccion.
                //    hubConnection2.Stop();
                //}
                #endregion

            }
            catch (Exception eSignal)
            {
                throw new Exception(eSignal.Message);
            }


        }

        /// <summary>
        /// Metodo que envia un mensaje al HUB de Desconexion de usuario
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="aplicacion"></param>
        /// <param name="usuarioID"></param>
        /// <param name="IPCliente"></param>
        /// <param name="browserCliente"></param>
        /// <param name="serverDescripcion"></param>
        private static void EnviarMensajeDesconexion(byte[] avatar, string nombreUsuario, string aplicacion, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //Creo la coneccion.
                var hubConnection = new HubConnection(ConfigurationManager.AppSettings["URL_SIGNAL_HUB_SERVER01"] + "signalr", useDefaultUrl: false);

                //Creo el proxy con el nombre de la clase
                var hubProxy = hubConnection.CreateHubProxy("AuditoriaUsuarioHub");

                //Credenciales
                hubConnection.Credentials = CredentialCache.DefaultNetworkCredentials;

                //Inicia la coneccion.
                hubConnection.Start().Wait();

                //Envio el mensaje.
                hubProxy.Invoke("RecibeDataDisconnect", avatar, nombreUsuario, aplicacion, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), usuarioID, IPCliente, browserCliente, serverDescripcion);

                //Cierro la coneccion.
                hubConnection.Stop();

            }
            catch (Exception eSignal)
            {
                throw new Exception(eSignal.Message);
            }
        }

        /// <summary>
        /// Metodo que loguea cuando un usuario de conecta
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="estructuraFuncional"></param>
        /// <param name="usuarioID"></param>
        /// <param name="IPCliente"></param>
        /// <param name="browserCliente"></param>
        /// <param name="serverDescripcion"></param>
        private void RegistrarAuditoriaConexionUsuario(string nombreUsuario, string estructuraFuncional, long usuarioID, string IPCliente, string browserCliente, string serverDescripcion)
        {
            //conexion nhibernate
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]);
            //guardar datos de auditoria
            AuditoriaConexion auditoria = new AuditoriaConexion();
            auditoria.FConexion = DateTime.Now;
            var usuario = session.QueryOver<Usuario>().Where(a => a.DUsuario == nombreUsuario).SingleOrDefault();
            auditoria.Usuario = usuario;
            auditoria.IPConexion = IPCliente;
            auditoria.Browser = browserCliente;
            auditoria.Server = serverDescripcion;
            auditoria.EstructuraFuncionalCodigo = estructuraFuncional;
            session.SaveOrUpdate(auditoria);
            session.Flush();
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

        private string BuscarTokenEnBD(ISession session)
        {
            try
            {
                //Se filtra por nombre para obtener los token, despues va a buscar el ultimo generado. (el ultimo c_id cargado).
                return session.QueryOver<SolicitudToken>().Where(z => z.Nombre == ConfigurationManager.AppSettings["NOMBRE_TOKEN"]).OrderBy(x => x.CId).Desc.List<SolicitudToken>().FirstOrDefault().Valor;
            }
            catch (Exception e)
            {
                string error = "Error al obtener token de BD (Metodo:BuscarTokenEnBD(ISession session) ). Detalle: ";
                error += e.InnerException != null ? e.InnerException.Message : e.Message;
                throw new Exception(error);
            }
        }

        #endregion
    }
}
