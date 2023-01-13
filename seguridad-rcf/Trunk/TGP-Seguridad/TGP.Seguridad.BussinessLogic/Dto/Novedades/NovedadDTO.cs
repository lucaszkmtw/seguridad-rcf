using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class NovedadDTO
    {

        public static string NOVEDADTODOS = "POR";

        public static string TIPOADVERTENCIA = "ADVERTENCIA";
        public static string TIPONOTIFICACION = "NOTIFICACION";

        public long Id { get; set; }
        public int Version { get; set; }
        public string EstructuraFuncional { get; set; }

        public string EstructuraFuncionalCodigo { get; set; }

        public string EstructuraFuncionalCodigoUICss
        {
            get
            {
                return TGP.Seguridad.BussinessLogic.Helpers.HelperService.ObtenerCssColorDeApliacion(EstructuraFuncionalCodigo);
            }
        }

        public string EstructuraFuncionalDescripcion { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> NodosFuncionales { get; set; }
        [DisplayName("Titulo")]
        public string Titulo { get; set; }

        [DisplayName("Descripcion")]
        [Required(ErrorMessage = "Campo requerido.")]
        public string Descripcion { get; set; }

        [DisplayName("Fecha Desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "Campo requerido.")]
        public DateTime? FechaDesde { get; set; }

        [DisplayName("Fecha Hasta")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? FechaHasta { get; set; }

        [DisplayName("Versión")]
        public string NumeroVersion { get; set; }

        public string TipoNovedad { get; set; }
        public bool SiPublica { get; set; }
        public bool SiLeida { get; set; }
        #region // Variables //
        private static int LIMITE_DESCRIPCION = 40;

        private static int LIMITE_TITULO = 120;
        #endregion
        public string GetResumen()
        {
            string sinTags = Regex.Replace(Descripcion, "<.*?>", String.Empty);
            if (sinTags.Length > LIMITE_DESCRIPCION)
                return sinTags.Remove(LIMITE_DESCRIPCION) + " ...";
            return sinTags;
        }

        public string GetResumenLista()
        {
            if (Descripcion.Length > LIMITE_TITULO)
                return Descripcion.Remove(LIMITE_TITULO) + " ...";
            return Descripcion;
        }

    }

}
