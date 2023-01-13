
using Mapster;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Helpers;
using TGP.Seguridad.DataAccess.Mapping;

namespace TGP.Seguridad.BussinessLogic
{
    public class EstructuraApplicationService : SeguridadApplicationService
    {
        private static EstructuraApplicationService instance;

        //Singleton
        public static new EstructuraApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EstructuraApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que obtiene el listado de estructuras
        /// </summary>
        /// <returns></returns>
        public IList<EstructuraFuncionalDTO> GetEstructuras(bool incluirSeguridad)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Version",
                "DescripcionEstructura",
                "SiBloqueado"
            };
            IList<EstructuraFuncional> estructuras = GetAll<EstructuraFuncional>();
            if (!incluirSeguridad) {
                estructuras = estructuras.Where(e => e.Codigo != "SEG").ToList();
            }
            return estructuras.Adapt<IList<EstructuraFuncionalDTO>>().OrderBy(e => e.DescripcionEstructura).ToList();
        }


        /// <summary>
        /// Metodo que verifica si el codigo de la estructura funcional se encuentra en ambiente destino
        /// </summary>
        /// <param name="estructura"></param>
        public string PrevGuardarNuevaEstructura(EstructuraFuncionalDTO estructura)
        {
            //if (ExisteCodigoElemento<EstructuraFuncional>(estructura.Codigo))
            //    throw new Exception("Ya existe una estructura funcional con el mismo codigo.");

            //IList<ElementoComparar> estructuraEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ESTRUCTURA, estructura.Codigo);
            //if (estructuraEnDestino.Any(x => x.Codigo.ToLower() == estructura.Codigo.ToLower()))
            //    return "Existe un elemento con el mismo código en el ambiente destino. ¿Desea generarlo de todas formas?";

            return string.Empty;
        }

        /// <summary>
        /// Metodo que guarda un nuevo nodo
        /// </summary>
        /// <param name="estructura"></param>
        public void GuardarNuevaEstructura(EstructuraFuncionalDTO estructura)
        {
            EstructuraFuncional estructuraGuardar = new EstructuraFuncional();

            //IList<ElementoComparar> estructuraEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ESTRUCTURA, estructura.Codigo);
            //if (estructuraEnDestino.Any(x => x.Codigo.ToLower() == estructura.Codigo.ToLower()))
            //{
            //    //copio el ID de la estructura funcional destino para evitar IDs desfasados entre los ambientes
            //    long idEstructuraDestino = estructuraEnDestino.Where(x => x.Codigo.ToLower() == estructura.Codigo.ToLower()).FirstOrDefault().Id;
            //    estructuraGuardar.Id = idEstructuraDestino;
            //}
            //else
            //{
            //}
            estructuraGuardar.Id = GetIdSecuencia(typeof(EstructuraFuncional));

            //Creo la estructura segun el view model
            estructuraGuardar.Codigo = estructura.Codigo;
            estructuraGuardar.DescripcionEstructura = estructura.DescripcionEstructura;
            estructuraGuardar.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
            estructuraGuardar.Usuario = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
            estructuraGuardar.SiBloqueado = estructura.SiBloqueado;
            
            Insert(estructuraGuardar);
        }

        /// <summary>
        /// Metodo que guarda una edicion de una estructura
        /// </summary>
        /// <param name="actividad"></param>
        public void GuardarEdicionEstructura(EstructuraFuncionalDTO estructura)
        {
            EstructuraFuncional estructuraEditar = GetById<EstructuraFuncional>(estructura.Id);

            //Si el version de la estructura gaurdada no es el mismo que del view model elevo la excepcion 
            if (estructura.Version != estructuraEditar.Version)
                throw new Exception("La Estructura fue modificada por otro usuario");

            //if (estructuraEditar.Codigo != estructura.Codigo)
            //{
            //    IList<ElementoComparar> estructuraEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.ESTRUCTURA, estructura.Codigo);
            //    if (estructuraEnDestino.Any(x => x.Codigo.ToLower() == estructura.Codigo.ToLower()))
            //        throw new Exception("Existe un elemento con el mismo código en el ambiente destino. Corrobore los datos cargados.");
            //}

            estructuraEditar.Codigo = estructura.Codigo;
            estructuraEditar.DescripcionEstructura = estructura.DescripcionEstructura;
            estructuraEditar.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
            estructuraEditar.Usuario = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
            estructuraEditar.SiBloqueado = estructura.SiBloqueado;

            Update(estructuraEditar);
        }

        /// <summary>
        /// Metodo que elimina una estructura
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarEstructura(long id, int version)
        {
            int versionBase = GetVersion<EstructuraFuncional>(id);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("La Estructura fue modificado por otro usuario.");

            if (ValidaEliminacionEstructura(id))
                Delete<EstructuraFuncional>(id);
        }



        /// <summary>
        /// Metodo que obtiene la estructura a editar y la transforma en viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EstructuraFuncionalDTO GetEstructuraEditar(long id)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Version",
                "Codigo",
                "Descripcion",
                "SiBloqueado",
            };

            return GetById<EstructuraFuncional>(id).Adapt<EstructuraFuncionalDTO>();
        }


        /// <summary>
        /// Metodo que obtiene la estructura a partir de su codigo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EstructuraFuncionalDTO GetEstructuraPorCodigo(string codEstructura)
        {
            Search search = new Search(typeof(EstructuraFuncional));
            search.AddExpression(Restrictions.Eq("Codigo", codEstructura));
            EstructuraFuncional e = GetByCriteria<EstructuraFuncional>(search).FirstOrDefault();
            return e.Adapt<EstructuraFuncionalDTO>();
        }
    }

}
