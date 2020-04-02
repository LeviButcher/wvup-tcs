namespace tcs_service.Models.DTO
{
    ///<summary>Represents the data used in the Peak Hours Report, which give a time and a count of visitors during that time</summary>
    public class PeakHoursDTO
    {

        /// <summary>PeakHours Constructor</summary>
        public PeakHoursDTO(int hour, int count)
        {
            this.hour = hour;
            Count = count;
        }

        readonly private int hour;

        /// <summary>The Hour that sign in's took place in</summary>
        public string Hour => HourToString(hour);

        /// <summary>The Count of the amount of visitors during this time</summary>
        public int Count { get; set; }

        private string HourToString(int hour)
        {
            switch (hour)
            {
                case 0:
                    return "12 A.M";
                case 1:
                    return "1 A.M";
                case 2:
                    return "2 A.M";
                case 3:
                    return "3 A.M";
                case 4:
                    return "4 A.M";
                case 5:
                    return "5 A.M";
                case 6:
                    return "6 A.M";
                case 7:
                    return "7 A.M";
                case 8:
                    return "8 A.M";
                case 9:
                    return "9 A.M";
                case 10:
                    return "10 A.M";
                case 11:
                    return "11 A.M";
                case 12:
                    return "12 P.M";
                case 13:
                    return "1 P.M";
                case 14:
                    return "2 P.M";
                case 15:
                    return "3 P.M";
                case 16:
                    return "4 P.M";
                case 17:
                    return "5 P.M";
                case 18:
                    return "6 P.M";
                case 19:
                    return "7 P.M";
                case 20:
                    return "8 P.M";
                case 21:
                    return "9 P.M";
                case 22:
                    return "10 P.M";
                case 23:
                    return "11 P.M";
                default:
                    return "Something went wrong";
            }
        }
    }
}
