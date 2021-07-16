using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CESManager.Data;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.UnitTests.Services
{
    [TestFixture]
    [Category("UnitTests")]
    public class SessionServiceUnitTests
    {
        [Test]
        public async Task UpdateSession_HappyPath_GivenValidSessionShouldReturnUpdatedResponseData()
        {
            //Arrange
            var fakeMapper = A.Fake<IMapper>();
            var fakeAccesor = A.Fake<IHttpContextAccessor>();


            IQueryable<Session> fakeIQueryable = new List<Session>()
            {
                new Session { Id = 1 }
            }.AsQueryable();

            var fakeDbSet = A.Fake<DbSet<Session>>((d =>
                d.Implements(typeof(IQueryable<Session>))));

            A.CallTo(() => ((IQueryable<Session>)fakeDbSet).GetEnumerator())
                .Returns(fakeIQueryable.GetEnumerator());
            A.CallTo(() => ((IQueryable<Session>)fakeDbSet).Provider)
                .Returns(fakeIQueryable.Provider);
            A.CallTo(() => ((IQueryable<Session>)fakeDbSet).Expression)
                .Returns(fakeIQueryable.Expression);
            A.CallTo(() => ((IQueryable<Session>)fakeDbSet).ElementType)
                .Returns(fakeIQueryable.ElementType);

            var fakeContext = A.Fake<DataContext>();

            A.CallTo(() => fakeContext.Sessions).Returns(fakeDbSet);

            var sessionService = new SessionService(fakeMapper, fakeContext, fakeAccesor);

            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddSeconds(1);

            //Act
            var results = sessionService.UpdateSession(new UpdateSessionDto
            {
                Id = 1,
                StartDateTime = startTime,
                EndDateTime = endTime
            });

            //Assert
            Assert.AreEqual(results.Result.Data.StartDateTime,startTime);
            Assert.AreEqual(results.Result.Data.EndDateTime,endTime);
        }
        [Test]
        public async Task UpdateSession_SadPath_GivenValidSessionAndExceptionThrownShouldReturnServerError()
        {
            //Arrange
            //Act
            //Assert
        }
        [Test]
        public async Task UpdateSession_SadPath_GivenInvalidDurationShouldReturnExpectedErrorMessage()
        {
            //Arrange
            //Act
            //Assert
        }
        [Test]
        public async Task UpdateSession_SadPath_GivenInvalidIdShouldReturnExpectedErrorMessage()
        {
            //Arrange
            //Act
            //Assert
        }
    }
}
