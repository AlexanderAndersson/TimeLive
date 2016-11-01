using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TimeLive.Models;
using System.Security.Principal;

namespace TimeLive.Controllers
{
    public class DistanceController : Controller
    {
        private static readonly OptEntities TimeLiveDB = new OptEntities();

        private static DateTime lastUpdate = DateTime.FromFileTime(0);
        private static readonly TimeSpan updateFrequency = TimeSpan.FromMinutes(5);

        // GET: Expenses
        public ActionResult Index()
        {

            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };

            var selections = Session["selection"] as DistanceSelection ?? DistanceSelection.ThisWeek;

            var model = new DistanceModel
            {
                SelectRows = TimeLiveDB.q_SelectRowsDistance(
                    ((Classes.UserClass.User)Session["User"]).Username, null),

                Selections = selections,
            };

            //ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(DateTime regDate, double startingDistance, double workDistance, double privateDistance, double endingDistance, string comment)
        {
            Session["User"] = (Classes.UserClass.User)Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };
            var user = (Classes.UserClass.User)Session["User"];

            TimeLiveDB.q_InsertRowDistance(regDate, user.Username, startingDistance, workDistance, privateDistance, endingDistance, comment, null);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(DateTime selectionFrom, DateTime selectionTo)
        {
            Session["User"] = Session["User"] ?? Classes.UserClass.GetUserByIdentity(WindowsIdentity.GetCurrent());
            //Session["User"] = Session["User"] as AdUser ?? new AdUser { Domain = "OPTIVASYS", FullName = "Rasmus Jansson", Username = "raja" };

            var selections = new DistanceSelection
            {
                From = selectionFrom,
                To = selectionTo,
            };

            Session["selection"] = selections;

            var model = new DistanceModel
            {
                SelectRows = TimeLiveDB.q_SelectRowsDistance(
                    ((Classes.UserClass.User)Session["User"]).Username, null)
            };

            ViewBag.User = ((Classes.UserClass.User)Session["User"]).FullName;
            return View("Index", model);
        }
    }
}