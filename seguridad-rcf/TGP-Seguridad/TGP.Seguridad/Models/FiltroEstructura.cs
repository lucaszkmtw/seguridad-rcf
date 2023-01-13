using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;

namespace Seguridad
{
    public class FiltroEstructuras
    {
        protected EstructuraApplicationService _brokerServices = EstructuraApplicationService.Instance;

        public FiltroEstructuras(bool todos, EstructuraFuncionalDTO estructuraSelected)
        {

            this.Estructuras = new List<EstructuraFuncionalDTO>();
            if (todos)
            {
                EstructuraFuncionalDTO opcionTodos = new EstructuraFuncionalDTO();
                opcionTodos.DescripcionEstructura = "--TODOS--";
                opcionTodos.Id = 0;
                opcionTodos.Codigo = "0";
                estructuras.Add(opcionTodos);
            }

            string[] propiedades = new string[]
            {
                "DescripcionEstructura",
                "Id"
            };

            List<EstructuraFuncionalDTO> estr = (List<EstructuraFuncionalDTO>)_brokerServices.GetEstructuras(bool.Parse(HttpContext.Current.Session["EsAdmin"].ToString())); 
            Estructuras.AddRange(estr.OrderBy(e => e.DescripcionEstructura).ToList());

            if (estructuraSelected == null)
            {
                Object valor = HttpContext.Current.Session["estructuraSelected"];
                // Si en la sesion habia quedado la estructura con id 0 (TODOS) y para este filtro no admite ese valor deberia quedarse con el primero.
                if (valor != null)
                {
                    if ( (string)valor != "0")
                    {
                       this.EstructuraSelected = (EstructuraFuncionalDTO)_brokerServices.GetEstructuraPorCodigo(valor.ToString());
                    }
                    else
                    {
                        if (todos)
                        {
                            this.EstructuraSelected = estructuras.FindAll(e => e.Id == 0).First();
                            HttpContext.Current.Session["estructuraSelected"] = estructuras.FindAll(e => e.Id == 0).First().Id.ToString();
                        }
                        else
                        {
                            this.EstructuraSelected = estructuras.FindAll(e => e.Id != 0).First();                           
                        }
                    }
                }
                else
                {
                    if (todos)
                    {
                        this.EstructuraSelected = estructuras.FindAll(e => e.Id == 0).First();
                        HttpContext.Current.Session["estructuraSelected"] = estructuras.FindAll(e => e.Id == 0).First().Id.ToString();
                    }
                    else
                    {
                        this.EstructuraSelected = estructuras.FindAll(e => e.Id != 0).First();
                    }
                }
            }
            else
            {
                this.EstructuraSelected = estructuraSelected;
            }
        }

        List<EstructuraFuncionalDTO> estructuras = new List<EstructuraFuncionalDTO>();

        public List<EstructuraFuncionalDTO> Estructuras
        {
            get { return estructuras; }
            set { estructuras = value; }
        }

        EstructuraFuncionalDTO estructuraSelected;

        public EstructuraFuncionalDTO EstructuraSelected
        {
            get { return estructuraSelected; }
            set { estructuraSelected = value; }
        }
    }
}