using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.EF
{
    public class DbInitializer
    {
        private TCSContext _context;

        public DbInitializer(TCSContext context)
        {
            _context = context;
        }

        public static void InitializeData(TCSContext context)
        {
            context.Database.Migrate();
            ClearData(context);
            SeedData(context);
        }

        public static void ClearData(TCSContext context)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ClassTours]");
        }

        private static void SeedData(TCSContext context)
        {
            string classTours = File.ReadAllText(@"../SampleData/dbo.ClassTours.data.sql");

            context.Database.ExecuteSqlCommand(classTours);
        }
    
    }
}
