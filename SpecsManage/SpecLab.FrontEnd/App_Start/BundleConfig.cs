using System.Web;
using System.Web.Optimization;

namespace SpecLab.FrontEnd
{
    public class BundleConfig
    {
        private static void RegisterCommonBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                "~/Scripts/jquery-2.1.0.js",
                "~/Scripts/jquery.json-2.3.js",
                "~/Scripts/jquery.numeric.js",
                "~/Scripts/angular.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/locales/bootstrap-datepicker.vi.js", 
                "~/Scripts/speclab.common.js"));

            bundles.Add(new StyleBundle("~/Content/common").Include(
                "~/Content/bootstrap.css",
                "~/Content/blue.css",
                "~/Content/common.css",
                "~/Content/datepicker.css",
                "~/Content/datepicker3.css"));
        }

        private static void RegisterChangePassBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/change_pass").Include(
                "~/Scripts/pages/change_pass.js"));

            bundles.Add(new StyleBundle("~/Content/change_pass").Include(
                "~/Content/pages/change_pass.css"));
        }

        private static void RegisterSignInBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/signin").Include(
                "~/Scripts/pages/signin.js"));

            bundles.Add(new StyleBundle("~/Content/signin").Include(
                "~/Content/pages/signin.css"));
        }

        private static void RegisterSpecControlBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/spec_control").Include(
                "~/Scripts/pages/spec_control.js"));

            bundles.Add(new StyleBundle("~/Content/spec_control").Include(
                "~/Content/pages/spec_control.css"));
        }

        private static void RegisterSpecExportBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/spec_export").Include(
                "~/Scripts/pages/spec_export.js"));

            bundles.Add(new StyleBundle("~/Content/spec_export").Include(
                "~/Content/pages/spec_export.css"));
        }

        private static void RegisterSpecImportBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/spec_import").Include(
                "~/Scripts/pages/spec_import.js"));

            bundles.Add(new StyleBundle("~/Content/spec_import").Include(
                "~/Content/pages/spec_import.css"));
        }

        private static void RegisterSpecRemovalBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/spec_removal").Include(
                "~/Scripts/pages/spec_removal.js"));

            bundles.Add(new StyleBundle("~/Content/spec_removal").Include(
                "~/Content/pages/spec_removal.css"));
        }

        private static void RegisterStoragesControlBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/storages_control").Include(
                "~/Scripts/pages/storages_control.js"));

            bundles.Add(new StyleBundle("~/Content/storages_control").Include(
                "~/Content/pages/storages_control.css"));
        }

        private static void RegisterUsersControlBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/users_control").Include(
                "~/Scripts/pages/users_control.js"));

            bundles.Add(new StyleBundle("~/Content/users_control").Include(
                "~/Content/pages/users_control.css"));
        }

        private static void RegisterContentControlBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/content_control").Include(
                "~/Scripts/pages/content_control.js"));

            bundles.Add(new StyleBundle("~/Content/content_control").Include(
                "~/Content/pages/content_control.css"));
        }

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterCommonBundles(bundles);
            RegisterChangePassBundles(bundles);
            RegisterSignInBundles(bundles);
            RegisterSpecControlBundles(bundles);
            RegisterSpecExportBundles(bundles);
            RegisterSpecImportBundles(bundles);
            RegisterSpecRemovalBundles(bundles);
            RegisterStoragesControlBundles(bundles);
            RegisterUsersControlBundles(bundles);
            RegisterContentControlBundles(bundles);
        }
    }
}