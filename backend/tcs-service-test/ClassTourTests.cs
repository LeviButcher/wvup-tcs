using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.Controllers;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service.Repos.Interfaces;
using Xunit;

namespace tcs_service_test
{
    public class ClassTourTests
    {
        [Fact]
        public async void DisasterTest()
        {
            var dbContextMock = new Mock<TCSContext>();
            var tourMock = new Mock<DbSet<ClassTour>>();

            var fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

            var tours = new List<ClassTour>();
            var lockedUser = fixture.Build<ClassTour>().With(u => u.Name, "Harry").Create();
            tours.Add(lockedUser);

            var users = tours.AsQueryable();
            var classTour = fixture.Create<ClassTour>();


            tourMock.As<IQueryable<ClassTour>>().Setup(m => m.Provider).Returns(users.Provider);
            tourMock.As<IQueryable<ClassTour>>().Setup(m => m.Expression).Returns(users.Expression);
            tourMock.As<IQueryable<ClassTour>>().Setup(m => m.ElementType).Returns(users.ElementType);
            tourMock.As<IQueryable<ClassTour>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            dbContextMock.Setup(x => x.ClassTours).Returns(tourMock.Object);

            

            var ClassTourRepo = new ClassTourRepo(dbContextMock.Object);

            var harry = await ClassTourRepo.Find(lockedUser.ID);

            Assert.Equal(harry.Name, lockedUser.Name);
        }
        [Fact]
        public async void AddingNewTour()
        {
            var fixture = new Fixture()
             .Customize(new AutoMoqCustomization());

            var classTour = fixture.Create<ClassTour>();
         
            var repository = fixture.Create<Mock<IClassTourRepo>>();

            await repository.Object.Add(classTour);
           
            
            var tour = await repository.Object.Exist(classTour.ID);
                       
            
            Assert.Equal(classTour.Name, classTour.Name );
          
        }


        
    }
}
