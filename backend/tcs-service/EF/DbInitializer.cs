﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.EF {
    /// <summary>Encompasses Initializing a database with sampledata</summary>
    public class DbInitializer {
        /// <summary>Initialize the database with Sample Data</summary>
        /// <remarks>
        /// In production environments, it will only create a user to be able to log in with
        /// In non-production environments, it will wipe the database then seed an array of sample data in the db
        /// </remarks>
        public static void InitializeData (TCSContext context, IUserRepo userRepo, IHostingEnvironment env) {
            context.Database.Migrate ();
            if (!env.IsProduction ()) {
                ClearData (context);
                SeedData (context);
            }
            Task.Run (() => SeedAdmin (context, userRepo)).Wait ();
        }

        private static void ClearData (TCSContext context) {
            // All Identifiers in Postgres must be wrapped
            // in double quotes to preserve capitalization
            context.Database.ExecuteSqlCommand ("DELETE FROM \"ClassTours\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"SessionReasons\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"SessionClasses\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"Sessions\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"Classes\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"Departments\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"Reasons\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"People\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"Semesters\"");
            context.Database.ExecuteSqlCommand ("DELETE FROM \"Users\"");
        }

        private async static Task SeedAdmin (TCSContext context, IUserRepo userRepo) {
            if (await context.Users.AnyAsync ()) return;
            var user = new User () {
                FirstName = "Change",
                LastName = "Username",
                Username = "tcs",
            };
            await userRepo.Create (user, "Develop@90");
        }

        private static void SeedData (TCSContext context) {
            string classTours = File.ReadAllText (@"./SampleData/dbo.ClassTours.data.sql");
            string classes = File.ReadAllText (@"./SampleData/dbo.Classes.data.sql");
            string departments = File.ReadAllText (@"./SampleData/dbo.Departments.data.sql");
            string people = File.ReadAllText (@"./SampleData/dbo.People.data.sql");
            string reasons = File.ReadAllText (@"./SampleData/dbo.Reasons.data.sql");
            string semesters = File.ReadAllText (@"./SampleData/dbo.Semesters.data.sql");
            string sessions = File.ReadAllText (@"./SampleData/dbo.Sessions.data.sql");
            string sessionReasons = File.ReadAllText (@"./SampleData/dbo.SessionReasons.data.sql");
            string sessionClasses = File.ReadAllText (@"./SampleData/dbo.SessionClasses.data.sql");

            context.Database.ExecuteSqlCommand (classTours);
            context.Database.ExecuteSqlCommand (departments);
            context.Database.ExecuteSqlCommand (classes);
            context.Database.ExecuteSqlCommand (people);
            context.Database.ExecuteSqlCommand (reasons);
            context.Database.ExecuteSqlCommand (semesters);
            context.Database.ExecuteSqlCommand (sessions);
            context.Database.ExecuteSqlCommand (sessionReasons);
            context.Database.ExecuteSqlCommand (sessionClasses);
        }
    }
}