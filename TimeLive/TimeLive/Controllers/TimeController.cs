using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TimeLive.Models;
using System.Security.Principal;

namespace TimeLive.Controllers
{
    public class TimeController : Controller
    {
        private static readonly OptEntities TimeLiveDB = new OptEntities();

        private static IEnumerable<Customer> customers;
        private static IEnumerable<Project> projects;
        private static IEnumerable<SubProject> subProjects;

        private static DateTime lastUpdate = DateTime.FromFileTime(0);
        private static readonly TimeSpan updateFrequency = TimeSpan.FromMinutes(5);

        public ActionResult Index()
        {

            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };

            if (DateTime.Now - lastUpdate >= updateFrequency) UpdateStatics();
            // var selections = TimeSelection.ThisWeek;
            var selections = Session["selection"] as TimeSelection ?? TimeSelection.ThisWeek;
            var model = new TimeModel
            {
                SelectRows = TimeLiveDB.q_SelectRowsTime(
                    null, ((Classes.UserClass.User)Session["User"]).Username,
                    selections.ProjectId, null, selections.SubProjectId, null,
                    selections.CustomerId, null, null, selections.From,
                    selections.To, selections.Delayed, null, null),

                Customers = customers,
                Projects = projects,
                SubProjects = subProjects,
                Selections = selections
            };

            ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(string pCompanyId, int pProjectId, string pSubProjectId, DateTime pRegDate, double pInvoicedTime, double pUsedTime, string pExternComment, string pInternComment, int pDealyinvoice)
        {
            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_InsertRowTime(user.Username, pProjectId, pSubProjectId, pCompanyId, pRegDate, pExternComment, pInternComment,
                 pInvoicedTime, pUsedTime, pDealyinvoice, null, null);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string q_hrp_guiid, string pCompanyId, int pProjectId, string pSubProjectId, DateTime pRegDate, double? pInvoicedTime, double pUsedTime, string pExternComment, string pInternComment, int pDealyinvoice)
        {
            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_UpdateRowTime(q_hrp_guiid, user.Username, pProjectId, pSubProjectId, pCompanyId, pRegDate, pExternComment, pInternComment,
                pInvoicedTime, pUsedTime, pDealyinvoice, null, null, null, null);

            //return Json(new { Result = true });
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string q_hrp_guiid)
        {
            TimeLiveDB.q_DeleteRowTime(q_hrp_guiid);
            return Json(new { Result = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(DateTime selectionFrom, DateTime selectionTo, string companyId, int? projectId, string subProjectId, int? pDealyinvoice)
        {
            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            if (DateTime.Now - lastUpdate >= updateFrequency) UpdateStatics();
            if (pDealyinvoice == 0)
                pDealyinvoice = null;

            subProjectId = string.IsNullOrEmpty(subProjectId) ? null : subProjectId;
            companyId = string.IsNullOrEmpty(companyId) ? null : companyId;

            var selections = new TimeSelection
            {
                CustomerId = companyId,
                From = selectionFrom,
                To = selectionTo,
                Delayed = pDealyinvoice,
                ProjectId = projectId,
                SubProjectId = subProjectId
            };

            Session["selection"] = selections;

            var model = new TimeModel
            {
                SelectRows = TimeLiveDB.q_SelectRowsTime(
                    null, ((Classes.UserClass.User)Session["User"]).Username, 
                    selections.ProjectId, null, selections.SubProjectId, null, 
                    selections.CustomerId, null, null,selections.From, 
                    selections.To, selections.Delayed, null, null),

                Selections = selections,
                Projects = projects,
                SubProjects = subProjects,
                Customers = customers
            };

            ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;
            return View("Index", model);
        }

        private static void UpdateStatics()
        {
            customers = TimeLiveDB.q_getdefinition.Select(x => new { x.ftgnr, x.ftgnamn }).Distinct().ToArray().Select(x => new Customer(x));
            projects = TimeLiveDB.q_getdefinition.Select(x => new { x.ftgnr, x.projdescr, x.projcode, x.ProjectStatus })
                .Where(x => !string.IsNullOrEmpty(x.ProjectStatus))
                .Distinct().ToArray().Select(x => new Project(x));
            subProjects = TimeLiveDB.q_getdefinition.Select(x => new { x.projcode, x.aktivitet, x.strdatetimehr, x.SubProjectStatus, x.SubprojectEntry })
                .Where(x => !string.IsNullOrEmpty(x.SubProjectStatus))
                .Distinct().ToArray().Select(x => new SubProject(x));
            lastUpdate = DateTime.Now;
        }   
    }
}