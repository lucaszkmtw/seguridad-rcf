
using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.BussinessLogic.Generics;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;
using HelperService = TGP.Seguridad.DataAccess.Helpers.HelperService;

namespace TGP.Seguridad.BussinessLogic
{
    public class RolApplicationService : SeguridadApplicationService
    {
        private static RolApplicationService instance;

        //Singleton
        public static new RolApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RolApplicationService();
                }
                return instance;
            }
        }


        #region ABM Rol y Listado de Roles
        /// <summary>
        /// Metodo que verifica si el codigo del rol se encuentra en ambiente destino
        /// </summary>
        /// <param name="rol"></param>
        public string PrevGuardarRol(RolDTO rol)
        {

            //IList<ElementoComparar> rolesEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ROL, rol.EstructuraFuncional);
            //if (rolesEnDestino.Any(x => x.Codigo.ToLower() == rol.Codigo.ToLower()))
            //    return "Existe un elemento con el mismo código en el ambiente destino. ¿Desea generarlo de todas formas?";

            return string.Empty;
        }

        /// <summary>
        /// Metodo que guarda un rol segun el viewmodel pasado
        /// </summary>
        /// <param name="rol"></param>
        public void GuardarRol(RolDTO rol)
        {
            //if (ExisteCodigoElemento<Rol>(rol.Codigo))
            //    throw new Exception("Ya existe un rol con el mismo codigo.");
            //if (String.IsNullOrEmpty(rol.Codigo))
            //    throw new Exception("Debe ingresar un código para el rol.");
            //if (String.IsNullOrEmpty(rol.Descripcion))
            //    throw new Exception("Debe ingresar una descripción para el rol.");

            Rol rolGuardar = new Rol();

            //IList<ElementoComparar> rolesEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ROL, rol.EstructuraFuncional);
            //if (rolesEnDestino.Any(x => x.Codigo.ToLower() == rol.Codigo.ToLower()))
            //{
            //    //copio el ID del rol destino para evitar IDs desfasados entre los ambientes
            //    long idRolDestino = rolesEnDestino.Where(x => x.Codigo.ToLower() == rol.Codigo.ToLower()).FirstOrDefault().Id;
            //    rolGuardar.Id = idRolDestino;
            //}
            //else
            //{
            //}
            rolGuardar.Id = GetIdSecuencia(typeof(Rol));

            //Buscamos las actividades que vienen en el viewmodel, si no viene ninguna la instanciamos vacia
            IList<Actividad> actividades = (rol.Actividades != null) ? GetActividadesPorCodigo(rol.Actividades.ToArray(), rol.EstructuraFuncional) : new List<Actividad>();

            //Instanciamos la lista de actividades del rol
            rolGuardar.Actividades = new List<RolActividad>();
            rolGuardar.Codigo = rol.Codigo;
            rolGuardar.Descripcion = rol.Descripcion;
            rolGuardar.SiDelegable = rol.SiDelegable;
            rolGuardar.InformacionRol = rol.InformacionRol;
            rolGuardar.EsMultinodo = rol.EsMultinodo == true ? 1 : 0;
            rolGuardar.EstructuraFuncional = GetEstructuraPorCodigo(rol.EstructuraFuncional);

            //TGPSEG-271, TGPSEG-272. Si se selecciona la opcion todos como tipo de nodo entonces se guarda como null. Indica que tiene multiples tipos de nodos funcionales.
            rolGuardar.TipoNodoFuncional = rol.TipoNodoFuncional != "TODOS" ? GetTipoNodoFuncionalPorCodigo(rol.TipoNodoFuncional) : null;

            rolGuardar.UsuarioAlta = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"]);
            rolGuardar.FechaUltimaOperacion = HelperService.Instance.GetDateToday();

            using (var trx = BeginTransaction())
            {
                Insert(rolGuardar);
                //Por cada actividad agregamos al rol un RolActividad
                foreach (Actividad act in actividades)
                {
                    rolGuardar.Actividades.Add(new RolActividad()
                    {
                        IdActividad = act.Id,
                        IdRol = rolGuardar.Id,
                        UsuarioResponsable = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"]),
                        FechaUltimaOperacion = HelperService.Instance.GetDateToday()
                    });
                }
                trx.Commit();
            }

        }

        /// <summary>
        /// Metodo que obtiene los roles para mostrar en el listado
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public IList<RolDTO> GetRolesListadoViewModel(string codigoEstructura)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura",
                "TipoNodoFuncional.Codigo",
                "UsuarioAlta.Id",
                "UsuarioAlta.NombreUsuario",
                "EsMultinodo"
            };

            IList<Rol> roles;

            //Si el codigo de Estructura es 0, hacemos un getall
            if (codigoEstructura.Equals(ComboGenerico.ComboVacio))
                roles = GetAll<Rol>(propiedades);
            else
            {
                Search searchRoles = new Search(typeof(Rol));
                searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                roles = GetByCriteria<Rol>(searchRoles, propiedades);
            }

            //Adaptamos la lista de roles a viewmodel
            IList<RolDTO> rolesViewModel = roles.Adapt<IList<RolDTO>>();
            return rolesViewModel;
        }
        /// <summary>
        /// Metodo que obtiene el rol a editar y lo transforma a viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RolDTO GetRolEditar(long id)
        {
            Rol rolEditar = GetById<Rol>(id);
            RolDTO rol = rolEditar.Adapt<RolDTO>();
            return rol;
        }


        /// <summary>
        /// Metodo que guarda la edicion de un rol
        /// </summary>
        /// <param name="rol"></param>
        public void GuardarEdicionRol(RolDTO rol)
        {

            Rol rolEditar = GetById<Rol>(rol.Id);

            if (ExisteCodigoElemento<Rol>(rol.Codigo) && rol.Codigo != rolEditar.Codigo)
                throw new Exception("Ya existe un rol con el mismo codigo.");
            //Si el version es distinto lanzamos excepcion
            if (rolEditar.Version != rol.Version)
                throw new Exception("El Rol fue editado por otro usuario");
            if (String.IsNullOrEmpty(rol.Codigo))
                throw new Exception("Debe ingresar un código para el rol.");
            if (String.IsNullOrEmpty(rol.Descripcion))
                throw new Exception("Debe ingresar una descripción para el rol.");


            //Copiamos las propiedades si son distintas
            if (rolEditar.Codigo != rol.Codigo)
            {
                IList<ElementoComparar> rolesEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ROL, rol.EstructuraFuncional);
                if (rolesEnDestino.Any(x => x.Codigo.ToLower() == rol.Codigo.ToLower()))
                    throw new Exception("Existe un elemento con el mismo código en el ambiente destino. Corrobore los datos cargados.");

                rolEditar.Codigo = rol.Codigo;
            }

            if (rolEditar.Descripcion != rol.Descripcion)
                rolEditar.Descripcion = rol.Descripcion;

            if (rolEditar.EstructuraFuncional.Codigo != rol.EstructuraFuncional)
                rolEditar.EstructuraFuncional = GetEstructuraPorCodigo(rol.EstructuraFuncional);

            if (rolEditar.TipoNodoFuncional == null || rolEditar.TipoNodoFuncional.Codigo != rol.TipoNodoFuncional)
                rolEditar.TipoNodoFuncional = rol.TipoNodoFuncional != "TODOS" ? GetTipoNodoFuncionalPorCodigo(rol.TipoNodoFuncional) : null;

            if (rolEditar.SiDelegable != rol.SiDelegable)
                rolEditar.SiDelegable = rol.SiDelegable;

            if (rolEditar.InformacionRol != rol.InformacionRol)
                rolEditar.InformacionRol = rol.InformacionRol;

            int esMultinodo = rol.EsMultinodo ? 1 : 0;
            if (rolEditar.EsMultinodo != esMultinodo)
                rolEditar.EsMultinodo = esMultinodo;

            //Buscamos las actividades del viewmodel si es que tiene
            IList<Actividad> actividades = (rol.Actividades != null) ? GetActividadesPorCodigo(rol.Actividades.ToArray(), rol.EstructuraFuncional) : new List<Actividad>();

            //Abrimos una transaccion porque vamos a borrar las asociasciones y vovler a crearlas si es necesario
            //TODO: Investigar como poder comparar ambos listados de actividades y borrar o crear solo las q hagan falta
            using (var trx = BeginTransaction())
            {
                try
                {
                    //TGPSEG-148
                    if (!rolEditar.SiDelegable)
                    {
                        //Elimino las relaciones de ese Rol
                        Search searchRolDelegadoAdminPorRol = new Search(typeof(RolDelegadoAdmin));
                        searchRolDelegadoAdminPorRol.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
                        searchRolDelegadoAdminPorRol.AddExpression(Restrictions.Eq("Rol.Id", rol.Id));
                        string[] propiedadesRolesDelegablesExistentes = new string[] { "Id", "AdminLocal" };
                        List<RolDelegadoAdmin> listaRolDelegadoAdminPorRol = GetByCriteria<RolDelegadoAdmin>(searchRolDelegadoAdminPorRol, propiedadesRolesDelegablesExistentes).ToList();
                        if (listaRolDelegadoAdminPorRol != null && listaRolDelegadoAdminPorRol.Count > 0)
                        {
                            List<long> IdsRolesDelegablesExistentes = listaRolDelegadoAdminPorRol.Select(a => a.Id).ToList();
                            DeleteAll(IdsRolesDelegablesExistentes, typeof(RolDelegadoAdmin));

                            //Elimino todos los admins locales sin Rol
                            List<long> idsALsinRol = listaRolDelegadoAdminPorRol.Select(al => al.AdminLocal.UsuarioAdmin.Id).ToList();
                            Search searchAdminsSinRol = new Search(typeof(AdministradorLocal));
                            searchAdminsSinRol.AddExpression(Restrictions.IsEmpty("RolesDelegadosAdmins"));
                            searchAdminsSinRol.AddExpression(Restrictions.In("UsuarioAdmin.Id", idsALsinRol));
                            string[] propiedadesAdminsExistentes = new string[] { "Id" };
                            List<AdministradorLocal> adminsLocales = GetByCriteria<AdministradorLocal>(searchAdminsSinRol, propiedadesAdminsExistentes).ToList();
                            if (adminsLocales != null && adminsLocales.Count > 0)
                            {
                                List<long> IdsAdminsExistentes = adminsLocales.Select(a => a.Id).ToList();
                                DeleteAll(IdsAdminsExistentes, typeof(AdministradorLocal));
                            }
                        }
                    }

                    //Limpiamos las actividades del rol y hacemos update sin commit
                    rolEditar.Actividades.Clear();
                    Update(rolEditar);

                    //Volvemos a crear las relaciones necesarias
                    foreach (Actividad act in actividades)
                    {
                        rolEditar.Actividades.Add(new RolActividad()
                        {
                            IdActividad = act.Id,
                            IdRol = rolEditar.Id,
                            UsuarioResponsable = GetById<Usuario>((long)HttpContext.Current.Session["usuarioId"]),
                            FechaUltimaOperacion = HelperService.Instance.GetDateToday()
                        });
                    }

                    //Hacemos update con commit
                    Update(rolEditar);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }


        }

        public IList<UsuarioAutoComplete> GetUsuariosAutoComplete(bool buscarNominados, string query = "")
        {

            List<UsuarioAutoComplete> usuarios = new List<UsuarioAutoComplete>();

            if (buscarNominados)
            {
                string[] propiedadesNominado = new string[]
            {
                "Id",
                "NombreNominado",
                "Apellido",
                "NombreUsuario"
            };
                Search searchNominados = new Search(typeof(Nominado));
                searchNominados.AddExpression(Restrictions.Eq("SiActivo", true));
                searchNominados.AddExpression(Restrictions.Or(
                        Restrictions.InsensitiveLike("NombreNominado", "%" + query + "%"),
                        Restrictions.InsensitiveLike("NombreUsuario", "%" + query + "%"))
                );
                usuarios.AddRange(GetByCriteria<Nominado>(searchNominados, propiedadesNominado).Adapt<IList<UsuarioAutoComplete>>());
            }
            else
            {
                string[] propiedadesAcreedor = new string[]
            {
                "Id",
                "RazonSocial",
                "NumeroCuit",
                "NombreUsuario"
            };
                Search searchAcreedor = new Search(typeof(Acreedor));
                searchAcreedor.AddExpression(Restrictions.Eq("SiActivo", true));
                searchAcreedor.AddExpression(Restrictions.Or(
                        Restrictions.InsensitiveLike("RazonSocial", "%" + query + "%"),
                        Restrictions.InsensitiveLike("NumeroCuit", "%" + query + "%"))
                );

                usuarios.AddRange(GetByCriteria<Acreedor>(searchAcreedor, propiedadesAcreedor).Adapt<IList<UsuarioAutoComplete>>());
            }            
            return usuarios;
        }


        #endregion

        /// <summary>
        /// Metodo que eliminar un rol
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void DeleteRol(long id, int version)
        {

            //Buscamos el version q esta en la base, si no coincide lanzamos excepcion
            Rol rol = GetById<Rol>(id);

            if (rol.Version != version)
                throw new Exception("El Rol fue modificado por otro usuario");

            string[] propiedades = new string[]
            {
                "Id",
                "Novedad",
            };


            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Rol.Id", id));
            IList<long> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario, new string[] { "Id" }).Select(x => x.Id).ToList();

            //verifico si el rol tiene novedades asociadas
            Search searchNovedadesRolNodo = new Search(typeof(NovedadRolNodo));
            searchNovedadesRolNodo.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchNovedadesRolNodo.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            searchNovedadesRolNodo.AddExpression(Restrictions.Eq("Rol.Id", id));
            IList<NovedadRolNodo> novedadesRolNodo = GetByCriteria<NovedadRolNodo>(searchNovedadesRolNodo, propiedades);

            using (var trx = BeginTransaction())
            {
                try
                {
                    DeleteAll<RolNodoUsuario>(rolesNodoUsuario, false);

                    //si posee novedades asociadas
                    if (novedadesRolNodo.Count > 0)
                    {
                        long[] idNovedades = novedadesRolNodo.Select(x => x.Novedad.Id).ToArray();

                        //busco por novedades para saber si poseen mas de un rol asociado
                        Search searchNovedadesRoles = new Search(typeof(NovedadRolNodo));
                        searchNovedadesRoles.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
                        searchNovedadesRoles.AddExpression(Restrictions.In("Novedad.Id", idNovedades));
                        IList<NovedadRolNodo> novedadesRoles = GetByCriteria<NovedadRolNodo>(searchNovedadesRoles, propiedades);

                        //Si existe novedades asociadas solo a ese rol, las borro
                        if (novedadesRoles.Count == novedadesRolNodo.Count)
                        {
                            EliminarNovedadesLeidasYguardadas(idNovedades);
                            //borro las novedades.
                            DeleteAll<Novedad>(novedadesRolNodo.Select(x => x.Novedad.GetId()).ToList(), false);
                        }
                        else
                        {   //sino solo borro la relacion del rol con la novedad

                            //verifico si existen novedades solo para ese rol. en caso de existir, las borro
                            long[] idNovedadesRol = novedadesRoles.GroupBy(x => x.Novedad.Id).Select(x => new
                            {
                                id = x.FirstOrDefault().Novedad.Id,
                                cant = x.Count(),
                            }).Where(x => x.cant == 1).Select(z => z.id).ToArray();

                            // borro la relacion del rol con la novedades
                            DeleteAll<NovedadRolNodo>(novedadesRolNodo.Select(x => x.GetId()).ToList(), false);

                            //borro las novedades.(leidas y guardadas tambien)
                            EliminarNovedadesLeidasYguardadas(idNovedadesRol);
                            DeleteAll<Novedad>(idNovedadesRol, false);
                        }
                    }

                    Delete<Rol>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }

        private void EliminarNovedadesLeidasYguardadas(long[] idNovedades)
        {
            //Search de novedad leidas
            Search searchNovedadLeida = new Search(typeof(NovedadLeida));
            searchNovedadLeida.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            searchNovedadLeida.AddExpression(Restrictions.In("Novedad.Id", idNovedades));
            IList<NovedadLeida> novedadLeida = GetByCriteria<NovedadLeida>(searchNovedadLeida, new string[] { "Id" });

            //Search de novedad guardadas
            Search searchNovedadGuardada = new Search(typeof(NovedadGuardada));
            searchNovedadGuardada.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            searchNovedadGuardada.AddExpression(Restrictions.In("Novedad.Id", idNovedades));
            IList<NovedadGuardada> novedadGuardada = GetByCriteria<NovedadGuardada>(searchNovedadGuardada, new string[] { "Id" });

            //Si existe novedades leidas asociados a la novedad las borro
            if (novedadLeida.Count > 0)
                DeleteAll<NovedadLeida>(novedadLeida.Select(x => x.GetId()).ToList(), false);

            //Si existe novedades guardadas asociados a la novedad las borro
            if (novedadLeida.Count > 0)
                DeleteAll<NovedadGuardada>(novedadGuardada.Select(x => x.GetId()).ToList(), false);
        }

        /// <summary>
        /// Metodo que retorna los usuarios para la asignacion masiva de roles
        /// </summary>
        /// <returns></returns>
        public IList<UsuarioAutoComplete> UsuariosMultiSelect()
        {
            List<UsuarioAutoComplete> usuarios = new List<UsuarioAutoComplete>();

            string[] propiedadesNominado = new string[]
            {
                "Id",
                "NombreNominado",
                "Apellido",
                "NombreUsuario"
            };
            Search searchNominados = new Search(typeof(Nominado));
            searchNominados.AddExpression(Restrictions.Eq("SiActivo", true));
            usuarios.AddRange(GetByCriteria<Nominado>(searchNominados, propiedadesNominado).Adapt<IList<UsuarioAutoComplete>>());

            string[] propiedadesAcreedor = new string[]
            {
                "Id",
                "RazonSocial",
                "NumeroCuit",
                "NombreUsuario"
            };
            Search searchAcreedor = new Search(typeof(Acreedor));
            searchAcreedor.AddExpression(Restrictions.Eq("SiActivo", true));
            //usuarios.AddRange(GetByCriteria<Nominado>(searchNominados, propiedadesNominado).Adapt<IList<UsuarioAutoComplete>>());
            usuarios.AddRange(GetByCriteria<Acreedor>(searchAcreedor, propiedadesAcreedor).Adapt<IList<UsuarioAutoComplete>>());
            return usuarios;
        }

        /// <summary>
        /// Metodo que retorna los usuarios para la asignacion masiva de roles en funcion del parámetro tipoUsuario
        /// </summary>
        /// <param name="tipoUsuario"></param>
        /// <returns></returns>
        public IList<UsuarioAutoComplete> UsuariosMultiSelect(string tipoUsuario)
        {
            List<UsuarioAutoComplete> usuarios = new List<UsuarioAutoComplete>();

            if (tipoUsuario == "N")
            {
                string[] propiedadesNominado = new string[]
                {
                "Id",
                "NombreNominado",
                "Apellido",
                "NombreUsuario"
                };
                Search searchNominados = new Search(typeof(Nominado));
                searchNominados.AddExpression(Restrictions.Eq("SiActivo", true));
                usuarios.AddRange(GetByCriteria<Nominado>(searchNominados, propiedadesNominado).Adapt<IList<UsuarioAutoComplete>>());
            }
            else if (tipoUsuario == "A")
            {
                string[] propiedadesAcreedor = new string[]
                {
                "Id",
                "RazonSocial",
                "NumeroCuit",
                "NombreUsuario"
                };
                Search searchAcreedor = new Search(typeof(Acreedor));
                searchAcreedor.AddExpression(Restrictions.Eq("SiActivo", true));
                usuarios.AddRange(GetByCriteria<Acreedor>(searchAcreedor, propiedadesAcreedor).Adapt<IList<UsuarioAutoComplete>>());
            }

            return usuarios;
        }

        /// <summary>
        /// Metodo que asigna de forma masiva los roles a los usuarios si es que no los tienen
        /// </summary>
        public void AsignarRolesMasivamente(string Estructura, string Rol, List<string> Nodos, List<string> Usuarios)
        {
            Search searchRolNodoUsuario;            

            //Rol a asignar masivamente
            Rol rol = GetRolPorCodigo(Rol, Estructura);

            if (Nodos != null)
            {
                //Por cada codigo de nodo
                foreach (string codigoNodo in Nodos)
                {
                    NodoFuncional nodo = GetNodoPorCodigo(codigoNodo, Estructura);

                    //Por cada nombre de usuario
                    foreach (string idUsuario in Usuarios)
                    {

                        string nombreUsuario = GetNombreUsuarioPorId(idUsuario);
                        Usuario usuario = GetUsuarioPorNombreUsuario(nombreUsuario);

                        //Buscamos si existe una asociacion entre este rol, nodo y usuario
                        searchRolNodoUsuario = new Search(typeof(RolNodoUsuario));
                        searchRolNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
                        searchRolNodoUsuario.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
                        searchRolNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
                        searchRolNodoUsuario.AddExpression(Restrictions.Eq("Rol.Id", rol.Id));
                        searchRolNodoUsuario.AddExpression(Restrictions.Eq("NodoFuncional.Id", nodo.Id));
                        searchRolNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", usuario.Id));

                        bool tieneRolAsignado = GetCountByCriteria<RolNodoUsuario>(searchRolNodoUsuario) > 0;

                        //Si no existe relacion insertamos una nueva
                        if (!tieneRolAsignado)
                        {
                            RolNodoUsuario rolNodoUsuario = new RolNodoUsuario();
                            rolNodoUsuario.NodoFuncional = nodo;
                            rolNodoUsuario.Rol = rol;
                            rolNodoUsuario.Usuario = usuario;
                            rolNodoUsuario.UsuarioAlta = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
                            rolNodoUsuario.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                            Insert(rolNodoUsuario);
                        }
                    }
                }
            }
        }

        private string GetNombreUsuarioPorId(string idUsuario)
        {
            return GetById<Usuario>(long.Parse(idUsuario)).NombreUsuario;
        }

        /// <summary>
        /// Metodo que asigna de forma masiva el/los rol/es de un usuario "copiaMasivaPermisos.Id" a los usuarios seleccionados en "copiaMasivaPermisos.Usuarios"
        /// Devuelve un string con los permisos copiados y los usuarios destinatarios involucrados en la transaccion.
        /// </summary>
        /// <param name="copiaMasivaPermisos"></param>
        public string CopiarRolesMasivamente(CopiaMasivaPermisosViewModel copiaMasivaPermisos)
        {
            string mensajeListaEstructurasRoles = "";
            string mensajeListaEstructuras = "";
            string mensajeListaRoles = "";
            string mensajeListaUsuarios = "";

            //Recupero los Permisos del Usuario Origen, de acuerdo a la/s Estructura/s seleccionada/s
            IList<PermisosViewModel> listaPermisos = new List<PermisosViewModel>();
            foreach (string codigoEstructura in copiaMasivaPermisos.Estructuras)
            {
                listaPermisos = UsuarioApplicationService.Instance.GetRolesNodoUsuario(copiaMasivaPermisos.Id, codigoEstructura);
                foreach (PermisosViewModel permiso in listaPermisos)
                {
                    copiaMasivaPermisos.Permisos.Add(permiso);
                }
            }

            //Elimino los Permisos Destino seleccionados, si éstos ya existen en los usuarios destino, y luego les asigno los nuevos Permisos
            using (var trx = BeginTransaction())
            {
                try
                {
                    IList<EstructuraFuncional> estructurasFuncionales = new List<EstructuraFuncional>();
                    IList<Rol> rolesDeEstructura = new List<Rol>();
                    //Busco y elimino todos los Roles-Nodo previos -si coinciden con los seleccionados-, por cada usuario en la listaUsuarios
                    foreach (string nombreUsuario in copiaMasivaPermisos.Usuarios)
                    {
                        Usuario usuario = GetUsuarioPorNombreUsuario(nombreUsuario);
                        Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
                        searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
                        searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Usuario.Id", usuario.Id));
                        //Recupero las Estructuras Funcionales Destino
                        foreach (string c in copiaMasivaPermisos.Estructuras)
                        {
                            EstructuraFuncional estructuraFuncional = GetEstructuraPorCodigo(c);
                            estructurasFuncionales.Add(estructuraFuncional);
                            mensajeListaEstructuras += estructuraFuncional.DescripcionEstructura + "\r\n";
                        }
                        //Recupero todos los Roles de esas Estructuras Funcionales Destino, y los agrego a una lista, para utilizar en el search de Permisos previos del Usuario Destino
                        foreach (EstructuraFuncional e in estructurasFuncionales)
                        {
                            foreach (Rol r in e.Roles)
                            {
                                rolesDeEstructura.Add(r);
                            }
                        }
                        searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
                        searchRolesNodoUsuario.AddExpression(Restrictions.In("Rol.Id", rolesDeEstructura.Select(x => x.Id).ToList()));
                        //Recupero en una lista los Id de RolNodoUsuario Destino con los criterios de search anteriores, para ser eliminados
                        IList<long> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario, new string[] { "Id" }).Select(x => x.Id).ToList();
                        DeleteAll<RolNodoUsuario>(rolesNodoUsuario, false);
                        mensajeListaUsuarios += nombreUsuario + "\r\n";
                    }
                    //Luego creo los nuevos Roles-Nodo, para cada usuario en la listaUsuarios
                    foreach (PermisosViewModel permiso in copiaMasivaPermisos.Permisos)
                    {
                        Rol rol = GetById<Rol>(permiso.IdRol);
                        if (permiso.NodosAsignados != null)
                        {
                            mensajeListaEstructurasRoles += rol.EstructuraFuncional.DescripcionEstructura + " - Rol: " + rol.Descripcion + "\r\n";
                            mensajeListaRoles += rol.Descripcion + ", ";
                            foreach (string codigoNodoAsignado in permiso.CodigoNodosAsignados)
                            {
                                NodoFuncional nodo = GetNodoPorCodigo(codigoNodoAsignado, permiso.CodigoEstructura);
                                foreach (string nombreUsuario in copiaMasivaPermisos.Usuarios)
                                {
                                    RolNodoUsuario rolNodoUsuario = new RolNodoUsuario();
                                    rolNodoUsuario.Rol = rol;
                                    rolNodoUsuario.NodoFuncional = nodo;
                                    rolNodoUsuario.Usuario = GetUsuarioPorNombreUsuario(nombreUsuario);
                                    rolNodoUsuario.UsuarioAlta = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
                                    rolNodoUsuario.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                                    Insert(rolNodoUsuario);
                                }
                            }
                        }
                    }
                    //Si todo se ejecutó correctamente, comiteo las transacciones
                    trx.Commit();
                    if (copiaMasivaPermisos.Estructuras.Count > 1)
                    {

                        return "Se han copiado los roles de las siguientes estructuras: \r\n" + mensajeListaEstructuras + "\r\nPara los usuarios seleccionados: \r\n" + mensajeListaUsuarios;
                    }
                    else
                    {
                        return "Se han copiado los roles de la estructura: \r\n" + mensajeListaEstructuras.TrimEnd('\n').TrimEnd('\r') + ": Roles: " + mensajeListaRoles + "\r\nPara los usuarios seleccionados: \r\n" + mensajeListaUsuarios;
                    }
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }


        public IList<RolUsuarioViewModel> GetRolesUsuario(string idRol, string estructura)
        {
            ICriteria criteria = Session().CreateCriteria<VWRolNodoUsuario>();
            if (!string.IsNullOrEmpty(estructura) && !estructura.Equals(ComboGenerico.ComboVacio))
            {
                criteria.Add(Restrictions.Eq("CodigoEstructuraFuncional", estructura));
            }
            if (!string.IsNullOrEmpty(idRol) && !idRol.Equals(ComboGenerico.ComboVacio))
            {
                criteria.Add(Restrictions.Eq("IdRol", long.Parse(idRol)));
            }

            IList<VWRolNodoUsuario> rolesNodoUsuario = criteria.List<VWRolNodoUsuario>();
            IList<RolUsuarioViewModel> usuariosPorRolViewModel = rolesNodoUsuario.GroupBy(x => new { x.IdUsuario }).Select(g => new RolUsuarioViewModel
            {
                IdUsuario = g.Key.IdUsuario,
                Estructura = g.First().DescripcionEstructuraFuncional,
                Rol = g.First().DescripcionRol,
                NombreUsuario = g.First().NombreUsuario,
                Nodos = g.Select(a => a.DescripcionNodo).ToList(),
            }).ToList();
            return usuariosPorRolViewModel;
        }

        /// <summary>
        /// Metodo que devuelve una lista de Actividades para un idRol dado.
        /// </summary>
        /// <param name="idRol"></param>
        /// <param name="estructura"></param>
        public IList<RolActividadViewModel> GetRolActividades(string idRol, string estructura)
        {
            Search searchRolActividades = new Search(typeof(RolActividad));
            searchRolActividades.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolActividades.AddAlias(new KeyValuePair<string, string>("Rol.EstructuraFuncional", "EstructuraFuncional"));
            if (!string.IsNullOrEmpty(estructura) && !estructura.Equals(ComboGenerico.ComboVacio))
            {
                searchRolActividades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            }
            if (!string.IsNullOrEmpty(idRol) && !idRol.Equals(ComboGenerico.ComboVacio))
            {
                searchRolActividades.AddExpression(Restrictions.Eq("Rol.Id", long.Parse(idRol)));
            }

            string[] propiedadesRolActividad = new string[]
            {
                "Actividad.Id",
                "Actividad.Codigo",
                "Actividad.Descripcion",
                "Rol.Id",
                "Rol.Codigo",
                "Rol.Descripcion",
                "Rol.EstructuraFuncional.Codigo",
                "Rol.EstructuraFuncional.DescripcionEstructura"
            };
            List<RolActividadViewModel> actividadesPorRolViewModel = new List<RolActividadViewModel>();
            actividadesPorRolViewModel.AddRange(GetByCriteria<RolActividad>(searchRolActividades, propiedadesRolActividad).Adapt<IList<RolActividadViewModel>>().OrderBy(x => x.CodigoActividad));
            return actividadesPorRolViewModel;
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
        public IList<UsuariosPorRolPaginado> GetUsuariosPorRolPaginate(string draw, string start, string length, string search, string sortColumn, string sortColumnDir, string idRol, string estructura)
        {
            ICriteria criteriaPaginado = Session().CreateCriteria<VWRolNodoUsuario>();
            if (!string.IsNullOrEmpty(estructura) && !estructura.Equals(ComboGenerico.ComboVacio))
            {
                criteriaPaginado.Add(Restrictions.Eq("CodigoEstructuraFuncional", estructura));
            }
            if (!string.IsNullOrEmpty(idRol) && !idRol.Equals(ComboGenerico.ComboVacio))
            {
                criteriaPaginado.Add(Restrictions.Eq("IdRol", long.Parse(idRol)));
            }


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            //Si el search no es vacio significa que debemos filtrar por los campos que mostramos en la lista
            if (!string.IsNullOrEmpty(search))
            {
                string searchLike = "%" + search + "%";
                criteriaPaginado
                    .Add(new Disjunction()
                        .Add(Restrictions.InsensitiveLike("NombreUsuario", searchLike))
                        .Add(Restrictions.InsensitiveLike("DescripcionRol", searchLike)));
                //.Add(Restrictions.InsensitiveLike("Rol", searchLike)));

            }


            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                string propertySortName = "";
                switch (sortColumn)
                {
                    case "Rol":
                        propertySortName = "DescripcionRol";
                        break;
                    case "Usuario":
                        propertySortName = "NombreUsuario";
                        break;
                    default:
                        propertySortName = sortColumn;
                        break;
                }
                if (sortColumnDir == "asc")
                {
                    criteriaPaginado.AddOrder(Order.Asc(propertySortName));
                }
                else
                {
                    criteriaPaginado.AddOrder(Order.Desc(propertySortName));
                }
            }

            ICriteria criteriaCount = (ICriteria)criteriaPaginado.Clone();
            recordsTotal = criteriaCount.SetProjection(Projections.RowCount()).UniqueResult<int>();

            IList<VWRolNodoUsuario> rolesNodoUsuario = criteriaPaginado.SetFirstResult(skip).SetMaxResults(pageSize).List<VWRolNodoUsuario>();
            IList<UsuariosPorRolPaginado> usuariosPorRolViewModel = rolesNodoUsuario.Adapt<IList<UsuariosPorRolPaginado>>();

            if (usuariosPorRolViewModel.Count > 0)
                usuariosPorRolViewModel.First().RecordsTotal = recordsTotal;
            return usuariosPorRolViewModel;
        }




    }

}
