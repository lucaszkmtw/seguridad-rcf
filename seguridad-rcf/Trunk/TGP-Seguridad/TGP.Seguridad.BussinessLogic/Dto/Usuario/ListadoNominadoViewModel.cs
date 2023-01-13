

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ListadoNominadoViewModel 
    {
        public long DT_RowId { get; set; }
        public string NombreUsuario { get; set; }
        public string DescripcionUsuario { get; set; }
        public string EMail { get; set; }
        public string SiActivo { get; set; }
        public string SiBloqueado { get; set; }
        public string DescripcionTipoAutenticacion { get; set; }
        public int Version { get; set; }
        public long RecordsTotal { get; set; }
        public int? CantidadIntentos { get; set; }
        public string Editar
        {
            get
            {
                return "<a href='UsuarioDashboard/" + this.DT_RowId + "?tipoUsuario=" + UsuarioDTO.USUARIONOMINADO + "' data-original-title='Editar' data-toggle='tooltip' href='#' class='acciones'> <i class='material-icons'>mode_edit</i></a>";
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
                return (this.SiActivo != "") ? "<a href='#' title='Enviar contraseña via mail al Usuario' class='badge element-bg-color-orange linkSubmit' data-toggle='modal' onclick=\"EnviarMail('" + this.NombreUsuario + "', '" + this.EMail + "', " + this.DT_RowId + ", 'ListadoNominados'," + this.Version + ", '" + this.DescripcionTipoAutenticacion + "')\"><i class='material-icons'>mail</i></a>" : "";
            }
        }
    }
}
