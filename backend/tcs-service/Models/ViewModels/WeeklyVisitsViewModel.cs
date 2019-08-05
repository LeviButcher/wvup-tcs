using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class WeeklyVisitsViewModel
    {
        public WeeklyVisitsViewModel(DateTime startDate, DateTime endDate) {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        private DateTime startDate;
        private DateTime endDate;

        public string Item => $"{startDate.Date.ToShortDateString()} - {endDate.ToShortDateString()}";

        public int Count { get; set; }

        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            WeeklyVisitsViewModel wv = (WeeklyVisitsViewModel) obj;
            // TODO: write your implementation of Equals() here
            return startDate == wv.startDate && endDate == wv.endDate && Count == wv.Count;
        }
    }
}
