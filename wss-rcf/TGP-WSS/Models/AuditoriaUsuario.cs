using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class AuditoriaUsuario {
        public virtual long Id { get; set; }
        [Required]
        public virtual string Usuario { get; set; }
        [Required]
        public virtual DateTime Accion { get; set; }
        [Required]
        public virtual string EstructuraFuncional { get; set; }
        [Required]
        public virtual string Actividad { get; set; }
        [Required]
        public virtual string Observacion { get; set; }
        [Required]
        public virtual int VersionHibernate { get; set; }
    }
}
