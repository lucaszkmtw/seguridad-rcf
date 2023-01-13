using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.Common.Dto;

namespace TGP.Seguridad.BussinessLogic.Dto.Usuario
{
    public class TipoAutenticacionDTO : BaseDto
    {
        public static string INTERNA = "1";
        //public static string LDAP = "2";
        public static string SIGAF = "2";
        public static string ACTIVEDIRECTORY = "3";

        public DateTime? FUltimaOperacion { get; set; }
        public int NVersionHibernate { get; set; }
    }
}
