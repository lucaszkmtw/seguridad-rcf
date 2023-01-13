using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{

    public class TerminosCondiciones
    {
        public TerminosCondiciones() { }
        public virtual long CId { get; set; }
        public virtual int NVersionHibernate { get; set; }
        [Required]
        public virtual byte[] Descripcion { get; set; }
        [Required]
        public virtual string Version { get; set; }
        [Required]
        public virtual DateTime FDesde { get; set; }

        public virtual DateTime? FHasta { get; set; }
        [Required]
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
    }
}
