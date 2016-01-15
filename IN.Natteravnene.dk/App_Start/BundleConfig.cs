using System.Web;
using System.Web.Optimization;

namespace IN.Natteravnene.dk
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/bundles/style").Include(
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/ace.css",
                         "~/Content/css/nrtheme.css",
                         "~/Content/css/datepicker.css",
                          "~/Content/css/bootstrap-timepicker.css",
                //"~/Content/smalot-datetimepicker/bootstrap-datetimepicker.min.css",
                        "~/Content/css/jquery.datetimepicker.css",
                         "~/Content/css/chosen.css",
                         "~/Content/css/jquery-ui.custom.min.css",
                          "~/Content/css/font-awesome.min.css",
                          "~/Content/css/nrfont/nrfont.css",
                         "~/Content/css/Animate.css"
                        ));



            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/jquery-ui-1.11.2.js",
                //"~/Scripts/jquery-ui.custom.min.js",
                //"~/Scripts/markdown/markdown.min.js",
                //"~/Scripts/markdown/bootstrap-markdown.min.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/ace-elements.min.js",
                        "~/Scripts/ace.min.js",
                        "~/Scripts/bootbox.min.js",
                        "~/Scripts/NRScripts.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/table").Include(
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/jquery.dataTables.bootstrap.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/form").Include(
                        "~/Scripts/chosen.jquery.min.js",
                        "~/Scripts/fuelux/fuelux.wizard.min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/jquery.datetimepicker.js",
                        "~/Scripts/locales/bootstrap-datepicker.da.min.js",
                        "~/Scripts/jquery.hotkeys.min.js",
                        "~/Scripts/bootstrap-wysiwyg.js",
                        "~/Scripts/jquery.maskedinput.min.js",

                        "~/Scripts/jquery.inputlimiter.1.3.1.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                       "~/Scripts/jquery.form.min.js",
                       "~/Scripts/jquery.Jcrop.min.js",
                       "~/Scripts/avatar.js"
                       ));



            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));



            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*",
                        "~/Scripts/respond.min.js"
                        ));

            /*bundles.Add(new LessBundle("~/assets/css/style").Include(
                        //"~/assets/css/style.min.css",
                        "~/assets/css/bootstrap/bootstrap.less",
                        "~/assets/css/font-awesome.min.css",
                        "~/assets/css/jquery-ui-1.10.3.custom.min.css",
                        "~/assets/css/jquery.gritter.css",
                        "~/assets/css/select2.css",
                        "~/assets/css/ace-fonts.css",
                        "~/assets/css/ace.min.css",
                        "~/assets/css/ace-rtl.min.css",
                        "~/assets/css/ace-skins.min.css",
                        "~/assets/css/ace-ie.min.css",
                        "~/css/less/nrmod/nrtheme.less"));*/



            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}