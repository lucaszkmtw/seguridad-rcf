

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ListadoAcreedoresViewModel
    {
        public long DT_RowId { get; set; }
        public string NombreUsuario { get; set; }
        public string RazonSocial { get; set; }
        public string EMail { get; set; }
        public string SiActivo { get; set; }
        public string SiBloqueado { get; set; }
        public int Version { get; set; }
        public long RecordsTotal { get; set; }
        public string Editar
        {
            get
            {
                return "<a href='UsuarioDashboard/" + this.DT_RowId +"?tipoUsuario="+UsuarioDTO.USUARIOACREEDOR+"' data-original-title='Editar' data-toggle='tooltip' href='#' class='acciones'> <i class='material-icons'>mode_edit</i></a>";
            }
        }
        public string Eliminar
        {
            get
            {
                return "<a class='acciones' data-original-title='Editar' data-toggle='tooltip' href='#' onclick=\"eliminarUsuario('" + this.NombreUsuario + "'," + this.DT_RowId + "," + this.Version + ")\"><i class='material-icons'>delete</i></a>";
            }
        }
        public string Pass
        {
            get
            {
                return (this.SiActivo != "") ? "<a href='#' title='Enviar contraseña via mail al Usuario' class='badge element-bg-color-orange linkSubmit' data-toggle='modal' onclick=\"EnviarMail('" + this.NombreUsuario + "', '" + this.EMail + "', " + this.DT_RowId + ", 'ListadoAcreedores'," + this.Version + ")\"><i class='material-icons'>mail</i></a>" : "";
            }
        }
        
    }
}
