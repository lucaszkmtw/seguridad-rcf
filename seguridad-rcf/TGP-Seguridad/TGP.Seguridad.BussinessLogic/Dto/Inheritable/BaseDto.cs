using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.Common.Dto
{
    /// <summary>
    /// Clase Base de los DTO para serializar la clase base del dominio
    /// Todas las clases que deriben de ella deben tener el sufijo DTO
    /// </summary>
    public class BaseDto
    {
        #region Variables Instancia
        private Guid guid = Guid.NewGuid();
        #endregion

        #region // Propiedades Publicas //
        /// <summary>
        /// Identificacion unica de la clase
        /// </summary>
        [IsId]
        public long Id { get; set; }

        /// <summary>
        /// Valor que identifica a la clase
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Valor descriptivo de la Clase
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Nro de version de la instancia para NHibernate.
        /// </summary>
        public virtual int Version { get; set; }
        #endregion

        public virtual Guid GUID()
        {
            return this.guid;
        }

        #region // Métodos Públicos //

        //public virtual Dictionary<string, object> GetId()
        //{
        //    return new Dictionary<string, object>() { { nameof(Id), Id } };
        //}
        public Dictionary<string, object> GetId()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            IList<PropertyInfo> properties = this.GetType().GetProperties();
            properties = properties.Where(x => x.GetCustomAttributes(false).Any(a => a.GetType() == typeof(IsIdAttribute))).ToList();
            if (properties.Count > 1)
            {
                properties = properties.Where(x => !x.Name.Equals("Id")).ToList();
            }
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                dictionary.Add(property.Name, property.GetValue(this));
            }
            return dictionary;
        }

        public virtual string IdSerialize()
        {
            return JsonConvert.SerializeObject(this.GetId());
        }
        public static Dictionary<string, object> IdDeserialize(string id)
        {
            return (Dictionary<string, object>)JsonConvert.DeserializeObject(id, typeof(Dictionary<string, object>));
        }

        #endregion
    }
}
