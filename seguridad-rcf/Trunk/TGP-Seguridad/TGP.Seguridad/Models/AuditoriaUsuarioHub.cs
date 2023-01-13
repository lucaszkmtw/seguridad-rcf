using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Web;

namespace TGP.Seguridad.Models
{
    [HubName("auditoriaUsuarioHub")]
    public class AuditoriaUsuarioHub : Hub
    {
        /// <summary>
        /// Metodo que notifica cuando un usuario se conecto
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="estructuraFuncional"></param>
        /// <param name="fechaRegistro"></param>
        public void RecibeDataConnect(byte[] avatar, String nombreUsuario, string estructuraFuncional, string fechaRegistro, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion)
        {
            try
            {
                //Obtengo el contexto.
                var msjContex = GlobalHost.ConnectionManager.GetHubContext<AuditoriaUsuarioHub>();

                //Si el usuario no tiene imagen obtengo la de por defecto
                if (avatar == null)
                    avatar = ObtenerImagenPorDefecto();

                //Envio el mensajes a los clientes.
                msjContex.Clients.All.recibeWEBData(avatar, nombreUsuario, estructuraFuncional, fechaRegistro, usuarioID, IPCliente, browserCliente, serverDescripcion);
            }
            catch
            {

            }
        }



        /// <summary>
        /// Metodo que notifica cuando un usuario se desconecto
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="estructuraFuncional"></param>
        /// <param name="fechaRegistro"></param>
        public void RecibeDataDisconnect(byte[] avatar, String nombreUsuario, string estructuraFuncional, string fechaRegistro, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion)
        {
            try
            {
                //Obtengo el contexto.
                var msjContex = GlobalHost.ConnectionManager.GetHubContext<AuditoriaUsuarioHub>();

                //Si el usuario no tiene imagen obtengo la de por defecto
                if (avatar == null)
                    avatar = ObtenerImagenPorDefecto();

                //Evio el mensajes a los clientes.
                msjContex.Clients.All.recibeWEBDataDisconect(avatar, nombreUsuario, estructuraFuncional, fechaRegistro, usuarioID, IPCliente, browserCliente, serverDescripcion);
            }
            catch
            {

            }
        }

        public void ServerSignal()
        {
            
            //Inicio del servidor.
        }

        /// <summary>
        /// Metodo que obtiene la imagen por defecto
        /// </summary>
        /// <returns></returns>
        private Byte[] ObtenerImagenPorDefecto()
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Content/img/user.jpg"));
            return imgdata;
        }

    }
}