using Microsoft.EntityFrameworkCore;
using tcs_service.EF;

namespace tcs_service_test.Helpers {
    public class DbInMemory
    {
        public static DbContextOptions getDbInMemoryOptions(string dbName)
        {
            var options = new DbContextOptionsBuilder<TCSContext>()
                  .UseInMemoryDatabase(databaseName: dbName)
                  .Options;
            return options;
        }
    }
}