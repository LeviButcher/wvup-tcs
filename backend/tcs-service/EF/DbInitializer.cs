using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.EF
{
    public class DbInitializer
    {
        public async static Task InitializeData(TCSContext context, IUserRepo userRepo, IHostingEnvironment env)
        {
            await context.Database.MigrateAsync();
            if (env.IsDevelopment())
            {
                await ClearData(context);
                await SeedData(context);
            }
            await SeedAdmin(context, userRepo);
        }

        private async static Task ClearData(TCSContext context)
        {
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[ClassTours]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[SignInReasons]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[SignInCourses]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[SignIns]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Courses]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Departments]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Reasons]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[People]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Semesters]");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Users]");
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

        private async static Task SeedData(TCSContext context)
        {
            string classTours = await File.ReadAllTextAsync(@"./SampleData/dbo.ClassTours.data.sql");
            string courses = await File.ReadAllTextAsync(@"./SampleData/dbo.Courses.data.sql");
            string departments = await File.ReadAllTextAsync(@"./SampleData/dbo.Departments.data.sql");
            string people = await File.ReadAllTextAsync(@"./SampleData/dbo.People.data.sql");
            string reasons = await File.ReadAllTextAsync(@"./SampleData/dbo.Reasons.data.sql");
            string semesters = await File.ReadAllTextAsync(@"./SampleData/dbo.Semesters.data.sql");
            string signIns = await File.ReadAllTextAsync(@"./SampleData/dbo.SignIns.data.sql");
            string signInReasons = await File.ReadAllTextAsync(@"./SampleData/dbo.SignInReasons.data.sql");
            string signInCourses = await File.ReadAllTextAsync(@"./SampleData/dbo.SignInCourses.data.sql");


            await context.Database.ExecuteSqlCommandAsync(classTours);
            await context.Database.ExecuteSqlCommandAsync(departments);
            await context.Database.ExecuteSqlCommandAsync(courses);
            await context.Database.ExecuteSqlCommandAsync(people);
            await context.Database.ExecuteSqlCommandAsync(reasons);
            await context.Database.ExecuteSqlCommandAsync(semesters);
            await context.Database.ExecuteSqlCommandAsync(signIns);
            await context.Database.ExecuteSqlCommandAsync(signInReasons);
            await context.Database.ExecuteSqlCommandAsync(signInCourses);
        }
    }
}
