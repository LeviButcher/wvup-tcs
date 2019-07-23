using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class PeakHoursViewModel
    {

        public PeakHoursViewModel(int hour, int count)
        {
            this.hour = hour;
            this.Count = count;
        }

        private int hour;
        public String Hour => hourToString(hour);


        public int Count { get; set; }
        private string hourToString(int hour)
        {
            switch (hour)
            {
                case 0:
                    return "12-12:59 A.M";
                case 1:
                    return "1-1:59 A.M";
                case 2:
                    return "2-2:59 A.M";
                case 3:
                    return "3-3:59 A.M";
                case 4:
                    return "4-4:59 A.M";
                case 5:
                    return "5-5:59 A.M";
                case 6:
                    return "6-6:59 A.M";
                case 7:
                    return "7-7:59 A.M";
                case 8:
                    return "8-8:59 A.M";
                case 9:
                    return "9-9:59 A.M";
                case 10:
                    return "10-10:59 A.M";
                case 11:
                    return "11-11:59 A.M";
                case 12:
                    return "12-12:59 P.M";
                case 13:
                    return "1-1:59 P.M";
                case 14:
                    return "2-2:59 P.M";
                case 15:
                    return "3-3:59 P.M";
                case 16:
                    return "4-4:59 P.M";
                case 17:
                    return "5-5:59 P.M";
                case 18:
                    return "6-6:59 P.M";
                case 19:
                    return "7-7:59 P.M";
                case 20:
                    return "8-8:59 P.M";
                case 21:
                    return "9-9:59 P.M";
                case 22:
                    return "10-10:59 P.M";
                case 23:
                    return "11-11:59 P.M";
                default:
                    return "Something went wrong";
            }
        }
    }
}
