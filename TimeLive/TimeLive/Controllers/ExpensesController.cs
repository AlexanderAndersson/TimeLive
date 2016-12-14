using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TimeLive.Models;
using System.Security.Principal;

namespace TimeLive.Controllers
{
    public class ExpensesController : Controller
    {
        private static readonly OptEntities TimeLiveDB = new OptEntities();

        private static IEnumerable<Customer> customers;
        private static IEnumerable<Project> projects;
        //private static IEnumerable<SubProject> subProjects;

        private static DateTime lastUpdate = DateTime.FromFileTime(0);
        private static readonly TimeSpan updateFrequency = TimeSpan.FromMinutes(5);
       
        // GET: Expenses
        public ActionResult Index()
        {

            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };

            if (DateTime.Now - lastUpdate >= updateFrequency) UpdateStatics();
            // var selections = TimeSelection.ThisWeek;
            var selections = Session["selection"] as ExpensesSelection ?? ExpensesSelection.ThisWeek;
            
            var model = new ExpensesModel
            {
                SelectRows = TimeLiveDB.q_SelectRowsExpense(
                    ((Classes.UserClass.User)Session["User"]).Username,
                    selections.ProjectId, selections.CustomerId, null,
                    null, selections.From, selections.To, null, null),

                Customers = customers,
                Projects = projects,
                Selections = selections,
                //Types = selections.types,
            };

            //ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(string pCompanyId, int pProjectId,string pArtnr, DateTime pRegDate, int? pReimburse, double? pQty, decimal? pAmount_excl_Vat, decimal? pVat_amount, string pExternComment, string pInternComment)
        {
            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_InsertRowExpense(user.Username, pProjectId, pCompanyId, pArtnr, pRegDate, pReimburse,
                pAmount_excl_Vat, pVat_amount, pExternComment, pInternComment, pQty, null, null, null);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string q_ex_guuid, string debug_msg)
        {
            TimeLiveDB.q_DeleteRowExpense(q_ex_guuid, null);
            return Json(new { Result = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string q_ex_guuid, int pProjectId, string pCompanyId, string pArtnr, DateTime? pRegDate, int? pReimburse, double? pQty, decimal? pAmount_excl_Vat, decimal? pVat_amount, string pExternComment, string pInternComment)
        {
            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_UpdateRowExpense(q_ex_guuid, user.Username, pProjectId, pCompanyId, pArtnr, pRegDate, pReimburse,
                pAmount_excl_Vat, pVat_amount, pExternComment, pInternComment, pQty, null, null, null);

            //return Json(new { Result = true });

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(DateTime selectionFrom, DateTime selectionTo, string companyId, int? projectId, int? pReimburse)
        {
            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            if (DateTime.Now - lastUpdate >= updateFrequency) UpdateStatics();
            if (pReimburse == 0)
                pReimburse = null;

            companyId = string.IsNullOrEmpty(companyId) ? null : companyId;

            var customerList = from c in customers
                           where c.Code == companyId
                           select c.Name;

            string customerName = customerList.FirstOrDefault();

            var projectList = from p in projects
                              where p.CustomerCode == projectId.ToString()
                              select p.Description;

            string projectName = projectList.FirstOrDefault();

            var selections = new ExpensesSelection
            {
                CustomerId = companyId,
                CustomerName = customerName,
                ProjectId = projectId,
                ProjectName = projectName,
                From = selectionFrom,
                To = selectionTo,
                reimburse = pReimburse,
            };

            Session["selection"] = selections;

            var model = new ExpensesModel
            {
                SelectRows = TimeLiveDB.q_SelectRowsExpense(
                    ((Classes.UserClass.User)Session["User"]).Username,
                    selections.ProjectId, selections.CustomerId, null,
                    null, selections.From, selections.To, selections.reimburse, null),

                Selections = selections,
                Customers = customers,
                Projects = projects,
                //Types = selections.types,
            };

            //ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;

            return View("Index", model);
        }

        private static void UpdateStatics()
        {
            customers = TimeLiveDB.q_getdefinition.Select(x => new { x.ftgnr, x.ftgnamn }).Distinct().ToArray().Select(x => new Customer(x));
            projects = TimeLiveDB.q_getdefinition.Select(x => new { x.ftgnr, x.projdescr, x.projcode, x.ProjectStatus })
                .Where(x => !string.IsNullOrEmpty(x.ProjectStatus))
                .Distinct().ToArray().Select(x => new Project(x));
            lastUpdate = DateTime.Now;
        }

    }
}