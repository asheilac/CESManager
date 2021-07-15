using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Services
{
    [TestFixture]
    [Category("UnitTests")]
    public class SessionServiceUnitTests
    {
        [Test]
        public async Task UpdateSession_ShouldReturn500InternalServerError()
        {
            //Arrange
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });
            
            //Act
            var result = await mockSessionService.UpdateSession(updatedSession);
            
            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
