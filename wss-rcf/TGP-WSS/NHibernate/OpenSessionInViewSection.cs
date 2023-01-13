using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NHibernate
{
    public class OpenSessionInViewSection : ConfigurationSection
    {
        [ConfigurationProperty("sessionFactories", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(SessionFactoriesCollection),
            AddItemName = "sessionFactory",
            ClearItemsName = "clearFactories")]
        public SessionFactoriesCollection SessionFactories
        {
            get
            {
                SessionFactoriesCollection sessionFactoriesCollection =
                    (SessionFactoriesCollection)base["sessionFactories"];
                return sessionFactoriesCollection;
            }
        }
    }
}