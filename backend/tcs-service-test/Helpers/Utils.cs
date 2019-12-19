using System;
using AutoMapper;
using tcs_service.Helpers;

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

        public static IMapper GetProjectMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            return config.CreateMapper();
        }
    }
}