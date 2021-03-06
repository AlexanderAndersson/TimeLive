﻿using System.Web;
using System.Web.Optimization;

namespace TimeLive.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-1.12.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/moment.js"));

            //Calendar css file
            bundles.Add(new StyleBundle("~/Content/fullcalendarcss").Include(
                     "~/Content/themes/jquery.ui.all.css",
                     "~/Content/fullcalendar.css"));


            //Calendar Script file
            bundles.Add(new ScriptBundle("~/bundles/fullcalendarjs").Include(
                      "~/Scripts/jquery-ui-{version}.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/fullcalendar.min.js"));

            bundles.Add(new StyleBundle("~/Content/Timecss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/TimeIndex.css"));

            bundles.Add(new StyleBundle("~/Content/Expensescss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/ExpensesIndex.css"));

            bundles.Add(new StyleBundle("~/Content/Dinstancecss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/DistanceIndex.css"));
        }
    }
}