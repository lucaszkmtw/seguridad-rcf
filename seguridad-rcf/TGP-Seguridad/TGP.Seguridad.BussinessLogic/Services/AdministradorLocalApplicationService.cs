using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;

namespace TGP.Seguridad.BussinessLogic
{
    public class AdministradorLocalApplicationService : SeguridadApplicationService
    {
        private static AdministradorLocalApplicationService instance;

        //Singleton
        public new static AdministradorLocalApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdministradorLocalApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que devuelve un listado de usuarios nominados listo para la vista
        /// </summary>
        /// <returns></returns>
        public IList<ListadoAdministradoresLocalesViewModel> GetListadoNominadosAdministradores()
        {
            Search searchNominado = new Search(typeof(Nominado));
            searchNominado.AddExpression(Restrictions.And(Restrictions.IsNotNull("AdministradoresLocales"), Restrictions.IsNotEmpty("AdministradoresLocales")));
            IList<Nominado> nominados = GetByCriteria<Nominado>(searchNominado);
            IList<ListadoAdministradoresLocalesViewModel> viewModel = nominados.Adapt<IList<ListadoAdministradoresLocalesViewModel>>();
            return viewModel;
        }

        public ListadoAdministradoresLocalesViewModel GetNominadoById(long id)
        {
            Nominado nominado = GetById<Nominado>(id);
            ListadoAdministradoresLocalesViewModel viewModel = nominado.Adapt<ListadoAdministradoresLocalesViewModel>();
            return viewModel;
        }

        public bool GuardarAdminsLocales(long idUsuario, string codigoEstructura, long[] roles, long[] nodos, string nombreUsuarioLogueado)
        {
            using (var trx = BeginTransaction())
            {
                try
                {
                    Usuario usuario = GetById<Usuario>(idUsuario);
                    Search searchUsuarioLogueado = new Search(typeof(Usuario));
                    searchUsuarioLogueado.AddExpression(Restrictions.Eq("NombreUsuario", nombreUsuarioLogueado));
                    Usuario usuarioLogueado = GetByCriteria<Usuario>(searchUsuarioLogueado).FirstOrDefault();

                    string[] propiedadesRolesDelegablesExistentes = new string[]
                    {
                        "Id",
                    };
                    //Borra todos los roles delegables del usuario para esa estructura 
                    Search searchRolesDelegablesExistentes = new Search(typeof(RolDelegadoAdmin));
                    searchRolesDelegablesExistentes.AddAlias(new KeyValuePair<string, string>("AdminLocal", "AdminLocal"));
                    searchRolesDelegablesExistentes.AddAlias(new KeyValuePair<string, string>("AdminLocal.UsuarioAdmin", "UsuarioAdmin"));
                    searchRolesDelegablesExistentes.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
                    searchRolesDelegablesExistentes.AddAlias(new KeyValuePair<string, string>("Rol.EstructuraFuncional", "EstructuraFuncional"));
                    searchRolesDelegablesExistentes.AddExpression(Restrictions.Eq("UsuarioAdmin.Id", idUsuario));
                    searchRolesDelegablesExistentes.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                    List<RolDelegadoAdmin> rolesDelegadosExistentes = GetByCriteria<RolDelegadoAdmin>(searchRolesDelegablesExistentes, propiedadesRolesDelegablesExistentes).ToList();
                    if (rolesDelegadosExistentes != null && rolesDelegadosExistentes.Count > 0)
                    {
                        List<long> IdsRolesDelegablesExistentes = rolesDelegadosExistentes.Select(a => a.Id).ToList();
                        DeleteAll(IdsRolesDelegablesExistentes, typeof(RolDelegadoAdmin));
                    }
                    string[] propiedadesAdminsExistentes = new string[]
                    {
                        "Id"
                    };
                    //Borra todos los admins locales del usuario para esa estructura
                    Search searchAdminsExistentes = new Search(typeof(AdministradorLocal));
                    searchAdminsExistentes.AddAlias(new KeyValuePair<string, string>("UsuarioAdmin", "UsuarioAdmin"));
                    searchAdminsExistentes.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
                    searchAdminsExistentes.AddAlias(new KeyValuePair<string, string>("NodoFuncional.EstructuraFuncional", "EstructuraFuncional"));
                    searchAdminsExistentes.AddExpression(Restrictions.Eq("UsuarioAdmin.Id", idUsuario));
                    searchAdminsExistentes.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                    List<AdministradorLocal> adminsLocales = GetByCriteria<AdministradorLocal>(searchAdminsExistentes, propiedadesAdminsExistentes).ToList();
                    if (adminsLocales != null && adminsLocales.Count > 0)
                    {
                        List<long> IdsAdminsExistentes = adminsLocales.Select(a => a.Id).ToList();
                        DeleteAll(IdsAdminsExistentes, typeof(AdministradorLocal));
                    }

                    if (nodos != null)
                    {
                        foreach (long idNodo in nodos)
                        {
                            //guardo el nuevo admin local
                            NodoFuncional nodoFun = GetById<NodoFuncional>(idNodo);
                            AdministradorLocal admin = new AdministradorLocal
                            {
                                Id = GetIdSecuencia(typeof(AdministradorLocal)),
                                NodoFuncional = nodoFun,
                                UsuarioAdmin = usuario,
                                Usuario = usuarioLogueado,
                                UsuarioAlta = usuarioLogueado,
                                FechaUltimaOperacion = DateTime.Now
                            };
                            Insert(admin);

                            if (roles != null)
                            {
                                //guardo los RolDelegadoAdmin para ese admin
                                foreach (long idrol in roles)
                                {
                                    Rol rol = GetById<Rol>(idrol);
                                    RolDelegadoAdmin rolDelegado = new RolDelegadoAdmin
                                    {
                                        Id = GetIdSecuencia(typeof(RolDelegadoAdmin)),
                                        Rol = rol,
                                        AdminLocal = admin,
                                        Usuario = usuarioLogueado,
                                        UsuarioAlta = usuarioLogueado,
                                        FechaUltimaOperacion = DateTime.Now
                                    };
                                    SaveOrUpdate(rolDelegado);
                                }
                            }
                        }
                    }
                    trx.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    trx.Rollback();
                    //throw e;
                    return false;
                }
            }
        }

