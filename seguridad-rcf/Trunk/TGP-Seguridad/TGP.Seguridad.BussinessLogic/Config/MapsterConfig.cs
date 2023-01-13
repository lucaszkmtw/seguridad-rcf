using Mapster;
using System.Collections.Generic;
using System.Linq;
using TGP.Seguridad.BussinessLogic.APIRequests;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.BussinessLogic.Dto.Usuario;
using TGP.Seguridad.BussinessLogic.Generics;
using TGP.Seguridad.DataAccess.Mapping;

namespace TGP.Seguridad.BussinessLogic.Config
{
    public static class MapsterConfig
    {
        public static void Config()
        {

            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            #region Usuario
            //Nominado a ListadoNominadoViewModel
            TypeAdapterConfig<Nominado, ListadoNominadoViewModel>
                .NewConfig()
                .Map(dest => dest.DT_RowId, src => src.Id)
                .Map(dest => dest.DescripcionUsuario, src => src.NombreNominado + " " + src.Apellido)
                .Map(dest => dest.DescripcionTipoAutenticacion, src => (src.TipoAutenticacion != null) ? src.TipoAutenticacion.Descripcion.ToUpper() : "")
                .Map(dest => dest.SiActivo, src => (src.SiActivo) ? "<i class='material-icons'>done</i>" : "")
                .Map(dest => dest.SiBloqueado, src => (src.SiBloqueado) ? "<i class='material-icons'>done</i>" : "")
                .Map(dest => dest.CantidadIntentos, src => src.CantidadIntentos)
                .Ignore(dest => dest.Editar)
                .Ignore(dest => dest.Eliminar)
                .Ignore(dest => dest.Pass)
                .Ignore(dest => dest.RecordsTotal);

            //Acreedor a ListadoAcreedoresViewModel
            TypeAdapterConfig<Acreedor, ListadoAcreedoresViewModel>
                .NewConfig()
                .Map(dest => dest.DT_RowId, src => src.Id)
                .Map(dest => dest.SiActivo, src => (src.SiActivo) ? "<i class='material-icons'>done</i>" : "")
                .Map(dest => dest.SiBloqueado, src => (src.SiBloqueado) ? "<i class='material-icons'>done</i>" : "")
                .Ignore(dest => dest.Editar)
                .Ignore(dest => dest.Eliminar)
                .Ignore(dest => dest.Pass)
                .Ignore(dest => dest.RecordsTotal);

            //Usuario a UsuarioDashboardViewModel
            TypeAdapterConfig<Usuario, UsuarioDashboardViewModel>
                .NewConfig()
                .Map(dest => dest.CodigoTipoAutenticacion, src => src.TipoAutenticacion.Codigo)
                .Map(dest => dest.DescripcionTipoUsuario, src => src.TipoUsuario.Descripcion)
                .Map(dest => dest.CodigoTipoUsuario, src => src.TipoUsuario.Codigo);

            //Nominado a NominadoViewModel
            TypeAdapterConfig<Nominado, NominadoViewModel>
                .NewConfig()
                .Map(dest => dest.CodigoTipoUsuario, src => src.TipoUsuario.Codigo)
                .Map(dest => dest.CodigoTipoAutenticacion, src => src.TipoAutenticacion.Codigo)
                .Map(dest => dest.DescripcionTipoUsuario, src => src.TipoUsuario.Descripcion);

            //UsuarioSIGAF a UsuarioSigafDTO
            TypeAdapterConfig<UsuarioSIGAF, UsuarioSigafDTO>
                .NewConfig()
                .Map(dest => dest.NombreUsuario, src => src.CUser)
                .Map(dest => dest.SiBloqueado, src => src.SiBloqueado)
                .Map(dest => dest.FechaBaja, src => src.FechaBaja);

            //Acreedor a AcreedorViewModel
            TypeAdapterConfig<Acreedor, AcreedorViewModel>
                .NewConfig()
                .Map(dest => dest.CodigoTipoUsuario, src => src.TipoUsuario.Codigo)
                .Map(dest => dest.DescripcionTipoUsuario, src => src.TipoUsuario.Descripcion);

            //VWRolNodoUsuario a AsignacioUsuariosViewModel
            TypeAdapterConfig<VWRolNodoUsuario, AsignacioUsuariosViewModel>
                .NewConfig()
                .Ignore(dest => dest.RecordsTotal);
            
            #endregion

            #region Novedad
            //Novedad a NovedadDTO
            TypeAdapterConfig<Novedad, NovedadDTO>
                .NewConfig()
                .Map(dest => dest.EstructuraFuncionalCodigo, src => (src.EstructuraFuncional != null) ? src.EstructuraFuncional.Codigo : null)
                .Map(dest => dest.EstructuraFuncionalDescripcion, src => (src.EstructuraFuncional != null) ? src.EstructuraFuncional.DescripcionEstructura : null)
                .Map(dest => dest.Roles, src => src.ListaNovedadRolNodo != null ? src.ListaNovedadRolNodo.Select(x => x.Rol.Codigo).Distinct().ToList() : new List<string>())
                .Map(dest => dest.NodosFuncionales, src => src.ListaNovedadRolNodo != null ? src.ListaNovedadRolNodo.Select(x => x.NodoFuncional.Codigo).Distinct().ToList() : new List<string>());

            TypeAdapterConfig<RequestCrearNovedad, NovedadDTO>
                .NewConfig()
                .Map(dest => dest.Titulo, src => src.Titulo ?? "")
                .Map(dest => dest.EstructuraFuncionalCodigo, src => (src.EstructuraFuncional != null) ? src.EstructuraFuncional : null);
            #endregion

            #region Rol

            //Rol a RolDTO
            TypeAdapterConfig<Rol, RolDTO>
                .NewConfig()
                .Map(dest => dest.EsMultinodo, src => true, srcCond => srcCond.EsMultinodo == 1)
                .Map(dest => dest.EstructuraFuncional, src => src.EstructuraFuncional.Codigo)
                //TGPSEG-271, TGPSEG-272. si TipoNodoFuncional es null quiere decir que puede tener multiple tipo de nodo.
                .Map(dest => dest.TipoNodoFuncional, src => src.TipoNodoFuncional != null ? src.TipoNodoFuncional.Codigo : "TODOS")
                .Map(dest => dest.Actividades, src => src.Actividades != null ? src.Actividades.Select(x => x.Actividad.Codigo) : new List<string>())
                .Map(dest => dest.UsuarioAlta, src => src.UsuarioAlta.NombreUsuario);

            TypeAdapterConfig<VWRolNodoUsuario, UsuariosPorRolPaginado>
                .NewConfig()
                .Map(dest => dest.Rol, src => src.DescripcionRol)
                .Map(dest => dest.Nodo, src => src.DescripcionNodo);
            #endregion

            #region Actividades
            //Actividad a ListadoActividadViewModel
            TypeAdapterConfig<Actividad, ListadoActividadViewModel>
                .NewConfig()
                .Map(dest => dest.CodigoEstructura, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura);

            //Actividad a ActividadDTO
            TypeAdapterConfig<Actividad, ActividadDTO>
                .NewConfig()
                .Map(dest => dest.EstructuraFuncional, src => src.EstructuraFuncional.Codigo);
            #endregion

            #region Comparar
            //Rol a ElementoComparar
            TypeAdapterConfig<Rol, ElementoComparar>
                .NewConfig()
                .Map(dest => dest.CodigoEstructura, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura)
                .Ignore(dest => dest.Padre)
                .Ignore(dest => dest.Estado);

            //Actividad a ElementoComparar
            TypeAdapterConfig<Actividad, ElementoComparar>
                .NewConfig()
                .Map(dest => dest.CodigoEstructura, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura)
                .Ignore(dest => dest.Estado);

            //MenuOpcion a ElementoComparar
            TypeAdapterConfig<MenuOpcion, ElementoComparar>
                .NewConfig()
                .Map(dest => dest.CodigoEstructura, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura)
                .Map(dest => dest.Padre, src => src.MenuOpcionPadre != null ? src.MenuOpcionPadre.Descripcion : "")
                .Ignore(dest => dest.Estado);

            //NodoFuncional a ElementoComparar
            TypeAdapterConfig<NodoFuncional, ElementoComparar>
                .NewConfig()
                .Map(dest => dest.CodigoEstructura, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura)
                .Ignore(dest => dest.Estado)
                .Ignore(dest => dest.Padre);

            //EstructuraFuncional a ElementoComparar
            TypeAdapterConfig<EstructuraFuncional, ElementoComparar>
                .NewConfig()
                .Ignore(dest => dest.Estado)
                .Ignore(dest => dest.Padre);
            #endregion

            #region Menu Opcion
            //MenuOpcion a ListadoMenuOpcionViewModel
            TypeAdapterConfig<MenuOpcion, ListadoMenuOpcionViewModel>
                .NewConfig()
                .Map(dest => dest.CodigoEstructura, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura)
                .Map(dest => dest.MenuPadre, src => src.MenuOpcionPadre != null ? src.MenuOpcionPadre.Descripcion : null);

            //MenuOpcion a MenuOpcionDTO
            TypeAdapterConfig<MenuOpcion, MenuOpcionDTO>
                .NewConfig()
                .Map(dest => dest.EstructuraFuncional, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.ActividadAsociada, src => src.ActividadAsociada != null ? src.ActividadAsociada.Actividad.Codigo : "0")
                .Map(dest => dest.MenuOpcionPadre, src => src.MenuOpcionPadre != null ? src.MenuOpcionPadre.Codigo : "0");
            #endregion

            #region Nodos Funcionales
            //NodoFuncional a ListadoNodoFuncionalViewModel
            TypeAdapterConfig<NodoFuncional, ListadoNodoFuncionalViewModel>
                .NewConfig()
                .Map(dest => dest.EstructuraFuncional, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.NodoPadre, src => src.NodoFuncionalPadre != null ? src.NodoFuncionalPadre.Codigo : null)
                .Map(dest => dest.TipoNodoFuncional, src => src.TipoNodoFuncional.Codigo, srcCond => srcCond.TipoNodoFuncional != null)
                .Map(dest => dest.DescripcionNodoPadre, src => src.NodoFuncionalPadre != null ? src.NodoFuncionalPadre.Descripcion : "")
                .Map(dest => dest.DescripcionEstructura, src => src.EstructuraFuncional.DescripcionEstructura);

            //NodoFuncional a NodoFuncionalDTO
            TypeAdapterConfig<NodoFuncional, NodoFuncionalDTO>
                .NewConfig()
                .Map(dest => dest.EstructuraFuncional, src => src.EstructuraFuncional.Codigo)
                .Map(dest => dest.TipoNodoFuncional, src => src.TipoNodoFuncional.Codigo, srcCond => srcCond.TipoNodoFuncional != null)
                .Map(dest => dest.NodoPadre, src => src.NodoFuncionalPadre != null ? src.NodoFuncionalPadre.Codigo : null);
            #endregion

            #region Administradores Locales 
            TypeAdapterConfig<AdministradorLocal, AdministradorLocalDTO>
                .NewConfig()
                .Map(dest => dest.UsuarioAdmin, src => src.UsuarioAdmin)
                .Map(dest => dest.NodoFuncional, src => src.NodoFuncional)
                .Map(dest => dest.RolesDelegadosAdmins, src => src.RolesDelegadosAdmins);

            TypeAdapterConfig<AdministradorLocal, AdminLocalLivianoDTO>
                .NewConfig()
                .Map(dest => dest.Usuario, src => src.UsuarioAdmin.NombreUsuario)
                .Map(dest => dest.NodoFuncional, src => src.NodoFuncional.Descripcion)
                .Map(dest => dest.Estructura, src => src.NodoFuncional.EstructuraFuncional.DescripcionEstructura);

            TypeAdapterConfig<Nominado, ListadoAdministradoresLocalesViewModel>
                .NewConfig()
                .Map(dest => dest.DescripcionUsuario, src => src.NombreNominado + " " + src.Apellido)
                .Map(dest => dest.AdministradoresLocalesDTO, src => src.AdministradoresLocales)
                .Map(dest => dest.Estructuras, src => src.AdministradoresLocales.Select(a => a.NodoFuncional.EstructuraFuncional.Codigo).Distinct().ToList());
            #endregion

            #region EstructuraFuncional

            ////EstructuraFuncional a EstructuraFuncionalDTO
            //TypeAdapterConfig<EstructuraFuncional, EstructuraFuncionalDTO>
            //    .NewConfig()
            //    .Map(dest => dest.EstructuraFuncional, src => src.EstructuraFuncional.Codigo)
            //    .Map(dest => dest.NodoPadre, src => src.NodoFuncionalPadre != null ? src.NodoFuncionalPadre.Codigo : null);
            #endregion

            #region BServicio
            ////BSERVICIO
            //TypeAdapterConfig<BServicio, BServicioDTO>
            //    .NewConfig()
            //    .Map(dest => dest.AA_EJERVG, src => src.AA_EJERVG)
            //    .Map(dest => dest.Id, src => src.Id)
            //    .Map(dest => dest.XL_SERVICIO, src => src.XL_SERVICIO);
            #endregion

            #region API
            //Rol a RolDEstino
            TypeAdapterConfig<Rol, RolDestino>
                .NewConfig()
                .Map(dest => dest.IdEstructuraFuncional, src => src.EstructuraFuncional.Id)
                .Map(dest => dest.IdActividades, src => src.Actividades.Select(x => x.IdActividad).ToList<long>())
                .Map(dest => dest.IdTipoNodoFuncional, src => src.TipoNodoFuncional.Id, srcCond => srcCond.TipoNodoFuncional != null)
                .Map(dest => dest.NombreUsuario, src => src.UsuarioAlta.NombreUsuario);

            //Actividad a Actividad Destino
            TypeAdapterConfig<Actividad, ActividadDestino>
                .NewConfig()
                .Map(dest => dest.IdEstructuraFuncional, src => src.EstructuraFuncional.Id)
                .Map(dest => dest.NombreUsuario, src => src.UsuarioResponsable.NombreUsuario);

            //MenuOpcion a MenuDestino
            TypeAdapterConfig<MenuOpcion, MenuDestino>
                .NewConfig()
                .Map(dest => dest.IdEstructuraFuncional, src => src.EstructuraFuncional.Id)
                .Map(dest => dest.NombreUsuario, src => src.UsuarioResponable.NombreUsuario)
                .Map(dest => dest.IdActividad, src => src.ActividadAsociada != null ? (long?)src.ActividadAsociada.IdActividad : null)
                .Map(dest => dest.IdMenuOpcionPadre, src => src.MenuOpcionPadre != null ? (long?)src.MenuOpcionPadre.Id : null);

            //NodoFuncional a NodoDestino
            TypeAdapterConfig<NodoFuncional, NodoDestino>
                .NewConfig()
                .Map(dest => dest.IdEstructuraFuncional, src => src.EstructuraFuncional.Id)
                .Map(dest => dest.NombreUsuario, src => src.Usuario.NombreUsuario)
                .Map(dest => dest.IdTipoNodoFuncional, src => src.TipoNodoFuncional.Id, srcCond => srcCond.TipoNodoFuncional != null)
                .Map(dest => dest.IdNodoPadre, src => src.NodoFuncionalPadre != null ? (long?)src.NodoFuncionalPadre.Id : null);

            //EstructuraFuncional a EstructuraDestino
            TypeAdapterConfig<EstructuraFuncional, EstructuraDestino>
                .NewConfig()
                .Map(dest => dest.NombreUsuario, src => src.Usuario != null ? src.Usuario.NombreUsuario : "USERTGP");

            //Usuario a Nominado
            TypeAdapterConfig<Usuario, NominadoResponse>
              .NewConfig()
                .Map(dest => dest.IdUsuario, src => src.Id)
                .Map(dest => dest.NombreUsuario, src => src.NombreUsuario)
                .Map(dest => dest.DescripcionUsuario, src => src.Descripcion)
                .Map(dest => dest.MailUsuario, src => src.EMail)
                .Map(dest => dest.Avatar, src => src.Avatar)
                .Map(dest => dest.EsContadorDelegado, src => src.EsContadorDelegado)
                .Map(dest => dest.Nodos, src => src.RolesNodoUsuario);

            //Rol a RolResponse
            TypeAdapterConfig<Rol, RolResponse>
              .NewConfig()
                .Map(dest => dest.IdRol, src => src.Id)
                .Map(dest => dest.DescripcionRol, src => src.Descripcion)
                .Map(dest => dest.CodigoRol, src => src.Codigo);

            //NodoFuncional a NodoResponse
            TypeAdapterConfig<NodoFuncional, NodoResponse>
              .NewConfig()
                .Map(dest => dest.IdNodo, src => src.Id)
                .Map(dest => dest.DescripcionNodo, src => src.Descripcion)
                .Map(dest => dest.CodigoNodo, src => src.Codigo);
            #endregion

            #region Otros
            //NodoFuncional a ComboGenerico
            TypeAdapterConfig<NodoFuncional, ComboGenerico>
                .NewConfig()
                .Map(dest => dest.Descripcion, src => src.Descripcion)
                .Map(dest => dest.Codigo, src => src.Codigo)
                .Map(dest => dest.Id, src => src.Id);
            //RolNodoUsuario a ComboGenerico
            TypeAdapterConfig<RolNodoUsuario, ComboGenerico>
                .NewConfig()
                .Map(dest => dest.Descripcion, src => src.Rol.Descripcion)
                .Map(dest => dest.Codigo, src => src.Rol.Codigo)
                .Map(dest => dest.Id, src => src.Rol.Id);

            //TipoNodoFuncional a ComboGenerico
            TypeAdapterConfig<TipoNodoFuncional, ComboGenerico>
                .NewConfig()
                .Map(dest => dest.Descripcion, src => src.Descripcion)
                .Map(dest => dest.Codigo, src => src.Codigo)
                .Map(dest => dest.Id, src => src.Id);

            //EstructuraFuncional a ComboGenerico
            TypeAdapterConfig<EstructuraFuncional, ComboGenerico>
                .NewConfig()
                .Map(dest => dest.Descripcion, src => src.DescripcionEstructura);

            //Nominado a UsuarioAutoComplete
            TypeAdapterConfig<Nominado, UsuarioAutoComplete>
                .NewConfig()
                .Map(dest => dest.NombreApellidoRazonSocial, src => src.NombreNominado + " " + src.Apellido)
                .Map(dest => dest.DescripcionAsignacionMasiva, src => src.NombreUsuario + " - " + src.NombreNominado + " " + src.Apellido);

            //Nominado a UsuarioAutoCompleteResponse
            TypeAdapterConfig<Nominado, NominadoAutocompleteResponse>
                .NewConfig()
                 .Map(dest => dest.IdUsuario, src => src.Id)
                //.Map(dest => dest.DniUsuario, src => src.NumeroDni)
                .Map(dest => dest.DescripcionUsuario, src => src.GetDescripcionUsuario());

            //Acreedor a UsuarioAutoComplete
            TypeAdapterConfig<Acreedor, UsuarioAutoComplete>
                .NewConfig()
                .Map(dest => dest.NombreApellidoRazonSocial, src => src.NumeroCuit + " - " + src.RazonSocial)
                .Map(dest => dest.DescripcionAsignacionMasiva, src => src.NombreUsuario + " - " + src.RazonSocial);

            //RolActividad a RolActividadViewModel
            TypeAdapterConfig<RolActividad, RolActividadViewModel>
                .NewConfig()
                .Map(dest => dest.IdActividad, src => src.Actividad.Id)
                .Map(dest => dest.CodigoActividad, src => src.Actividad.Codigo)
                .Map(dest => dest.DescripcionActividad, src => src.Actividad.Descripcion)
                .Map(dest => dest.IdRol, src => src.Rol.Id)
                .Map(dest => dest.CodigoRol, src => src.Rol.Codigo)
                .Map(dest => dest.DescripcionRol, src => src.Rol.Descripcion)
                .Map(dest => dest.CodigoEstructura, src => src.Rol.EstructuraFuncional.Codigo)
                .Map(dest => dest.DescripcionEstructura, src => src.Rol.EstructuraFuncional.DescripcionEstructura);

            #endregion

        }
    }
}
