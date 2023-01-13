
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TGP.Seguridad.DataAccess.Infrastructure
{
    public class DeepTransformer : IResultTransformer
    {
        private string[] complexAliasesP { get; set; }
        private Type EntityType { get; set; }

        public DeepTransformer(string[] properties, Type tipoEntidad)
        {
            this.complexAliasesP = properties;
            this.EntityType = tipoEntidad;
        }
        // rows iterator
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            aliases = complexAliasesP;
            var list = new List<string>(aliases);

            var propertyAliases = new List<string>(list);
            var complexAliases = new List<string>();

            for (var i = 0; i < list.Count; i++)
            {
                var aliase = list[i];
                // Aliase with the '.' represents complex IPersistentEntity chain
                if (aliase.Contains('.'))
                {
                    complexAliases.Add(aliase);
                    propertyAliases[i] = null;
                }
            }

            // be smart use what is already available
            // the standard properties string, valueTypes
            var result = Transformers
                 .AliasToBean(EntityType)
                 .TransformTuple(tuple, propertyAliases.ToArray());

            TransformPersistentChain(tuple, complexAliases, result, list);

            return result;
        }

        /// <summary>Iterates the Path Client.Address.City.Code </summary>
        protected virtual void TransformPersistentChain(object[] tuple
              , List<string> complexAliases, object result, List<string> list)
        {
            dynamic entity = result;

            foreach (var aliase in complexAliases)
            {
                // the value in a tuple by index of current Aliase
                var index = list.IndexOf(aliase);
                var value = tuple[index];
                if (value == null)
                {
                    continue;
                }

                // split the Path into separated parts
                var parts = aliase.Split('.');
                var name = parts[0];

                var propertyInfo = entity.GetType()
                      .GetProperty(name, BindingFlags.NonPublic
                                       | BindingFlags.Instance
                                       | BindingFlags.Public);

                object currentObject = entity;
                var current = 1;


                while (current < parts.Length)
                {
                    name = parts[current];
                    object instance = propertyInfo.GetValue(currentObject);
                    if (instance == null)
                    {
                        instance = Activator.CreateInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(currentObject, instance);
                    }
                    propertyInfo = propertyInfo.PropertyType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    currentObject = instance;
                    current++;
                }
                
                // even dynamic objects could be injected this way
                var dictionary = currentObject as IDictionary;
                if (dictionary != null)
                {
                    dictionary[name] = value;
                }
                else
                {
                    propertyInfo.SetValue(currentObject, value);
                }
            }
        }

        // convert to DISTINCT list with populated Fields
        public System.Collections.IList TransformList(System.Collections.IList collection)
        {
            var results = Transformers.AliasToBean(EntityType).TransformList(collection);
            return results;
        }
    }
}
