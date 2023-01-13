
using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGP.Seguridad.BussinessLogic.Generics;
using TGP.Seguridad.BussinessLogic.Helpers;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Helpers;
using TGP.Seguridad.DataAccess.Mapping;
using HelperService = TGP.Seguridad.DataAccess.Helpers.HelperService;

namespace TGP.Seguridad.BussinessLogic
{
    public class MenuOpcionApplicationService : SeguridadApplicationService
    {
        private static MenuOpcionApplicationService instance;

        //Singleton
        public new static MenuOpcionApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MenuOpcionApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que obtiene el listado de actividades segun un codigo de estructura
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public IList<ListadoMenuOpcionViewModel> GetMenues(string codigoEstructura)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "NumeroOrden",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura",
                "MenuOpcionPadre.Descripcion"
            };
            Search searchMenues = new Search(typeof(MenuOpcion));

            if (codigoEstructura != ComboGenerico.ComboVacio)
                searchMenues.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));

            IList<MenuOpcion> menues = GetByCriteria<MenuOpcion>(searchMenues, propiedades);
            return menues.Adapt<List<ListadoMenuOpcionViewModel>>();
        }

        /// <summary>
        /// Metodo que obtiene el nuevo codigo de menu con el que se va a guardar
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public long GetCodigoNuevoMenu(string codigoEstructura)
        {
            //Busco por estructura cual es el maximo codigo de menu
            ICriteria criteriaGetCodigo = Session().CreateCriteria<MenuOpcion>();
            criteriaGetCodigo
                .CreateAlias("EstructuraFuncional", "EstructuraFuncional")
                .Add(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura))
                .SetProjection(Projections.Max(Projections.Cast(NHibernateUtil.Int64, Projections.Property("Codigo"))));
            long? nuevoCodigo = criteriaGetCodigo.UniqueResult<long?>();
            //Si el codigo no es nulo, retorno el codigo+1
            if (nuevoCodigo != null)
                return (long)nuevoCodigo + 1;
            //Este else es por si elegimos una estructura que aun no tiene menues
            else
            {
                IQuery queryGetMaxCodigoAll = Session().CreateSQLQuery("select max(TO_NUMBER(m.c_codigo)) from seg_menu_opcion m");
                var codigoAll = queryGetMaxCodigoAll.UniqueResult();
                return ((Convert.ToInt64(codigoAll) / 100) + 1) * 100;
            }
        }

        /// <summary>
        /// Metodo que verifica si el codigo del menu se encuentra en ambiente destino
        /// </summary>
        /// <param name="menu"></param>
        public string PrevGuardarNuevoMenu(MenuOpcionDTO menu)
        {
            

            return string.Empty;
        }

        /// <summary>
        /// Metodo que guarda una nueva actividad
        /// </summary>
        /// <param name="actividad"></param>
        public void GuardarNuevoMenu(MenuOpcionDTO menu)
        {

            MenuOpcion menuGuardar = new MenuOpcion();

            menuGuardar.Id = GetIdSecuencia(typeof(Actividad));

            //Creo el menu segun el view model
            menuGuardar.Codigo = menu.Codigo;
            menuGuardar.Descripcion = menu.Descripcion;
            menuGuardar.EstructuraFuncional = GetEstructuraPorCodigo(menu.EstructuraFuncional);
            menuGuardar.UsuarioResponable = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
            menuGuardar.Icono = menu.Icono;
            menuGuardar.NumeroOrden = menu.NumeroOrden;
            menuGuardar.Url = menu.Url;
            menuGuardar.MenuOpcionPadre = menu.MenuOpcionPadre != null ? GetMenuPorCodigo(menu.MenuOpcionPadre, menu.EstructuraFuncional) : null;
            
            //Si el view model tiene una actividad asociada 
            if (menu.ActividadAsociada != null && menu.ActividadAsociada != ComboGenerico.ComboVacio)
            {
                using (var trx = BeginTransaction())
                {
                    try
                    {
                        Insert(menuGuardar);
                        Actividad a = GetActividadPorCodigo(menu.ActividadAsociada, menu.EstructuraFuncional);
                        ActividadMenuOpcion ActividadAsociadaMenu = new ActividadMenuOpcion()
                        {
                            IdActividad = a.Id,
                            UsuarioAlta = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString()),
                            FechaUltimaOperacion = HelperService.Instance.GetDateToday(),
                            IdMenuOpcion = menuGuardar.Id
                        };
                        menuGuardar.ActividadAsociada = ActividadAsociadaMenu;
                        trx.Commit();
                    }
                    catch (Exception)
                    {
                        trx.Rollback();
                        throw;
                    }
                }
            }
            else
                Insert(menuGuardar);
        }

        /// <summary>
        /// Metodo que guarda una edicion de un menu
        /// </summary>
        /// <param name="actividad"></param>
        public void GuardarEdicionMenu(MenuOpcionDTO menu)
        {

            //Chequeamos la version del menu
            MenuOpcion menuEditar = GetById<MenuOpcion>(menu.Id);
            if (menuEditar.Version != menu.Version)
                throw new Exception("El Menú fue modificado por otro usuario.");

            //Si ya existe un menu con el mismo codigo elevamos una excepcion
            if (menu.EstructuraFuncional != menuEditar.EstructuraFuncional.Codigo) {
                if (ExisteCodigoElemento<MenuOpcion>(menu.Codigo))
                    throw new Exception("Ya existe un menú con el mismo codigo.");

                IList<ElementoComparar> menuOpcionEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.MENUOPCION, menu.EstructuraFuncional);
                if (menuOpcionEnDestino.Any(x => x.Codigo.ToLower() == menu.Codigo.ToLower()))
                    throw new Exception("Existe un elemento con el mismo código en el ambiente destino. Corrobore los datos cargados.");
            }
            
            //Pasamos las propiedades
            menuEditar.Icono = menu.Icono;
            menuEditar.Codigo = menu.Codigo;
            menuEditar.Url = menu.Url;
            menuEditar.NumeroOrden = menu.NumeroOrden;
            menuEditar.Descripcion = menu.Descripcion;
            //Si se cambio la estructura traemos la nueva
            if (menu.EstructuraFuncional != menuEditar.EstructuraFuncional.Codigo)
                menuEditar.EstructuraFuncional = GetEstructuraPorCodigo(menu.EstructuraFuncional);

            using (var trx = BeginTransaction())
            {
                try
                {
                    //Si se eligio sin padre ponemos null
                    if (menu.MenuOpcionPadre == ComboGenerico.ComboVacio)
                        menuEditar.MenuOpcionPadre = null;


                    else
                    {
                        //Si originalmente no tenia padre o elegimos un padre distinto vamos a buscarlo
                        if (menuEditar.MenuOpcionPadre == null || menu.MenuOpcionPadre != menuEditar.MenuOpcionPadre.Codigo)
                            menuEditar.MenuOpcionPadre = GetMenuPorCodigo(menu.MenuOpcionPadre, menu.EstructuraFuncional);
                    }

                    //Si elegimos sin actividad y antes tenia alguna asociada la eliminamos y la nuleamos
                    if (menu.ActividadAsociada == ComboGenerico.ComboVacio && menuEditar.ActividadAsociada != null)
                    {
                        Delete<ActividadMenuOpcion>(menuEditar.ActividadAsociada.GetId());
                        menuEditar.ActividadAsociada = null;
                    }
                    //Si no tenia actividad asociada o elegimos una nueva actividad asociada
                    else if (menuEditar.ActividadAsociada == null || menu.ActividadAsociada != menuEditar.ActividadAsociada.Actividad.Codigo)
                    {
                        //Si antes tenia actividad asociada la borramos
                        if (menuEditar.ActividadAsociada != null)
                        {
                            dynamic idAct = menuEditar.ActividadAsociada.GetId();
                            menuEditar.ActividadAsociada = null;
                            Update(menuEditar);
                            Delete<ActividadMenuOpcion>(idAct);
                        }
                        //Si ahora si tiene actividad asociada y antes no
                        if(menu.ActividadAsociada != ComboGenerico.ComboVacio)
                        {
                            Actividad a = GetActividadPorCodigo(menu.ActividadAsociada, menu.EstructuraFuncional);
                            //Creamos una nueva actividad asociada
                            menuEditar.ActividadAsociada = new ActividadMenuOpcion
                            {
                                IdActividad = a.Id,
                                FechaUltimaOperacion = HelperService.Instance.GetDateToday(),
                                IdMenuOpcion = menuEditar.Id,
                                UsuarioAlta = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString())
                            };
                        }
                        
                    }
                    Update(menuEditar);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Metodo que elimina una actividad
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarMenu(long id, int version)
        {
            MenuOpcion menu = GetById<MenuOpcion>(id, new string[] { "Id", "ActividadAsociada" });
            int versionBase = GetVersion<MenuOpcion>(id);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("El menú fue modificado por otro usuario.");

            using (var trx = BeginTransaction())
            {
                try
                {
                    if (menu.ActividadAsociada != null)
                        Delete<ActividadMenuOpcion>(menu.ActividadAsociada.GetId());

                    Delete<MenuOpcion>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }


        }

        /// <summary>
        /// Metodo que obtiene el menu a editar y la transforma en viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MenuOpcionDTO GetMenuEditar(long id)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Version",
                "Codigo",
                "Descripcion",
                "NumeroOrden",
                "Url",
                "Icono",
                "ActividadAsociada.Actividad.Codigo",
                "EstructuraFuncional.Codigo",
                "MenuOpcionPadre.Codigo"
            };

            return GetById<MenuOpcion>(id).Adapt<MenuOpcionDTO>();
        }

    }

}
