using NHibernate;
using NHibernate.Criterion;
using TGP.WSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGP.WSS.Models.DTO;

namespace TGP.WSS.Services
{
    /// <summary>
    /// Clase que cumple la funcion de Gestionar y recuperar Usuario
    /// </summary>
    public class UsuarioService : RepositoryCRUDService<Usuario>
    {
        public static UsuarioService instance = null;

        /// <summary>
        /// Metodo que recupera la instancia del objeto
        /// </summary>
        /// <returns></returns>
        public static UsuarioService GetInstance()
        {
            if (instance == null)
                instance = new UsuarioService();
            return instance;
        }

    
        /// <summary>
        /// Metodo que obtiene el usuario por nombre de Usuario
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <returns></returns>
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            ICriteria usuario = getSession().CreateCriteria<Usuario>();
            usuario.Add(Expression.Eq("DUsuario", nombreUsuario));
            return (Usuario)usuario.UniqueResult();
        }

        /// <summary>
        /// Metodo que busca un usuario nominado por medio del ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Models.DTO.UsuarioNominadoDTO ObtenerUsuarioNominado(long id, string estruturaFuncional)
        {
            //primero busco el usuario
            var usuarioSearch = getSession().QueryOver<Usuario>().Where(a => a.CId == id).SingleOrDefault();

            //luego voy a la vista a buscar VW_UsuarioPersimos
            var permisos = getSession().QueryOver<VWUsuarioPermisos>().Where(p => p.DUsuario == usuarioSearch.DUsuario && p.CEstructuraFuncional == estruturaFuncional).List<VWUsuarioPermisos>();

            if (usuarioSearch != null && permisos.Count > 0)
            {
                if (usuarioSearch.TipoUsuario.CCodigo == TipoUsuario.NOMINADO)
                {
                    Nominado usuario = (Nominado)usuarioSearch;
                    return CargarUsuarioNominadoDTO(usuario, permisos.First().CodigoRol);
                }
                else
                    throw new Exception("El usuario no es del tipo Nominado");
            }
            else
            {
                if(usuarioSearch == null)
                    throw new Exception("No existe un usuario con ese ID");
                else if(permisos.Count == 0)
                    throw new Exception("El usuario no posee permisos para al estructura funcional solicitada");
                else
                    throw new Exception("No se pudo especificar el error.");
            }
        }
        

        /// <summary>
        /// Metodo que obtiene una lista de usuarios nominados DTO
        /// </summary>
        /// <param name="codigoRol">Codigo de Rol</param>
        /// <returns>Lista</returns>
        internal static List<UsuarioNominadoDTO> ObtenerUsuariosNominadoPorRol(string codigoRol, string estructura)
        {
            ICriteria cRolNodoUsuario = getSession().CreateCriteria<RolNodoUsuario>();
            var estrutura = getSession().QueryOver<EstructuraFuncional>().Where(e=>e.CCodigo == estructura).SingleOrDefault();
            var rol = getSession().QueryOver<Rol>().Where(r => r.EstructuraFuncional.CId == estrutura.CId && r.Codigo == codigoRol).SingleOrDefault();
            cRolNodoUsuario.CreateAlias("Rol", "Rol");
            cRolNodoUsuario.Add(Expression.Eq("Rol.CId", rol.CId));
            List<Usuario> usuarios = cRolNodoUsuario.List<RolNodoUsuario>().ToList().Select(r=>r.Usuario).ToList();

            List<UsuarioNominadoDTO> usuarioNominados = new List<UsuarioNominadoDTO>();

            foreach (var u in usuarios)
            {
                usuarioNominados.Add(CargarUsuarioNominadoDTO((Nominado)u, codigoRol));
            }
            return usuarioNominados;
        }


        /// <summary>
        /// Metodo que carga un UsuarioNominadoDTO desde un objeto Nominado
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private static UsuarioNominadoDTO CargarUsuarioNominadoDTO(Nominado usuario, string codigoRol)
        {
            Models.DTO.UsuarioNominadoDTO usuarioDTO = new UsuarioNominadoDTO();
            usuarioDTO.Apellidos = usuario.DApellido;
            if (usuario.BAvatar != null)
                usuarioDTO.Avatar = Models.Helpers.GetThumbnails(usuario.BAvatar);
            usuarioDTO.Email = usuario.DMail;
            usuarioDTO.ID = usuario.CId;
            usuarioDTO.Nombres = usuario.DNombre;
            usuarioDTO.NombreUsuario = usuario.DUsuario;
            usuarioDTO.Telefono = usuario.DTelefono;
            usuarioDTO.CodigoRol = codigoRol;
            usuarioDTO.Cuit = usuario.Cuit;
            return usuarioDTO;
        }

    }

}