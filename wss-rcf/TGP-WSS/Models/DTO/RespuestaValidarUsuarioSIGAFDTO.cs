using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Models.DTO
{
    public enum CodigoRespuestaValidarUsuarioSIGAF
    {
        OK,
        Error,
        Bloqueado,
        DadoBaja
    }

    public class RespuestaValidarUsuarioSIGAFDTO
    {
        public CodigoRespuestaValidarUsuarioSIGAF Codigo { get; set; }

        public string Descripcion { get; set; }
    }
}