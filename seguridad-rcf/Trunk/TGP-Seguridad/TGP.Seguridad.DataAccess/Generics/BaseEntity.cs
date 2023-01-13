using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.DataAccess.Generics
{
    /// <summary>
    /// Clase base que modelo las columnas genericas para determinadas tablas
    /// </summary>
    /// <typeparam name="ID"></typeparam>
    public class BaseEntity
    {
        #region Variables Instancia
        /// <summary>
        /// Identificador unico para instancias de BaseEntity
        /// </summary>
        private Guid guid = Guid.NewGuid();
        #endregion

        #region // Propiedades Publicas //
        /// <summary>
        /// Propiedad que modela la columna ID 
        /// </summary>
        [IsId]
        public virtual long Id { get; set; }
        /// <summary>
        /// Propiedad que modela la columna Codigo
        /// aqui conservamos el nombre de esta propiedad
        /// </summary>
        public virtual string Nombre { get; set; }
        /// <summary>
        /// Propiedad que modela la columna Descripcion
        /// </summary>
        public virtual string Descripcion { get; set; }

        /// <summary>
        /// Nro de version de la instancia para NHibernate.
        /// </summary>
        public virtual int Version { get; set; }
        #endregion

        /// <summary>
        /// Devuelve el ID de la instancia del objeto
        /// </summary>
        /// <returns></returns>
        public virtual Guid GUID()
        {
            return this.guid;
        }

        public virtual Dictionary<string, object> GetId()
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
    }
}
