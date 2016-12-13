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
        public ActionResult Insert(string companyId, int projectId,string artnr, DateTime regDate, int reimburse, double qty, decimal amount_excl_Vat, decimal? vat_amount, string externComment, string internComment)
        {
            //var artnr = (int)Types;

            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_InsertRowExpense(user.Username, projectId, companyId, artnr, regDate, reimburse, amount_excl_Vat, vat_amount, externComment, internComment,
                 qty, null, null, null);

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
        public ActionResult Update(string q_ex_guuid, int projectId, string companyId, string artnr, DateTime regDate, int reimburse, double qty, decimal amount_excl_Vat, decimal vat_amount, string externComment, string internComment)
        {
            //var artnr = (int)Types;

            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_UpdateRowExpense(q_ex_guuid, user.Username, projectId, companyId, artnr, regDate, reimburse, amount_excl_Vat, vat_amount, externComment, internComment, qty, null, null, null);

            //return Json(new { Result = true });

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(DateTime selectionFrom, DateTime selectionTo, string companyId, int? projectId, int? pReimburse)
        {
            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            if (DateTime.Now - lastUpdate >= updateFrequency) UpdateStatics();
            if (pReimburse == 0)
                pReimburse = null;

            companyId = string.IsNullOrEmpty(companyId) ? null : companyId;

            var selections = new ExpensesSelection
            {
                CustomerId = companyId,
                ProjectId = projectId,
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

            ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;
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