        public IList<RolDTO> GetRolesAdministrables(string codigoEstructura)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura",
                "UsuarioAlta.Id",
                "UsuarioAlta.NombreUsuario"
            };

            IList<Rol> roles;
            Search searchRoles = new Search(typeof(Rol));
            searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
            searchRoles.AddExpression(Expression.Eq("SiDelegable", true));
            roles = GetByCriteria<Rol>(searchRoles, propiedades);

            //Adaptamos la lista de roles a viewmodel
            IList<RolDTO> rolesViewModel = roles.Adapt<IList<RolDTO>>();
            return rolesViewModel;
        }

        public List<AdminsLocalesDeUsuarioEstructuraViewModel> AdminsLocalesDeUsuario(long idUsuario)
        {
            try
            {
                List<AdminsLocalesDeUsuarioEstructuraViewModel> RDYNDEVMs = new List<AdminsLocalesDeUsuarioEstructuraViewModel>();
                List<EstructuraFuncional> estructuras = GetAll<EstructuraFuncional>().ToList();
                foreach (EstructuraFuncional estructura in estructuras)
                {
                    AdminsLocalesDeUsuarioEstructuraViewModel vm = estructura.Adapt<AdminsLocalesDeUsuarioEstructuraViewModel>();

                    Search searchAdminsLocales = new Search(typeof(AdministradorLocal));
                    searchAdminsLocales.AddAlias(new KeyValuePair<string, string>("UsuarioAdmin", "UsuarioAdmin"));
                    searchAdminsLocales.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
                    searchAdminsLocales.AddAlias(new KeyValuePair<string, string>("NodoFuncional.EstructuraFuncional", "EstructuraFuncional"));
                    searchAdminsLocales.AddExpression(Restrictions.Eq("UsuarioAdmin.Id", idUsuario));
                    searchAdminsLocales.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura.Codigo));
                    IList<AdministradorLocal> admins = GetByCriteria<AdministradorLocal>(searchAdminsLocales);
                    if (admins != null && admins.Count > 0)
                    {
                        vm.AdministradoresLocales = admins.Adapt<List<AdministradorLocalDTO>>();
                        vm.Estructura = estructura.DescripcionEstructura;
                        RDYNDEVMs.Add(vm);
                    }
                }
                return RDYNDEVMs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<long> RolesAdministradosPorUsuarioYCodigoEstructura(long idUsuario, string codEstructura)
        {

            string[] propiedadesRolDelegado = new string[]
            {
                "Rol.Id",
            };

            Search searchRolDelegado = new Search(typeof(RolDelegadoAdmin));
            searchRolDelegado.AddAlias(new KeyValuePair<string, string>("AdminLocal", "AdminLocal"));
            searchRolDelegado.AddAlias(new KeyValuePair<string, string>("AdminLocal.UsuarioAdmin", "UsuarioAdmin"));
            searchRolDelegado.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolDelegado.AddAlias(new KeyValuePair<string, string>("Rol.EstructuraFuncional", "EstructuraFuncional"));
            searchRolDelegado.AddExpression(Restrictions.Eq("UsuarioAdmin.Id", idUsuario));
            searchRolDelegado.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codEstructura));
            IList<RolDelegadoAdmin> rolesDelegados = GetByCriteria<RolDelegadoAdmin>(searchRolDelegado, propiedadesRolDelegado);
            List<long> idsRolesDelegados = rolesDelegados.Select(a => a.Rol.Id).Distinct().ToList();
            return idsRolesDelegados;
        }

        public List<long> NodosAdministradosPorUsuarioYCodigoEstructura(long idUsuario, string codEstructura)
        {

            string[] propiedadesAdminLocal = new string[]
            {
                "NodoFuncional.Id",
            };

            Search searchAdminsLocales = new Search(typeof(AdministradorLocal));
            searchAdminsLocales.AddAlias(new KeyValuePair<string, string>("UsuarioAdmin", "UsuarioAdmin"));
            searchAdminsLocales.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
            searchAdminsLocales.AddAlias(new KeyValuePair<string, string>("NodoFuncional.EstructuraFuncional", "EstructuraFuncional"));
            searchAdminsLocales.AddExpression(Restrictions.Eq("UsuarioAdmin.Id", idUsuario));
            searchAdminsLocales.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codEstructura));
            IList<AdministradorLocal> admins = GetByCriteria<AdministradorLocal>(searchAdminsLocales, propiedadesAdminLocal);
            List<long> idsRolesDelegados = admins.Select(a => a.NodoFuncional.Id).ToList();
            return idsRolesDelegados;
        }

        public int GetCantidadDEAdminsPorRol(long idRol)
        {
            ISession sesion = Session();
            IQuery query = sesion.CreateSQLQuery(string.Format(@"Select u.c_id
                            FROM seg_rol_delegado_admin del
                                    inner Join seg_administrador_local ad on del.c_id_admin_local = ad.c_id
                                    inner join seg_usuario u on u.c_id = ad.c_id_usuario_admin
                            WHERE del.c_id_rol = {0}
                            GROUP BY u.c_id", idRol));
            int cant = query.List<long>().Count();
            return cant;
        }

        public List<AuditoriaAdminDTO> GetHistorialAdministrador(long idUsuarioAdmin)
        {
            string[] propiedades = new string[]
            {
                "DescripcionRol",
                "DescripcionNodoFuncional",
                "NombreUsuario",
                "FechaOperacion",
                "DescripcionOperacion",
                "DescripcionEstructura"
            };
            Search search = new Search(typeof(AuditoriaAdministradorLocal));
            search.AddExpression(Restrictions.Eq("CodigoAdminLocal", idUsuarioAdmin));
            IList<AuditoriaAdministradorLocal> historial = GetByCriteria<AuditoriaAdministradorLocal>(search, propiedades);
            return historial.Adapt<List<AuditoriaAdminDTO>>();
        }
    }
}
