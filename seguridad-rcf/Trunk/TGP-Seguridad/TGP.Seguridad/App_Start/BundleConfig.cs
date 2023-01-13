using System.Web;
using System.Web.Optimization;

namespace TGP.Seguridad.App_Start
{
    /// <summary>
    /// Clase que permite la agrupación de enlaces a archivos externos desde el HTML.
    /// Para evitar una tarea tan repetitiva se han creado estos Bundles, que son accesos 
    /// directos a agrupaciones de este tipo de archivos
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Metodo de registracion de los Bundles
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // bundles.Add(new ScriptBundle("~/bundles/filterList").Include(
            //    "~/Scripts/template/FilterFramework/filterlist.js",
            //    "~/Scripts/template/FilterFramework/filters/filter.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-autocomplete.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-check.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-select.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-select-multiple.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-text-multiple.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-text.js",
            //    "~/Scripts/template/FilterFramework/filters/filter-type-text-ajax.js"
            //));

            //bundles.UseCdn = true;
            //var cdnRouteResources = "http://192.168.128.21/ResourceDesign/Script/FilterFramework/";

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/filterlist", cdnRouteResources));

            //bundles.Add(new ScriptBundle("~/bundles/filterList", cdnRouteResources).Include(
            //    "~/filterlist.js",
            //    "~/filters/filter.js",
            //    "~/filters/filter-type-autocomplete.js",
            //    "~/filters/filter-type-check.js",
            //    "~/filters/filter-type-select.js",
            //    "~/filters/filter-type-select-multiple.js",
            //    "~/filters/filter-type-text-multiple.js",
            //    "~/filters/filter-type-text.js",
            //    "~/filters/filter-type-text-ajax.js"
            //    ));

            //BundleTable.EnableOptimizations = true;

        }
    }
}
