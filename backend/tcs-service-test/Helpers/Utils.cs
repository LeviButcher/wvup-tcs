using System;

namespace tcs_service_test.Helpers
{
    public class Utils
    {
        public static int ParseOrDefault(string toParse, int fallback)
        {
            int resultStorage;
            bool success = Int32.TryParse(toParse, out resultStorage);
            if (success) return resultStorage;
            else return fallback;
        }
    }
}