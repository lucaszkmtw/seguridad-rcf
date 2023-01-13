using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.UI.WebControls;

namespace TGP.WSS
{
    /// <summary>
    /// Interface del servicio de seguridad
    /// </summary>
    [ServiceContract]
    public interface LibreriaSeguridad
    {

        [OperationContract]
        string Eco();
        [OperationContract]
        Models.Resultado.ResultadoDTO Favoritear(Models.Requerimiento.RequerimientoFavoritearDTO requerimiento, string token);
        [OperationContract]
        Models.Resultado.ResultadoObtenerFavoritosDTO ObtenerFavoritos(Models.Requerimiento.RequerimientoObtenerFavoritosDTO requerimiento, string token);

        [OperationContract]
        Models.Resultado.ResultadoObtenerUsuarioDTO ObtenerUsuarioNominado(Models.Requerimiento.RequerimientoObtenerUsuarioDTO requerimiento, string token);

        [OperationContract]
        Models.Resultado.ResultadoObtenerUsuariosNominadosPorRolDTO ObtenerUsuariosNominadosPorRol(Models.Requerimiento.RequerimientoObtenerUsuariosNominadosPorRolDTO requerimiento, string token);

        [OperationContract]
        ResultadoLogin Login(String usu, String pass, string codigo_estruc_func, string token);

        [OperationContract]
        ResultadoLogin ExternalLogin(String usu, string codigo_estruc_func, string token);

        [OperationContract]
        ResultadoChangePassword ChangePassword(String usu, String passActual, String passNueva, string token);

        [OperationContract]
        ResultadoSendPasswordFT SendPasswordFT(String usu, string token);

        [OperationContract]
        ResultadoResetPassword ResetPassword(String usu, string token);

        [OperationContract]
        ResultadoSetNewPasswordByActiveDirectory SetNewPasswordByActiveDirectory(String usu, String passNueva, string token);

        [OperationContract]
        void addAuditoria(string usuario, DateTime fecha, string codEstrutura, string codActividad, string observaciones, string token);

        [OperationContract]
        void RegistarAuditoriaConexion(byte[] avatar, string nombreUsuario, string aplicacion, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion, string token);

        [OperationContract]
        void RegistarAuditoriaDesconexion(byte[] avatar, string nombreUsuario, string aplicacion, Int64 usuarioID, string IPCliente, string browserCliente, string serverDescripcion, string token);

        [OperationContract]
        IList<UsuarioSTGP> UsuariosXEstructura(string codEstructura, string token);

        [OperationContract]
        Models.DTO.ResultadoGetAuditoriaDTO GetAuditoria(Models.DTO.SolicitudGetAuditoria request, string token);

    }


    [DataContract]
    public class NodoFuncionalWS
    {
        //solo codigo y descirpcion, el id no sirve pq en algunas aplicaciones
        //se aparea con la tabla del esidif tgp_cut_borganismos, y habria q obtener el vigente en esos casos
        String codigo;
        String descripcion;

        [DataMember]
        public String Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        [DataMember]
        public String Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        public NodoFuncionalWS(String codigo, String descripcion)
        {
            this.Codigo = codigo;
            this.Descripcion = descripcion;
        }
    }

    [DataContract]
    public class MenuWS
    {
        int? id;

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }
        string descripcion;

        [DataMember]
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        string url;

        [DataMember]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        string icono;

        [DataMember]
        public string Icono
        {
            get { return icono; }
            set { icono = value; }
        }

        int? idPadre;

        [DataMember]
        public int? IdPadre
        {
            get { return idPadre; }
            set { idPadre = value; }
        }

        int? orden;

        [DataMember]
        public int? Orden
        {
            get { return orden; }
            set { orden = value; }
        }

