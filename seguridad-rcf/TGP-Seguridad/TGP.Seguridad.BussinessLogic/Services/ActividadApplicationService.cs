
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
    public class ActividadApplicationService : SeguridadApplicationService
    {
        private static ActividadApplicationService instance;

        //Singleton
        public new static ActividadApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ActividadApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que obtiene el listado de actividades segun un codigo de estructura
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public IList<ListadoActividadViewModel> GetActividades(string codigoEstructura)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura"
            };
            Search searchActividades = new Search(typeof(Actividad));

            if(codigoEstructura != ComboGenerico.ComboVacio)
                searchActividades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));

            IList<Actividad> actividades = GetByCriteria<Actividad>(searchActividades, propiedades);
            return actividades.Adapt<List<ListadoActividadViewModel>>();
        }

        /// <summary>
        /// Metodo que verifica si el codigo de la actividad se encuentra en ambiente destino
        /// </summary>
        /// <param name="actividad"></param>
        public string PrevGuardarNuevaActividad(ActividadDTO actividad)
        {
            //if (ExisteCodigoElemento<Actividad>(actividad.Codigo))
            //    throw new Exception("Ya existe una actividad con el mismo codigo.");

            //IList<ElementoComparar> actividadesEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ACTIVIDAD, actividad.EstructuraFuncional);
            //if (actividadesEnDestino.Any(x => x.Codigo.ToLower() == actividad.Codigo.ToLower()))
            //    return "Existe un elemento con el mismo código en el ambiente destino. ¿Desea generarlo de todas formas?";

            return string.Empty;
        }
        
        /// <summary>
        /// Metodo que guarda una nueva actividad
        /// </summary>
        /// <param name="actividad"></param>
        public void GuardarNuevaActividad(ActividadDTO actividad)
        {
            //if (ExisteCodigoElemento<Actividad>(actividad.Codigo))
            //    throw new Exception("Ya existe una actividad con el mismo codigo.");

            Actividad actividadGuardar = new Actividad();

            //IList<ElementoComparar> actividadesEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ACTIVIDAD, actividad.EstructuraFuncional);
            //if (actividadesEnDestino.Any(x => x.Codigo.ToLower() == actividad.Codigo.ToLower()))
            //{
            //    //copio el ID de la actividad destino para evitar IDs desfasados entre los ambientes
            //    long idActividadDestino = actividadesEnDestino.Where(x => x.Codigo.ToLower() == actividad.Codigo.ToLower()).FirstOrDefault().Id;
            //    actividadGuardar.Id = idActividadDestino;
            //}
            //else
            //{
            //    actividadGuardar.Id = GetIdSecuencia(typeof(Actividad));
            //}
            actividadGuardar.Id = GetIdSecuencia(typeof(Actividad));

            actividadGuardar.Codigo = actividad.Codigo;
            actividadGuardar.Descripcion = actividad.Descripcion;
            actividadGuardar.EstructuraFuncional = GetEstructuraPorCodigo(actividad.EstructuraFuncional);
            actividadGuardar.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
            actividadGuardar.UsuarioResponsable = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
            
            Insert(actividadGuardar);
        }

        /// <summary>
        /// Metodo que guarda una edicion de  actividad
        /// </summary>
        /// <param name="actividad"></param>
        public void GuardarEdicionActividad(ActividadDTO actividad)
        {
            Actividad actividadEditar = GetById<Actividad>(actividad.Id);
            if (actividadEditar.Version != actividad.Version)
                throw new Exception("La Actividad fue modificada por otro usuario.");

            if (actividadEditar.Codigo != actividad.Codigo)
            {
                if (ExisteCodigoElemento<Actividad>(actividad.Codigo))
                    throw new Exception("Ya existe una actividad con el mismo codigo.");

                IList<ElementoComparar> actividadesEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ACTIVIDAD, actividadEditar.EstructuraFuncional.Codigo);
                if (actividadesEnDestino.Any(x => x.Codigo.ToLower() == actividad.Codigo.ToLower() ))
                    throw new Exception("Existe un elemento con el mismo código en el ambiente destino. Corrobore los datos cargados.");

                actividadEditar.Codigo = actividad.Codigo;
            }

            if(actividadEditar.Descripcion != actividad.Descripcion)
            {
                if (ExisteDescripcionElemento<Actividad>(actividad.Descripcion))
                    throw new Exception("Ya existe una actividad con la misma descripcion.");
                
                actividadEditar.Descripcion = actividad.Descripcion;
            }

            Update(actividadEditar);
        }

        /// <summary>
        /// Metodo que elimina una actividad
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarActividad(long id, int version)
        {
            int versionBase = GetVersion<Actividad>(id);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("La actividad fue modificada por otro usuario.");

            
            //Search de roles actividad
            Search searchRolesActivdiad = new Search(typeof(RolActividad));
            searchRolesActivdiad.AddAlias(new KeyValuePair<string, string>("Actividad", "Actividad"));
            searchRolesActivdiad.AddExpression(Restrictions.Eq("Actividad.Id", id));

            //Search de menues actividad
            Search searchActividadMenu = new Search(typeof(ActividadMenuOpcion));
            searchActividadMenu.AddAlias(new KeyValuePair<string, string>("Actividad", "Actividad"));
            searchActividadMenu.AddExpression(Restrictions.Eq("Actividad.Id", id));

            //Si existen menues asociados a la actividad no permito borrar y elevo una excepcion
            IList<ActividadMenuOpcion> menuActividad = GetByCriteria<ActividadMenuOpcion>(searchActividadMenu, new string[] { "Actividad.Id","MenuOpcion.Id" });
            if (menuActividad.Count > 0)
                throw new Exception("La actividad se encuentra relacionada a un Menu, por favor verifique");

            IList<RolActividad> rolesActividad = GetByCriteria<RolActividad>(searchRolesActivdiad, new string[] { "IdActividad","IdRol" });
            using (var trx = BeginTransaction())
            {
                try
                {
                    //Si existe roles asociados a la actividada los borro
                    if(rolesActividad.Count > 0)
                        DeleteAll<RolActividad>(rolesActividad.Select(x=>x.GetId()).ToList(), false);

                    Delete<Actividad>(id);
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
        /// Metodo que obtiene la actividad a editar y la transforma en viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActividadDTO GetActividadEditar(long id)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Version",
                "Codigo",
                "Descripcion",
                "EstructuraFuncional.Codigo",
            };

            return GetById<Actividad>(id).Adapt<ActividadDTO>();
        }

    }

}
