using System;
using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class RequestCrearNovedad : RequestAPI
    {
        public string EstructuraFuncional { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> NodosFuncionales { get; set; }
        public string Descripcion { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string TipoNovedad { get; set; }
        public bool SiPublica { get; set; }
        public string Titulo { get; set; }
    }


}
