using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CESManager.Controllers;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Tests.UnitTests.Controllers
{
    [TestFixture]
    [Category("UnitTests")]
    public class SessionControllerUnitTests
    {
        [Test]
        public async Task Get_HappyPath_ShouldReturnAllSessions()
        {
            // Arrange
            var endDate = DateTime.Now;
            var startDate = DateTime.Now.AddDays(-1);
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetAllSessions()).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>()
                    {
                        new GetSessionDto
                        {
                            EndDateTime = endDate,
                            StartDateTime = startDate,
                            Id = 1
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);

            // Act
            var result = await controller.Get();
            var okResult = result as OkObjectResult;
            var actualResponse = okResult.Value as ServiceResponse<List<GetSessionDto>>;

            // Assert
            Assert.AreEqual(typeof(OkObjectResult),result.GetType());
            Assert.AreEqual(1, actualResponse.Data.Count);
            Assert.AreEqual(1, actualResponse.Data.First().Id);
            Assert.AreEqual(startDate, actualResponse.Data.First().StartDateTime);
            Assert.AreEqual(endDate, actualResponse.Data.First().EndDateTime);
        }

        [Test]
        public async Task Get_SadPath_ShouldReturnBadRequest()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetAllSessions()).Returns(
                new ServiceResponse<List<GetSessionDto>>()
                {
                    Data = null
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.Get();

            //Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task GetSingle_HappyPath_ShouldReturnOK()
        {
            // Arrange
            var endDate = DateTime.Now;
            var startDate = DateTime.Now.AddDays(-1);
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetSessionById(1)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto()
                    {
                        StartDateTime = startDate,
                        EndDateTime = endDate,
                        Id = 1
                    }
                });
            var controller = new SessionController(mockSessionService);

            // Act
            var result = await controller.GetSingle(1);
            var okResult = result as OkObjectResult;
            var actualResponse = okResult.Value as ServiceResponse<GetSessionDto>;

            // Assert
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
            Assert.AreEqual(1, actualResponse.Data.Id);
            Assert.AreEqual(startDate, actualResponse.Data.StartDateTime);
            Assert.AreEqual(endDate, actualResponse.Data.EndDateTime);
        }

        [Test]
        public async Task GetSingle_HappyPath_ShouldReturnNotFound()
        {
            //Arrange
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetSessionById(1)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = null
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.GetSingle(1);

            //Assert
            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task AddSession_HappyPath_ShouldReturnOk()
        {
            //Arrange
            var endDate = DateTime.Now;
            var startDate = DateTime.Now.AddDays(-1);
            var newSession = new AddSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>
                    {
                        new GetSessionDto
                        {
                            Id = 1,
                            StartDateTime = startDate,
                            EndDateTime = endDate
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.AddSession(newSession);
            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as ServiceResponse<List<GetSessionDto>>;

            //Assert
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
            Assert.AreEqual(1, actualResult.Data[0].Id);
            Assert.AreEqual(startDate, actualResult.Data[0].StartDateTime);
            Assert.AreEqual(endDate, actualResult.Data[0].EndDateTime);
        }

        [Test]
        public async Task AddSession_HappyPath_ShouldReturnBadRequest()
        {
            //Arrange
            var newSession = new AddSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = null
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.AddSession(newSession);

            //Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSession_HappyPath_ShouldReturnOk()
        {
            //Arrange
            var endDate = DateTime.Now;
            var startDate = DateTime.Now.AddDays(-1);
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1,
                        StartDateTime = startDate,
                        EndDateTime = endDate
                    }
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.UpdateSession(updatedSession);
            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as ServiceResponse<GetSessionDto>;

            //Assert
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
            Assert.AreEqual(1, actualResult.Data.Id);
            Assert.AreEqual(startDate, actualResult.Data.StartDateTime);
            Assert.AreEqual(endDate, actualResult.Data.EndDateTime);
        }

        [Test]
        public async Task UpdateSession_HappyPath_ShouldReturnNotFound()
        {
            //Arrange
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = null,
                    Message = "Session not found."
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.UpdateSession(updatedSession);

            //Assert
            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSession_HappyPath_ShouldReturnBadRequest()
        {
            //Arrange
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Message = "EndDateTime cannot be earlier than StartDateTime."
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.UpdateSession(updatedSession);
            var badResult = result as BadRequestObjectResult;
            var actualResult = badResult.Value as string;

            //Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task Delete_HappyPath_ShouldReturnOk()
        {
            //Arrange
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now;
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.DeleteSession(2)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>()
                    {
                        new GetSessionDto
                        {
                            Id = 1,
                            StartDateTime = startDate,
                            EndDateTime = endDate
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.Delete(2);
            var okResult = result as OkObjectResult;
            var actualResult = okResult.Value as ServiceResponse<List<GetSessionDto>>;

            //Assert
            Assert.AreEqual(typeof(OkObjectResult),result.GetType());
            Assert.AreEqual(1, actualResult.Data[0].Id);
            Assert.AreEqual(startDate, actualResult.Data[0].StartDateTime);
            Assert.AreEqual(endDate, actualResult.Data[0].EndDateTime);
        }

        [Test]
        public async Task Delete_HappyPath_ShouldReturnNotFound()
        {
            //Arrange
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.DeleteSession(2)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = null
                });
            var controller = new SessionController(mockSessionService);

            //Act
            var result = await controller.Delete(2);

            //Assert
            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }
    }
}