        public MenuWS(int? id, string descripcion, string url, int? idPadre, int? orden, string icono)
        {
            this.id = id;
            this.descripcion = descripcion;
            this.url = url;
            this.Icono = icono;
            this.idPadre = idPadre;
            this.orden = orden;
        }
    }

    [DataContract]
    public class PermisoUsuarioWS
    {
        [DataMember]
        public String CodActividad { get; set; }

        [DataMember]
        public bool Audita { get; set; }

        [DataMember]
        public String DescripcionRol { get; set; }

        [DataMember]
        public String CodigoRol { get; set; }

        [DataMember]
        public List<NodoFuncionalWS> NodosAutorizados { get; set; }

        public PermisoUsuarioWS(String codActividad, String descRol, string codigoRol)
        {//, short audita) {
            this.CodActividad = codActividad;
            this.DescripcionRol = descRol;
            this.CodigoRol = codigoRol;
            //this.Audita = audita==1;
            this.NodosAutorizados = new List<NodoFuncionalWS>();
        }
    }


    
    /// <summary>
    /// Clase que modela el resultado del login
    /// </summary>
    [DataContract]
    public class ResultadoLogin
    {
        Boolean valido = false;
        List<PermisoUsuarioWS> permisos = new List<PermisoUsuarioWS>();
        String msj;
        String exception;
        List<MenuWS> menues = new List<MenuWS>();
        String msjActiveDirectory;
        int diasCambioClave;
        String codTipoAutenticacion;
        String cuitNominado;
        String nombreNominado;
        String apellidoNominado;
        String razonSocialAcreedor; 

        [DataMember]
        public Boolean Valido
        {
            get { return valido; }
            set { valido = value; }
        }

        [DataMember]
        public String Msj
        {
            get { return msj; }
            set { msj = value; }
        }

        [DataMember]
        public List<PermisoUsuarioWS> Permisos
        {
            get { return permisos; }
            set { permisos = value; }
        }

        [DataMember]
        public List<MenuWS> Menues
        {
            get { return menues; }
            set { menues = value; }
        }

        [DataMember]
        public Byte[] Avatar { get; set; }

        [DataMember]
        public Int64 UsuarioId { get; set; }

        [DataMember]
        public String Exception
        {
            get { return exception; }
            set { exception = value; }
        }                  

        [DataMember]
        public String MsjActiveDirectory
        {
            get { return msjActiveDirectory; }
            set { msjActiveDirectory = value; }
        }

        [DataMember]
        public int DiasCambioClave
        {
            get { return diasCambioClave; }
            set { diasCambioClave = value; }
        }
           
        [DataMember]
        public String CodTipoAutenticacion
        {
            get { return codTipoAutenticacion; }
            set { codTipoAutenticacion = value; }
        }

        [DataMember]
        public String CuitNominado
        {
            get { return cuitNominado; }
            set { cuitNominado = value; }
        }

        [DataMember]
        public String NombreNominado
        {
            get { return nombreNominado; }
            set { nombreNominado = value; }
        }

        [DataMember]
        public String ApellidoNominado
        {
            get { return apellidoNominado; }
            set { apellidoNominado = value; }
        }

        [DataMember]
        public String RazonSocialAcreedor
        {
            get { return razonSocialAcreedor; }
            set { razonSocialAcreedor = value; }
        }
    }

    /// <summary>
    /// Clase que modela el resultado de la Password
    /// </summary>
    [DataContract]
    public class ResultadoChangePassword
    {
        Boolean valido = false;
        String msj;
        String exception;

        [DataMember]
        public Boolean Valido
        {
            get { return valido; }
            set { valido = value; }
        }

        [DataMember]
        public String Msj
        {
            get { return msj; }
            set { msj = value; }
        }
        [DataMember]
        public String Exception
        {
            get { return exception; }
            set { exception = value; }
        }
    }

    /// <summary>
    /// Clase que modela el resultado de envio de la Passoword FT
    /// </summary>
    [DataContract]
    public class ResultadoSendPasswordFT
    {
        Boolean valido = false;
        String msj;
        String exception;

        [DataMember]
        public Boolean Valido
        {
            get { return valido; }
            set { valido = value; }
        }

        [DataMember]
        public String Msj
        {
            get { return msj; }
            set { msj = value; }
        }
        [DataMember]
        public String Exception
        {
            get { return exception; }
            set { exception = value; }
        }
    }

    /// <summary>
    /// Clase que modela el resultado de envio de la Passoword FT
    /// </summary>
    [DataContract]
    public class ResultadoResetPassword : ResultadoSendPasswordFT 
    {
    }

    /// <summary>
    /// Clase que modela el resultado de seteo de la clave de AD a un usuario
    /// </summary>
    [DataContract]
    public class ResultadoSetNewPasswordByActiveDirectory : ResultadoChangePassword 
    {

    }


    [DataContract]
    public class UsuarioSTGP
    {
        long id;
        String username;
        String descripcion;
        String mail;

        [DataMember]
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public String Username
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public String Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }         

        [DataMember]
        public String Mail
        {
            get { return mail; }
            set { mail = value; }
        }
    }
}
