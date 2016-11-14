﻿using System;
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
        //public string LastEnd;
        public DateTime LastEnd;
        public DateTime From;
        public DateTime To;
        //public static readonly TimeModel hej = new TimeModel();

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

        public ActionResult GetEvents()
        {
            var eventList = NewEvents();
            //var rows = eventList;

            return Json(eventList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReports()
        {
            var eventList = NewEvents();
            var rows = eventList;

            return Json(new { reports = rows }, JsonRequestBehavior.AllowGet);
        }

        private List<Events> NewEvents()
        {
            List<string> color = new List<string>();
            color.Add("#006600");
            color.Add("#006666");
            color.Add("#330066");
            color.Add("#CC6600");
            color.Add("#004C99");
            color.Add("#660066");
            color.Add("#006666");
            color.Add("#660000");

            int count = 0;

            object tdFrom = TempData["From"];
            object tdTo = TempData["To"];


            //If tempdata is null make the from and to dates this week
            if (tdFrom == null && tdTo == null)
            {
                From = DateTime.Today;
                int delta = DayOfWeek.Monday - From.DayOfWeek;
                From = From.AddDays(delta);
                To = From.AddDays(6);
            }
            else
            {
                From = DateTime.Parse(tdFrom.ToString());
                To = DateTime.Parse(tdTo.ToString());
            }

            var selectRows = from c in TimeLiveDB.q_SelectRowsTime(null, ((Classes.UserClass.User)Session["User"]).Username,
                             null, null, null, null,
                             null, null, null, From,
                             To, null, null, null).ToArray().OrderBy((x => x.regdate))
                             select c;

            List<Events> eventList = new List<Events>();

            if (selectRows.Count() > 0)
            {
         
                foreach (var row in selectRows)
                {
                    if (row.regdate == LastEnd.Date)
                    {
                        count++;

                        if (count == 8)
                        {
                            count = 0;
                        }
                    }
                    else
                    {
                        count = 0;   
                    }

                    //Checks if the latest date is the same as the latest report. If not, LastEnd = 00:00:00
                    if (row.regdate != LastEnd.Date)
                    {
                        LastEnd = LastEnd.Date/* + ts*/;
                    }


                    if (LastEnd.Hour == 0)
                    {
                        Events newEvent = new Events //This is always the first event on each day
                        {
                            title = row.usedtime.ToString("0.0" + "h").Replace(",", "."), //Title equals to how many hours you been reporting
                            start = row.regdate.AddHours(1).ToString(), //Start-time begins at 01:00 if LastEnd hour = 00:00:00
                            end = row.regdate.AddHours(1).AddHours((double)row.usedtime).ToString(), //End-time equals to 01:00 + invoicedtime
                            backgroundColor = color[count],
                        };

                        LastEnd = row.regdate.AddHours(1).AddMinutes(LastEnd.Minute).AddHours((double)row.usedtime); //LastEnd equals to 01:00

                        eventList.Add(newEvent); //Add event
                    }
                    else
                    {
                        Events newEvent = new Events //This is always the event after the first event on the same day if there is one
                        {
                            title = row.usedtime.ToString("0.0" + "h").Replace(",", "."), //Title equals to how many hours you been reporting
                            start = LastEnd.ToString(), //Start-time begins when the latest report ended
                            end = row.regdate.AddHours(LastEnd.Hour).AddMinutes(LastEnd.Minute).AddHours((double)row.usedtime).ToString(), //End-time equals to LastEnd + invoicedtime
                            backgroundColor = color[count],
                        };

                        LastEnd = row.regdate.AddHours(LastEnd.Hour).AddMinutes(LastEnd.Minute).AddHours((double)row.usedtime);

                        eventList.Add(newEvent);
                    }
                }
            }
            return eventList;
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

            TempData["From"] = selectionFrom;
            TempData["To"] = selectionTo;

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
                    selections.CustomerId, null, null, selections.From,
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