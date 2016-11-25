using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeLive.Models
{
    public class ExpensesModel
    {
        public IEnumerable<q_SelectRowsExpense_Result> SelectRows { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        //public IEnumerable<Typee> Types { get; set; }
        public ExpensesSelection Selections { get; set; }
        //public Typee Types { get; set; }
    }

    //public enum Typee
    //{
    //    Milersättning = 100,
    //    Boende = 200,
    //    Transporter = 300,
    //    Representationavdragsgill = 400,

    //}

    public class ExpensesSelection
        {
            //public Typee types { get; set;}
            public int? ProjectId { get; set; }
            public string CustomerId { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public int? reimburse { get; set; }

            public static ExpensesSelection ThisWeek
            {
                get
                {
                    var delta = DayOfWeek.Monday - DateTime.Today.DayOfWeek;
                    delta = delta > 0 ? -6 : delta;
                    var from = (DateTime.Today.AddDays(delta));

                    return new ExpensesSelection { From = from, To = DateTime.Today };
                }
            }

            public static ExpensesSelection ThisMonth
            {
                get
                {
                    var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    return new ExpensesSelection { From = from, To = DateTime.Today };
                }
            }
        }

        //public class Typee
        //{
        //    public Typee(dynamic def)
        //    {
        //        number = def.artnr;
        //    }

        //    public string number { get; set; }
        //}
}