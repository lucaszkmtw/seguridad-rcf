using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NHibernate;
using Models;
using System.Configuration;
using Microsoft.AspNet.SignalR.Client;
using System.Reflection;
using NHibernate.Criterion;
using TGP.WSS.Models.Resultado;
using TGP.WSS.Models.Requerimiento;
using TGP.WSS.Services;
using System.Transactions;


namespace TGP.WSS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GenerarUsuarioService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GenerarUsuarioService.svc or GenerarUsuarioService.svc.cs at the Solution Explorer and start debugging.
    public class GenerarUsuarioService : IGenerarUsuarioService
    {
        #region // Metodos Publicos //
        /// <summary>
        /// Metodo utilizado para probar el servicio
        /// </summary>
        /// <returns>Nro de version del Servicio</returns>
        public ResultadoDTO Eco()
        {
            ResultadoDTO resultado = new ResultadoDTO();
            try
            {
                resultado.Codigo = "1";
                resultado.Descripcion = System.Configuration.ConfigurationManager.AppSettings["version"].ToString();
                return resultado;
            }
            catch (Exception eEco)
            {
                resultado.Codigo = "999";
                resultado.Descripcion = "Error general, más detalle: " + eEco.Message;
                return resultado;
            }
        }

        /// <summary>
        /// Metodo que permite la generacion de un Usuario Nominado desde una aplicacion externa a la aplicacion
        /// de seguridad
        /// NOTA: Se esta invocando desde la aplicacion de "Consulta de Expediente" cuando se aprueba la solicitud de usuario
        /// y tambien desde "RUCO".
        /// </summary>
        public ResultadoGenerarUsuarioNominadoUsuarioDTO GenerarNominado(RequerimientoGenerarUsuarioNominadoDTO requerimiento)
        {
            //Instancio el resultado
            ResultadoGenerarUsuarioNominadoUsuarioDTO resultado = new ResultadoGenerarUsuarioNominadoUsuarioDTO();
            //Defino las instancias de los services a utilizar
            #region // Variables Locales //
            SolicitudService solicitudService = SolicitudService.GetInstance();
            TipoSolicitudService tipoSolicitudService = TipoSolicitudService.GetInstance();
            SolicitudNodoFuncionalService solicitudNodoService = SolicitudNodoFuncionalService.GetInstance();
            EstadoSolicitudService estadoSolicitudService = EstadoSolicitudService.GetInstance();
            UsuarioService usuarioService = UsuarioService.GetInstance();
            EstructuraFuncionalService estructuraFuncionalService = EstructuraFuncionalService.GetInstance();
            RolService rolService = RolService.GetInstance();
            RolNodoUsuarioService rolNodoUsu = RolNodoUsuarioService.GetInstance();
            TipoAutenticacionService tipoAutenticacionService = TipoAutenticacionService.GetInstance();
            TipoUsuarioService tipoUsuarioService = TipoUsuarioService.GetInstance();
            TerminosYCondicionesService termYCondService = TerminosYCondicionesService.GetInstance();
            #endregion

            ///Verifico que los parametros enviados sean validos
            if (SiParametroValido(requerimiento) == true)
            {
                if (SiUsuarioExiste(requerimiento) == false)
                {
                    //Instancio un transaccion
                    TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 10, 0));
                    try
                    {
                        using (transaction)
                        {
                            Nominado usuarioNominado = RegistrarUsuarioNominadoDesdeGenerarNominado(requerimiento, solicitudNodoService, usuarioService, rolService, rolNodoUsu, tipoAutenticacionService, tipoUsuarioService, termYCondService);

                            var claveGenerada = RegistrarUsuarioDesdeGenerarNominado(usuarioService, usuarioNominado);
                            transaction.Complete();
                            transaction.Dispose();
                            //Como todo salio bien respondo el servicio con OK
                            resultado.Codigo = "1";
                            resultado.Descripcion = "OK";
                            resultado.ClaveGenerada = claveGenerada;
                            resultado.Email = requerimiento.UsuarioNominadoDTO.Email;
                            resultado.NombreUsuario = requerimiento.UsuarioNominadoDTO.NombreUsuario;
                        }


                    }
                    catch (Exception eGeneral)
                    {
                        transaction.Dispose();
                        resultado.Codigo = "999";
                        resultado.Descripcion = "Error General del Servicio, más detalle:  " + eGeneral.Message;
                    }
                }
                else
                {
                    resultado.Codigo = "100";
                    resultado.Descripcion = "Atención: Ya existe un usuario con ese Nombre de Usuario ingresado: " + requerimiento.UsuarioNominadoDTO.NombreUsuario + ". Comuníquese con el Depto. Usuarios 4294400 int.84633. ";
                }
               
            }
            else
            {
                resultado.Codigo = "101";
                resultado.Descripcion = "Parámetros NO válidos. Verifique haber ingresado todos los parámetros solicitados.";
            }

            
            return resultado;
        }

        #endregion

        #region // Metodos Privados de Generar Usuario Nominado //
        /// <summary>
        /// Metodo que verifica la existencia de un usuario para el Nombre de usuario enviado como parametro
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        private bool SiUsuarioExiste(RequerimientoGenerarUsuarioNominadoDTO requerimiento)
        {

            Usuario usuario = UsuarioService.GetInstance().ObtenerPorNombreUsuario(requerimiento.UsuarioNominadoDTO.NombreUsuario);
            if (usuario != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Metodo que registra un Usuario
        /// </summary>
        /// <param name="usuarioService"></param>
        /// <param name="usuarioNominado"></param>
        /// <returns></returns>
        private static string RegistrarUsuarioDesdeGenerarNominado(UsuarioService usuarioService, Nominado usuarioNominado)
        {
            Usuario usuario = usuarioNominado;
            var usu = usuario.DUsuario;
            var passReturn = usuario.DPassword;
            usuario.DPassword = MD5Password.GetMd5Hash(usuario.DPassword);
            usuarioService.UpdateWithOutFlush(usuario);
            return passReturn;
        }

        /// <summary>
        /// Metodo que registra un usuario Nominado
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <param name="solicitudNodoService"></param>
        /// <param name="usuarioService"></param>
        /// <param name="rolService"></param>
        /// <param name="rolNodoUsu"></param>
        /// <param name="tipoAutenticacionService"></param>
        /// <param name="tipoUsuarioService"></param>
        /// <param name="termYCondService"></param>
        /// <returns></returns>
        private static Nominado RegistrarUsuarioNominadoDesdeGenerarNominado(RequerimientoGenerarUsuarioNominadoDTO requerimiento, SolicitudNodoFuncionalService solicitudNodoService, UsuarioService usuarioService, RolService rolService, RolNodoUsuarioService rolNodoUsu, TipoAutenticacionService tipoAutenticacionService, TipoUsuarioService tipoUsuarioService, TerminosYCondicionesService termYCondService)
        {
            //Usuario que invoca el servicio para dar de alta un usuario
            Usuario usuarioLogin = usuarioService.ObtenerPorNombreUsuario(requerimiento.NombreUsuario);
            //Se obtiene el tipo de autenticacion interna
            TipoAutenticacion interna = tipoAutenticacionService.ObtenerPorCodigo(TGP.WSS.TipoAutenticacion.Interna);
            //Se obtiene el tipo de usuario Nominado
            TipoUsuario nominado = tipoUsuarioService.ObtenerPorCodigo(TipoUsuario.NOMINADO);
            //Instancio el Objeto Nominado y se le asigna los datos provenientes del requerimiento
            Nominado usuarioNominado = new Nominado();
            usuarioNominado.NDni = long.Parse(requerimiento.UsuarioNominadoDTO.NumeroDocumento.Substring(2, 8));
            usuarioNominado.DApellido = requerimiento.UsuarioNominadoDTO.Apellidos;
            usuarioNominado.DNombre = requerimiento.UsuarioNominadoDTO.Nombres;
            usuarioNominado.DUsuario = requerimiento.UsuarioNominadoDTO.NombreUsuario;
            //Se genera una clase aleatoria
            GeneradorPassword pass = new GeneradorPassword();
            pass.LongitudPassword = 8;
            pass.PorcentajeSimbolos = 0;
            string claveGenerada = pass.GetNewPassword();
            usuarioNominado.DPassword = claveGenerada;
            usuarioNominado.TipoAutenticacion = interna;
            usuarioNominado.TipoUsuario = nominado;
            usuarioNominado.MActivo = true;
            usuarioNominado.MBloqueado = false;
            usuarioNominado.DMail = requerimiento.UsuarioNominadoDTO.Email;
            usuarioNominado.NCantIntentos = 0;
            usuarioNominado.FAlta = DateUtilsService.ObtenerFechaActual();
            usuarioNominado.FUltimaOperacion = DateUtilsService.ObtenerFechaActual();
            usuarioNominado.UsuarioLogin = usuarioLogin;
            //Se obtiene los terminos y condiciones 
            List<TerminosCondiciones> terminoYCondiciones = termYCondService.ObtenerPorCodigo(requerimiento.CodigoEstructuraFuncional);
            if (terminoYCondiciones.Count() > 0)
            {
                usuarioNominado.TerminosYCondiciones = terminoYCondiciones.First();
                usuarioNominado.FAceptacionTerminos = DateUtilsService.ObtenerFechaActual();
            }
            //Registro el Usuario
            usuarioService.SaveWithOutFlush(usuarioNominado);
            //Registro la relacion entre Rol Nodo Usuario
            RegistrarRelacionRolNodoUsuarioDesdeGenerarNominado(requerimiento, solicitudNodoService, rolService, rolNodoUsu, usuarioLogin, usuarioNominado);
            //Retorno el usuarioNominado
            return usuarioNominado;
        }

        /// <summary>
        /// Metodo que registra la relacion entre Rol Nodo y Usuario
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <param name="solicitudNodoService"></param>
        /// <param name="rolService"></param>
        /// <param name="rolNodoUsu"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuarioNominado"></param>
        private static void RegistrarRelacionRolNodoUsuarioDesdeGenerarNominado(RequerimientoGenerarUsuarioNominadoDTO requerimiento, SolicitudNodoFuncionalService solicitudNodoService, RolService rolService, RolNodoUsuarioService rolNodoUsu, Usuario usuarioLogin, Nominado usuarioNominado)
        {
            //Obtener los nodos pertenecientes a la solicitud
            List<NodoFuncional> nodosFuncionales = solicitudNodoService.ObtenerNodosPorSolicitud(requerimiento.SolicitudID).Select(n => n.NodoFuncional).ToList();

            Rol rol = rolService.ObtenerRolPorCodigoYEstructura(requerimiento.CodigoRol, requerimiento.CodigoEstructuraFuncional);
            foreach (NodoFuncional nodo in nodosFuncionales)
            {
                RolNodoUsuario rnu = new RolNodoUsuario();
                rnu.FUltimaOperacion = DateUtilsService.ObtenerFechaActual();
                rnu.NodoFuncional = nodo;
                rnu.Usuario = usuarioNominado;
                rnu.UsuarioLogin = usuarioLogin;
                rnu.Rol = rol;
                rolNodoUsu.SaveWithOutFlush(rnu);
            }
        }

        /// <summary>
        /// Metodo que valida los parametros de entrada del servicio
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        private bool SiParametroValido(RequerimientoGenerarUsuarioNominadoDTO requerimiento)
        {
            if ((string.IsNullOrWhiteSpace(requerimiento.NombreUsuario) == false) 
                && (requerimiento.SolicitudID > 0) 
                && (string.IsNullOrWhiteSpace(requerimiento.CodigoRol) == false) 
                && (string.IsNullOrWhiteSpace(requerimiento.CodigoEstructuraFuncional) == false) 
                && (requerimiento.UsuarioNominadoDTO != null) 
                &&(SiValidaUsuarioNominadoDTO(requerimiento.UsuarioNominadoDTO) == true))
                return true;
            else
                return false;
        }

        /// <summary>
        /// metodo que valida los datos del UsuarioNominadoDTO
        /// </summary>
        /// <param name="usuarioNominadoDTO"></param>
        /// <returns></returns>
        private bool SiValidaUsuarioNominadoDTO(Models.DTO.UsuarioNominadoDTO usuarioNominadoDTO)
        {
            if ((string.IsNullOrWhiteSpace(usuarioNominadoDTO.Apellidos) == false)
                && (string.IsNullOrWhiteSpace(usuarioNominadoDTO.Nombres) == false)
                && (string.IsNullOrWhiteSpace(usuarioNominadoDTO.NumeroDocumento) == false)
                && (string.IsNullOrWhiteSpace(usuarioNominadoDTO.NombreUsuario) == false)
                && (string.IsNullOrWhiteSpace(usuarioNominadoDTO.Email) == false))
                return true;
            else
                return false;
        }

        #endregion
    }
}
