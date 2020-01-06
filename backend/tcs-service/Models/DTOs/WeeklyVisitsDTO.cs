using System;


namespace tcs_service.Models.DTO
{
    public class WeeklyVisitsDTO
    {
        public WeeklyVisitsDTO(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        readonly private DateTime startDate;
        readonly private DateTime endDate;

        public string Item => $"{startDate.Date.ToShortDateString()} - {endDate.ToShortDateString()}";

        public int Count { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WeeklyVisitsDTO wk && this == wk;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(startDate, endDate, Count);
        }

        public static bool operator ==(WeeklyVisitsDTO x, WeeklyVisitsDTO y)
        {
            return x.startDate == y.startDate && x.endDate == y.endDate && x.Count == y.Count;
        }

        public static bool operator !=(WeeklyVisitsDTO x, WeeklyVisitsDTO y)
        {
            return !(x == y);
        }


    }
}
