

using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.Dto;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseGetEntidades:ResponseAPI
    {
        public IList<ElementoComparar> Elementos { get; set; }
    }
}
