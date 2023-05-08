using System.Web;
using System.Web.Optimization;

namespace UniversityCity
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            /*Templete Website*/

            //#region Templete Website

            //bundles.Add(new ScriptBundle("~/Templete/js").Include(
            //        "~/Content/assests/jquery/jquery.min.js",
            //        "~/Content/bootstrap/js/bootstrap.min.js",
            //        "~/Content/slick/slick.min.js",
            //        "~/Content/countDown/js/countDown.js",
            //        "~/Content/dateTimePicker/js/moment.min.js",
            //        "~/Content/dateTimePicker/js/dateTimePicker.min.js",
            //        "~/scripts/main.js"

            //        ));

            //bundles.Add(new StyleBundle("~/Templete/css").Include(

            //        "~/Content/assests/bootstrap/css/bootstrap.min.css",
            //        "~/Content/assests/slick/slick.css",
            //        "~/Content/assests/slick/slick-theme.css",
            //        "~/Content/assests/fontawesome/css/fontawesome-all.min.css",
            //        "~/Content/assests/dateTimePicker/css/dateTimePicker.min.css",
            //        "~/Content/css/style.css",
            //        "~/Content/css/media.css"
            //    ));

            //#endregion
        }
    }
}
