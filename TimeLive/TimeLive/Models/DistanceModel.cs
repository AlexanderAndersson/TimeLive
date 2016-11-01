using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeLive.Models
{
    public class DistanceModel
    {
        public IEnumerable<q_SelectRowsDistance_Result> SelectRows { get; set; }
        public DistanceSelection Selections { get; set; }

    }
    public class DistanceSelection
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public static DistanceSelection ThisWeek
        {
            get
            {
                var delta = DayOfWeek.Monday - DateTime.Today.DayOfWeek;
                delta = delta > 0 ? -6 : delta;
                var from = (DateTime.Today.AddDays(delta));

                return new DistanceSelection { From = from, To = DateTime.Today };
            }
        }

        public static DistanceSelection ThisMonth
        {
            get
            {
                var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                return new DistanceSelection { From = from, To = DateTime.Today };
            }
        }
    }
}