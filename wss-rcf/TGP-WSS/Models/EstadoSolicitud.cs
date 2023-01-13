using System;
using System.Text;
using System.Collections.Generic;


namespace TGP.WSS
{
    
    public class EstadoSolicitud {
        public EstadoSolicitud() { }

        public static readonly string VERIFICACION = "1";
        public static readonly string ACTIVO = "2";
        public static readonly string BLOQUEADO = "3";
        public static readonly string PENDIENTE = "4";
        public static readonly string ARCHIVADO = "5";


        public virtual long Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual int NVersionHibernate { get; set; }
    }
}
