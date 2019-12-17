﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.EF
{
    public class DbInitializer
    {
        public static void InitializeData(TCSContext context, IUserRepo userRepo, IHostingEnvironment env)
        {
            context.Database.Migrate();
            if (!env.IsProduction())
            {
                ClearData(context);
                SeedData(context);
            }
            Task.Run(() => SeedAdmin(context, userRepo)).Wait();
        }

        private static void ClearData(TCSContext context)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ClassTours]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[SessionReasons]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[SessionClasses]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Sessions]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Classes]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Departments]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Reasons]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[People]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Semesters]");
            context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Users]");
        }

        private async static Task SeedAdmin(TCSContext context, IUserRepo userRepo)
        {
            if (await context.Users.AnyAsync()) return;
            var user = new User()
            {
                FirstName = "Change",
                LastName = "Username",
                Username = "tcs",
            };
            await userRepo.Create(user, "Develop@90");
        }

        private static void SeedData(TCSContext context)
        {
            string classTours = File.ReadAllText(@"./SampleData/dbo.ClassTours.data.sql");
            string classes = File.ReadAllText(@"./SampleData/dbo.Classes.data.sql");
            string departments = File.ReadAllText(@"./SampleData/dbo.Departments.data.sql");
            string people = File.ReadAllText(@"./SampleData/dbo.People.data.sql");
            string reasons = File.ReadAllText(@"./SampleData/dbo.Reasons.data.sql");
            string semesters = File.ReadAllText(@"./SampleData/dbo.Semesters.data.sql");
            string sessions = File.ReadAllText(@"./SampleData/dbo.Sessions.data.sql");
            string sessionReasons = File.ReadAllText(@"./SampleData/dbo.SessionReasons.data.sql");
            string sessionClasses = File.ReadAllText(@"./SampleData/dbo.SessionClasses.data.sql");


            context.Database.ExecuteSqlCommand(classTours);
            context.Database.ExecuteSqlCommand(departments);
            context.Database.ExecuteSqlCommand(classes);
            context.Database.ExecuteSqlCommand(people);
            context.Database.ExecuteSqlCommand(reasons);
            context.Database.ExecuteSqlCommand(semesters);
            context.Database.ExecuteSqlCommand(sessions);
            context.Database.ExecuteSqlCommand(sessionReasons);
            context.Database.ExecuteSqlCommand(sessionClasses);
        }
    }
}
