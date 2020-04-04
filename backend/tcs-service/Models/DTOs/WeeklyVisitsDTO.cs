using System;

namespace tcs_service.Models.DTO {
    ///<summary>Represents the data returned for the WeeklyVisits Report</summary>
    public class WeeklyVisitsDTO {

        ///<summary>WeeklyVisitsDTO contractor</summary>
        public WeeklyVisitsDTO (DateTime startDate, DateTime endDate) {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        readonly private DateTime startDate;
        readonly private DateTime endDate;

        ///<summary>The week this took place in</summary>
        ///This is a poor name, but it needs changed on the frontend too
        public string Item => $"{startDate.Date.ToShortDateString()} - {endDate.ToShortDateString()}";

        ///<summary>The amount of visitors during this week</summary>
        public int Count { get; set; }

        ///<summary>Check whether this is equal to another week</summary>
        public override bool Equals (object obj) {
            return obj is WeeklyVisitsDTO wk && this == wk;
        }

        ///<summary>Return the hash representation of this object</summary>
        public override int GetHashCode () {
            return HashCode.Combine (startDate, endDate, Count);
        }

        ///<summary>Overrides == operator, checks if two weekly visits our in the same week</summary>
        public static bool operator == (WeeklyVisitsDTO x, WeeklyVisitsDTO y) {
            return x.startDate == y.startDate && x.endDate == y.endDate && x.Count == y.Count;
        }

        ///<summary>Overrides != operator, checks if two weekly visits our not in the same week</summary>
        public static bool operator != (WeeklyVisitsDTO x, WeeklyVisitsDTO y) {
            return !(x == y);
        }

    }
}