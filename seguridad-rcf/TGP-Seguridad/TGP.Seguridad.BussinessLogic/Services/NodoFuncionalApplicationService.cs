
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
    public class NodoFuncionalApplicationService : SeguridadApplicationService
    {
        private static NodoFuncionalApplicationService instance;

        //Singleton
        public static new NodoFuncionalApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NodoFuncionalApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que obtiene el listado de nodos segun un codigo de estructura
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public IList<ListadoNodoFuncionalViewModel> GetNodos(string codigoEstructura)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "SiDescentralizado",
                "EstructuraFuncional.DescripcionEstructura",
                "NodoFuncionalPadre.Descripcion",
                "TipoNodoFuncional.Codigo"
            };
            Search searchNodos = new Search(typeof(NodoFuncional));

            if (codigoEstructura != ComboGenerico.ComboVacio)
                searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));

            IList<NodoFuncional> nodos = GetByCriteria<NodoFuncional>(searchNodos, propiedades);
            return nodos.Adapt<List<ListadoNodoFuncionalViewModel>>();
        }


        /// <summary>
        /// Metodo que verifica si el codigo del nodo funcional se encuentra en ambiente destino
        /// </summary>
        /// <param name="nodo"></param>
        public string PrevGuardarNuevoNodo(NodoFuncionalDTO nodo)
        {
            //Chequeamos si existe un nodo con el mismo codigo en la misma estructura en el ambiente origen
            //Search searchNodoExistente = new Search(typeof(NodoFuncional));
            //searchNodoExistente.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            //searchNodoExistente.AddExpression(Expression.Eq("Codigo", nodo.Codigo));
            //searchNodoExistente.AddExpression(Expression.Eq("EstructuraFuncional.Codigo", nodo.EstructuraFuncional));

            //IList<ElementoComparar> nodoFuncionalEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.NODO, nodo.EstructuraFuncional);

            ////si no existe en ambiente origen pero si en el ambiente destino
            //if (GetCountByCriteria<NodoFuncional>(searchNodoExistente) == 0 && nodoFuncionalEnDestino.Any(x => x.Codigo.ToLower() == nodo.Codigo.ToLower()))
            //    return "Existe un elemento con el mismo código en el ambiente destino. ¿Desea generarlo de todas formas?";

            return string.Empty;
        }


        /// <summary>
        /// Metodo que guarda un nuevo nodo
        /// </summary>
        /// <param name="nodo"></param>
        public string GuardarNuevoNodo(NodoFuncionalDTO nodo)
        {
            //Chequeamos si existe un nodo con el mismo codigo en la misma estructura
            //Search searchNodoExistente = new Search(typeof(NodoFuncional));
            //searchNodoExistente.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            //searchNodoExistente.AddExpression(Expression.Eq("Codigo", nodo.Codigo));
            //searchNodoExistente.AddExpression(Expression.Eq("EstructuraFuncional.Codigo", nodo.EstructuraFuncional));

            ////Si es así, devolvemos un string
            //if (GetCountByCriteria<NodoFuncional>(searchNodoExistente) > 0)
            //    return "Ya existe un nodo funcional con el mismo codigo para " + GetEstructuraPorCodigo(nodo.EstructuraFuncional).DescripcionEstructura;
            

            //IList<ElementoComparar> nodoFuncionalEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.NODO, nodo.EstructuraFuncional);
            //if (nodoFuncionalEnDestino.Any(x => x.Codigo.ToLower() == nodo.Codigo.ToLower()))
            //{
            //    //copio el ID del nodo funcional destino para evitar IDs desfasados entre los ambientes
            //    long idNodoFuncionalDestino = nodoFuncionalEnDestino.Where(x => x.Codigo.ToLower() == nodo.Codigo.ToLower()).FirstOrDefault().Id;
            //    nodoGuardar.Id = idNodoFuncionalDestino;
            //}
            //else
            //{
            //}
            NodoFuncional nodoGuardar = new NodoFuncional();

            nodoGuardar.Id = GetIdSecuencia(typeof(NodoFuncional));

            //Creo el nodo segun el view model
            nodoGuardar.Codigo = nodo.Codigo;
            nodoGuardar.Descripcion = nodo.Descripcion;
            nodoGuardar.EstructuraFuncional = GetEstructuraPorCodigo(nodo.EstructuraFuncional);
            nodoGuardar.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
            nodoGuardar.Usuario = GetUsuarioPorNombreUsuario(HttpContext.Current.Session["usuario"].ToString());
            nodoGuardar.SiDescentralizado = nodo.SiDescentralizado;
            nodoGuardar.TipoNodoFuncional = GetTipoNodoFuncionalPorCodigo(nodo.TipoNodoFuncional);
            nodoGuardar.NodoFuncionalPadre = nodo.NodoPadre != "-1" ? GetNodoPorCodigo(nodo.NodoPadre, nodo.EstructuraFuncional) : null;
            
            Insert(nodoGuardar);
            return string.Empty;
        }

        /// <summary>
        /// Metodo que guarda una edicion de un nodo
        /// </summary>
        /// <param name="actividad"></param>
        public string GuardarEdicionNodo(NodoFuncionalDTO nodo)
        {
            //Chequeamos si existe un nodo con el mismo codigo en la misma estructura
            Search searchNodoExistente = new Search(typeof(NodoFuncional));
            searchNodoExistente.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchNodoExistente.AddExpression(Expression.Eq("Codigo", nodo.Codigo));
            searchNodoExistente.AddExpression(Expression.Eq("EstructuraFuncional.Codigo", nodo.EstructuraFuncional));



            //Chequeamos la version del nodo
            NodoFuncional nodoEditar = GetById<NodoFuncional>(nodo.Id);
            if (nodoEditar.Version != nodo.Version)
                throw new Exception("El Nodo fue modificado por otro usuario.");

            //Si existe en la misma estructura un nodo con el mismo codigo que no sea el que estoy editando
            //o existe un nodo con el mismo codigo en la nueva estructura
            if (GetCountByCriteria<NodoFuncional>(searchNodoExistente) > 0 &&
                ((nodo.Codigo != nodoEditar.Codigo && nodo.EstructuraFuncional == nodoEditar.EstructuraFuncional.Codigo) || nodo.EstructuraFuncional != nodoEditar.EstructuraFuncional.Codigo))
                return "Ya existe un nodo funcional con el mismo codigo para " + GetEstructuraPorCodigo(nodo.EstructuraFuncional).DescripcionEstructura;

            if (nodo.Codigo != nodoEditar.Codigo)
            {
                IList<ElementoComparar> nodoFuncionalEnDestino = ComparadorApplicationService.Instance.GetElementosDestino(ComparadorGenerico.NODO, nodo.EstructuraFuncional);
                if (nodoFuncionalEnDestino.Any(x => x.Codigo.ToLower() == nodo.Codigo.ToLower()))
                    throw new Exception("Existe un elemento con el mismo código en el ambiente destino. Corrobore los datos cargados.");
            }

            //Pasamos las propiedades
            nodoEditar.Codigo = nodo.Codigo;
            nodoEditar.SiDescentralizado = nodo.SiDescentralizado;
            nodoEditar.Descripcion = nodo.Descripcion;
            //Si se cambio la estructura traemos la nueva
            if (nodo.EstructuraFuncional != nodoEditar.EstructuraFuncional.Codigo)
                nodoEditar.EstructuraFuncional = GetEstructuraPorCodigo(nodo.EstructuraFuncional);

            //Si se cambio la tipo de nodo traemos el nuevo
            if (nodoEditar.TipoNodoFuncional == null || nodo.TipoNodoFuncional != nodoEditar.TipoNodoFuncional.Codigo)
                nodoEditar.TipoNodoFuncional = GetTipoNodoFuncionalPorCodigo(nodo.TipoNodoFuncional);

            //Si el nodo padre es sin padre ponemos null
            if (nodo.NodoPadre == "-1")
                nodoEditar.NodoFuncionalPadre = null;
            //Si el nodo a a editar no tiene padre o es diferente del que elegimos
            else if (nodoEditar.NodoFuncionalPadre == null || nodoEditar.NodoFuncionalPadre.Codigo != nodo.NodoPadre)
            {
                nodoEditar.NodoFuncionalPadre = GetNodoPorCodigo(nodo.NodoPadre, nodo.EstructuraFuncional);
            }

            Update(nodoEditar);
            return string.Empty;
        }

        /// <summary>
        /// Metodo que elimina un nodo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarNodo(long id, int version)
        {
            //NodoFuncional nodo = GetById<NodoFuncional>(id, new string[] { "Id"});
            int versionBase = GetVersion<NodoFuncional>(id);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("El menú fue modificado por otro usuario.");

            using (var trx = BeginTransaction())
            {
                try
                {
                    Search searchRolNodosUsuario = new Search(typeof(RolNodoUsuario));
                    searchRolNodosUsuario.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
                    searchRolNodosUsuario.AddExpression(Restrictions.Eq("NodoFuncional.Id", id));
                    IList<RolNodoUsuario> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolNodosUsuario, new string[] { "Id" });
                    if (rolesNodoUsuario.Count > 0)
                        DeleteAll<RolNodoUsuario>(rolesNodoUsuario.Select(x => x.Id).ToList(), false);

                    Delete<NodoFuncional>(id);
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
        /// Metodo que obtiene el nodo a editar y la transforma en viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NodoFuncionalDTO GetNodoEditar(long id)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Version",
                "Codigo",
                "Descripcion",
                "SiDescentralizado",
                "EstructuraFuncional.Codigo",
                "NodoFuncionalPadre.Codigo",
                "TipoNodoFuncional"
            };

            return GetById<NodoFuncional>(id).Adapt<NodoFuncionalDTO>();
        }

    }

}
