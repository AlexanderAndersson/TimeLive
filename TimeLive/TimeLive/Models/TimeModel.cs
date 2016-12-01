using System;
using System.Collections.Generic;

namespace TimeLive.Models
{
    public class Events
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public string textColor { get; set; }
        public string description { get; set; }
        public string invoiced { get; set; }

    }

    public class TimeModel
    {
        public IEnumerable<DateTime> LessThan8H { get; set; }
        public IEnumerable<q_SelectRowsTime_Result> LatestRows { get; set; }
        public IEnumerable<q_SelectRowsTime_Result> SelectRows { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<SubProject> SubProjects { get; set; }
        public TimeSelection Selections { get; set; }
    }

    public class TimeSelection
    {
        public int? ProjectId { get; set; }
        public string SubProjectId { get; set; }
        public string CustomerId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Delayed { get; set; }

        public static TimeSelection ThisWeek
        {
            get
            {
                var delta = DayOfWeek.Monday - DateTime.Today.DayOfWeek;
                delta = delta > 0 ? -6 : delta;
                var from = (DateTime.Today.AddDays(delta));

                return new TimeSelection { From = from, To = DateTime.Today };
            }
        }

        public static TimeSelection ThisMonth
        {
            get
            {
                var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                return new TimeSelection { From = from, To = DateTime.Today };
            }
        }
    }

    public class Customer
    {
        public Customer(dynamic def)
        {
            Code = def.ftgnr;
            Name = def.ftgnamn;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Project
    {
        public Project(dynamic def)
        {
            Id = def.projcode;
            Description = def.projdescr;
            CustomerCode = def.ftgnr;
            Status = def.ProjectStatus.ToLower();
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public string CustomerCode { get; set; }
        public string Status { get; set; }
    }

    public class SubProject
    {
        public SubProject(dynamic def)
        {
            Id = def.strdatetimehr;
            Description = def.aktivitet;
            ProjectId = def.projcode;
            Status = def.SubProjectStatus.ToLower();
            Mandatory = def.SubprojectEntry == "Mandatory";
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
        public string Status { get; set; }
        public bool Mandatory { get; set; }
    }
}