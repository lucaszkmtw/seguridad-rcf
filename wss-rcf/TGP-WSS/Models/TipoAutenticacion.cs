using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class TipoAutenticacion {

        public static string Interna = "1";
        public static string LDAP = "2";
        public static string ActiveDirectory = "3";

        public TipoAutenticacion() { }
        public virtual long CId { get; set; }
        public virtual Usuario Usuario { get; set; }
        [Required]
        public virtual string CCodigo { get; set; }
        [Required]
        public virtual string DDescripcion { get; set; }
        [Required]
        public virtual int NVersionHibernate { get; set; }
        public virtual DateTime? FUltimaOperacion { get; set; }
    }
}
