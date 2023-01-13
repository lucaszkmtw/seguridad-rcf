using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class AuditoriaConexion {
        public virtual long CId { get; set; }
        [Display(Name="Usuario")]
        public virtual Usuario Usuario { get; set; }
        [Required]
        [Display(Name = "Fecha de Conexion")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public virtual DateTime FConexion { get; set; }
        [Display(Name = "IP de Conexion")]
        public virtual String IPConexion { get; set; }
        [Display(Name = "Navegador")]
        public virtual String Browser { get; set; }
        [Display(Name = "Servidor")]
        public virtual String Server { get; set; }
        [Display(Name = "EstructuraFuncionalCodigo")]
        public virtual String EstructuraFuncionalCodigo { get; set; } 

        //propiedad que se completa luego de la consulta de group by y almacena la cant de veces que se conecta el usuario en un dia
        public virtual int CantConexiones { get; set; }
    }
}
