using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TGP.WSS.Models.Requerimiento;
using TGP.WSS.Models.Resultado;

namespace TGP.WSS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGenerarUsuarioService" in both code and config file together.
    [ServiceContract]
    public interface IGenerarUsuarioService
    {
        /// <summary>
        /// Metodo utilizado para probar el servicio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ResultadoDTO Eco();
       
        /// <summary>
        /// Metodo que permite la generacion de un Usuario Nominado desde una aplicacion externa a la aplicacion
        /// de seguridad
        /// NOTA: Se esta invocando desde la aplicacion de "Consulta de Expediente" cuando se aprueba la solicitud de usuario
        /// y tambien desde "RUCO".
        /// </summary>
        /// <param name="requerimiento"></param>
        /// <returns></returns>
        [OperationContract]
        ResultadoGenerarUsuarioNominadoUsuarioDTO GenerarNominado(RequerimientoGenerarUsuarioNominadoDTO requerimiento);
        
    }
}
