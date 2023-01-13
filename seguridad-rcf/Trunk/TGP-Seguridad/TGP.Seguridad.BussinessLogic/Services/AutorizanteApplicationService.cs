
using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;
using Utils;

namespace TGP.Seguridad.BussinessLogic
{
    public class AutorizanteApplicationService : SeguridadApplicationService
    {
        private static AutorizanteApplicationService instance;

        private static readonly long ID_TIPO_USUARIO_NOMINADO = 2;
        private static readonly string CODIGO_ORGANO_RECTOR = "0";
        //Codigo rol Contador Delegado
        private static readonly string COD_ROL_CON_DELEG = "AUTORIZ-CON-DELE";
        //Codigo rol Autorizante Mov. Fondos
        private static readonly string COD_ROL_MOV_FON = "AUTORIZ-MOV-FON";

        //Singleton
        public static new AutorizanteApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AutorizanteApplicationService();
                }
                return instance;
            }
        }

        public ResponseAPINominados GetNominados(List<string> codigosRoles)
        {
            ResponseAPINominados response = new ResponseAPINominados();

            Search searchUsuarios = new Search(typeof(Usuario));
            KeyValuePair<string, string> aliasTipoUsuario = new KeyValuePair<string, string>("TipoUsuario", "TipoUsuario");
            KeyValuePair<string, string> aliasRNU = new KeyValuePair<string, string>("RolesNodoUsuario", "RolesNodoUsuario");
            KeyValuePair<string, string> aliasRol = new KeyValuePair<string, string>("RolesNodoUsuario.Rol", "Rol");
            searchUsuarios.AddAlias(aliasTipoUsuario);
            searchUsuarios.AddAlias(aliasRNU);
            searchUsuarios.AddAlias(aliasRol);
            searchUsuarios.AddExpression(Expression.Eq("TipoUsuario.Id", ID_TIPO_USUARIO_NOMINADO));
            if (codigosRoles.Count > 0)
            {
                searchUsuarios.AddExpression(Expression.In("Rol.Codigo", codigosRoles));
            }

            IList<Usuario> usuarios = GetByCriteria<Usuario>(searchUsuarios);
            IList<NominadoResponse> nominados = usuarios.GroupBy(n => n.Id).Select(nom => new NominadoResponse()
            {
                IdUsuario = nom.Key,
                DescripcionUsuario = nom.First().Descripcion,
                MailUsuario = nom.First().EMail,
                Avatar = nom.First().Avatar,
                EsContadorDelegado = nom.Last().EsContadorDelegado,
                Nodos = nom.Select(r => r.RolesNodoUsuario.Where(rnu => rnu.NodoFuncional.EstructuraFuncional.Codigo == "CONC" && codigosRoles.Contains(rnu.Rol.Codigo)).Select(n => n.NodoFuncional.Descripcion).ToList()).First()
            }).ToList();

            response.Nominados = nominados.Adapt<IList<NominadoResponse>>().ToList();
            return response;
        }

        public ResponseAPINominadoAutocomplete GetNominadoAutocomplete(string query)
        {
            ResponseAPINominadoAutocomplete response = new ResponseAPINominadoAutocomplete();

            Search searchNominados = new Search(typeof(Nominado));
            KeyValuePair<string, string> aliasTipoUsuario = new KeyValuePair<string, string>("TipoUsuario", "TipoUsuario");
            searchNominados.AddAlias(aliasTipoUsuario);
            searchNominados.AddExpression(Expression.Eq("TipoUsuario.Id", ID_TIPO_USUARIO_NOMINADO));
            if (!String.IsNullOrEmpty(query))
                searchNominados.AddExpression(Restrictions.Or(Restrictions.InsensitiveLike("Apellido", "%" + query + "%"), Restrictions.InsensitiveLike("NombreNominado", "%" + query + "%")));

            string[] propiedades = new string[]
            {
                "Id",
                "NombreNominado",
                "Apellido"
            };

            IList<NominadoAutocompleteResponse> nominados = GetByCriteria<Nominado>(searchNominados, propiedades)
            .GroupBy(n => n.Id).Select(nom => new NominadoAutocompleteResponse()
            {
                IdUsuario = nom.Key,
                DescripcionUsuario = nom.First().GetDescripcionUsuario()
            }).ToList();

            response.Nominados = nominados.Adapt<IList<NominadoAutocompleteResponse>>().ToList();
            return response;
        }

        public ResponseAPIRolNodoUsuario GetRolNodoUsuario(long idUsuario)
        {
            ResponseAPIRolNodoUsuario response = new ResponseAPIRolNodoUsuario();

            Search searchRNU = new Search(typeof(RolNodoUsuario));
            KeyValuePair<string, string> aliasUsuario = new KeyValuePair<string, string>("Usuario", "Usuario");
            KeyValuePair<string, string> aliasRol = new KeyValuePair<string, string>("Rol", "Rol");
            searchRNU.AddAlias(aliasUsuario);
            searchRNU.AddAlias(aliasRol);
            searchRNU.AddExpression(Expression.Eq("Usuario.Id", idUsuario));
            List<string> codRoles = new List<string>() { COD_ROL_CON_DELEG, COD_ROL_MOV_FON };
            searchRNU.AddExpression(Expression.In("Rol.Codigo", codRoles));
            IList<RolNodoUsuario> rnu = GetByCriteria<RolNodoUsuario>(searchRNU);
            RolNodoUsuarioResponse rnuResponse = rnu
                .GroupBy(n => n.Usuario.Id).Select(rolNodoUsuario => new RolNodoUsuarioResponse()
                {
                    NodosFuncionales = rolNodoUsuario.Select(r => r.NodoFuncional).Adapt<IList<NodoResponse>>().ToList(),
                    Rol = rolNodoUsuario.First().Rol.Adapt<RolResponse>()
                }).First();

            response.RolesNodoUsuario = rnuResponse;
            return response;
        }

        public ResponseAPIRoles GetRoles(List<string> codRoles, string codEstructuraFuncional)
        {
            ResponseAPIRoles response = new ResponseAPIRoles();
            Search searchRoles = new Search(typeof(Rol));
            KeyValuePair<string, string> aliasEstructuraFuncional = new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional");
            searchRoles.AddAlias(aliasEstructuraFuncional);
            searchRoles.AddExpression(Expression.Eq("EstructuraFuncional.Codigo", codEstructuraFuncional));
            searchRoles.AddExpression(Expression.In("Codigo", codRoles));
            string[] propiedades = new string[]
            {
                "Id",
                "Descripcion"
            };

            IList<Rol> roles = GetByCriteria<Rol>(searchRoles, propiedades);
            response.Roles = roles.Adapt<IList<RolResponse>>().ToList();
            return response;
        }

        public ResponseAPINodos GetNodos(string codEstructuraFuncional, long? idNominado)
        {
            ResponseAPINodos response = new ResponseAPINodos();
            Search searchNodos = new Search(typeof(NodoFuncional));
            KeyValuePair<string, string> aliasEstructuraFuncional = new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional");
            searchNodos.AddAlias(aliasEstructuraFuncional);
            searchNodos.AddExpression(Expression.Eq("EstructuraFuncional.Codigo", codEstructuraFuncional));
            searchNodos.AddExpression(Expression.Not(Expression.Eq("Codigo", CODIGO_ORGANO_RECTOR)));

            if (idNominado != null)
            {
                KeyValuePair<string, string> aliasUsuario = new KeyValuePair<string, string>("Usuario", "Usuario");
                searchNodos.AddAlias(aliasUsuario);
                searchNodos.AddExpression(Expression.Eq("Usuario.Id", idNominado));
            }
            string[] propiedades = new string[]
            {
                "Id",
                "Descripcion",
                "Codigo"
            };

            IList<NodoFuncional> nodos = GetByCriteria<NodoFuncional>(searchNodos, propiedades);
            response.Nodos = nodos.Adapt<IList<NodoResponse>>().ToList();

            return response;
        }

        public ResponseAPI EliminarPermisosAutorizante(string query)
        {
            long idUsuario = long.Parse(query);
            ResponseAPI response = new ResponseAPI();
            try
            {
                List<long> rnus = GetRolesNodoUsuario(idUsuario).Select(rnu => rnu.Id).ToList();
                DeleteAll<RolNodoUsuario>(rnus, true);
                response.Codigo = ResponseAPI.CodigoResponseSuccess;
                response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                return response;
            }
            catch (Exception eapi)
            {
                throw eapi;
            }
        }

        private List<RolNodoUsuario> GetRolesNodoUsuario(long idUsuario)
        {
            List<string> codsRoles = new List<string>() { COD_ROL_CON_DELEG, COD_ROL_MOV_FON };
            Search searchRNU = new Search(typeof(RolNodoUsuario));
            KeyValuePair<string, string> aliasRol = new KeyValuePair<string, string>("Rol", "Rol");
            KeyValuePair<string, string> aliasUsuario = new KeyValuePair<string, string>("Usuario", "Usuario");
            searchRNU.AddAlias(aliasRol);
            searchRNU.AddAlias(aliasUsuario);
            searchRNU.AddExpression(Expression.Eq("Usuario.Id", idUsuario));
            searchRNU.AddExpression(Expression.In("Rol.Codigo", codsRoles));
            string[] propiedades = new string[]
            {
                "Id"
            };

            IList<RolNodoUsuario> rnu = GetByCriteria<RolNodoUsuario>(searchRNU, propiedades);
            return rnu.Adapt<IList<RolNodoUsuario>>().ToList();
        }

        public ResponseAPI SetPermisos(long? IdRol, List<long> IdsNodos, long IdUsuario)
        {
            using (var trx = BeginTransaction())
            {
                ResponseAPI response = new ResponseAPI();
                try
                {
                    IList<long> idsRNU = GetRolesNodoUsuario(IdUsuario).Select(rnu => rnu.Id).ToList();
                    DeleteAll<RolNodoUsuario>(idsRNU);
                    long id = Convert.ToInt64(IdRol);
                    Rol rol = GetById<Rol>(id);
                    Usuario usuario = GetById<Usuario>(IdUsuario);
                    foreach (long idNodo in IdsNodos)
                    {
                        NodoFuncional nodoFuncional = GetById<NodoFuncional>(idNodo);
                        RolNodoUsuario rnuAutorizante = new RolNodoUsuario()
                        {
                            Rol = rol,
                            NodoFuncional = nodoFuncional,
                            Usuario = usuario,
                            UsuarioAlta = usuario,
                            FechaUltimaOperacion = DateTime.Now
                        };
                        SaveOrUpdate(rnuAutorizante);
                    }
                    Session().Flush();
                    trx.Commit();
                    response.Codigo = ResponseAPI.CodigoResponseSuccess;
                    if (idsRNU.Count > 0)
                    {
                        response.Mensaje = "El Autorizante fue modificado correctamente.";
                    }
                    else if (idsRNU.Count == 0)
                    {
                        response.Mensaje = "El Autorizante fue dado de alta correctamente.";
                    }
                    else
                    {
                        response.Mensaje = ResponseAPI.MensajeRepsonseSuccess;
                    }
                    return response;
                }
                catch (Exception eapi)
                {
                    trx.Rollback();
                    throw eapi;
                }
            }
        }

        /// <summary>
        /// Metodo que valida la existencia de un usuario
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="contraseña"></param>
        /// <returns></returns>
        public bool SiValidaUsuario(string nombreUsuario)
        {
            ISession session = Session();
            Usuario usuario = session.QueryOver<Usuario>().Where(u => u.NombreUsuario == nombreUsuario).SingleOrDefault();
            return usuario != null;
        }

        /// <summary>
        /// Metodo que valida que al key del llamado sea correcta
        /// </summary>
        /// <param name="keyEncrypt"></param>
        /// <returns></returns>
        public bool SiValidaKeyWSS(string keyEncrypt)
        {

            string key = Encriptador.Desencriptar(keyEncrypt);
            return key == ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString();
        }
    }

}
