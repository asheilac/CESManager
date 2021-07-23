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
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NUnit.Framework;

namespace Tests.UnitTests.Controllers
{
    [TestFixture]
    [Category("UnitTests")]
    public class SessionControllerUnitTests
    {
        [Test]
        public async Task GetAllShouldReturnOKWhenSessionExists()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetAllSessions()).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>()
                    {
                        new GetSessionDto
                        {
                            Id = 1
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.Get();

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task GetAllShouldReturnExpectedNumberOfSessionsWhenSessionExists()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetAllSessions()).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>()
                    {
                        new GetSessionDto
                        {
                            Id = 1
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);

            var result = await controller.Get();
            var okResult = (OkObjectResult) result;
            var actualResponse = (List<GetSessionDto>) okResult.Value;

            Assert.That(actualResponse.Count, Is.EqualTo(1));
            //Assert.AreEqual(1, actualResponse.Count);
            //actualResponse.Count.Should().Be(1);
            //actualResponse.Should().HaveCount(1);
        }

        [Test]
        public async Task GetAllShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetAllSessions()).Returns(
                new ServiceResponse<List<GetSessionDto>>()
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.Get();

            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task GetAllSessionShouldReturnExpectedErrorMessageWhenSessionDoesNotExist()
        {
            var fakeSessionService = A.Fake<ISessionService>();
            A.CallTo(() => fakeSessionService.GetAllSessions())
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Could not find sessions."
                });
            var controller = new SessionController(fakeSessionService);
            var result = await controller.Get();
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Could not find sessions."));
        }

        [Test]
        public async Task GetSingleShouldReturnOKWhenSessionExists()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetSessionById(1)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto()
                    {
                        Id = 1
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.Get();

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task GetSingleShouldReturnDataWhenSessionExists()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetSessionById(1)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto()
                    {
                        Id = 1
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.GetSingle(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSingleShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetSessionById(1)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.GetSingle(1);

            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task GetSingleShouldReturnExpectedErrorMessageWhenSessionNotFound()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.GetSessionById(1))
                .Returns(new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Session not found."
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.GetSingle(1);
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Session not found."));
        }

        [Test]
        public async Task AddSessionShouldReturnOkWhenSessionExists()
        {
            var newSession = new AddSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>
                    {
                        new GetSessionDto
                        {
                            Id = 1
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.AddSession(newSession);

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task AddSessionShouldReturnDataWhenSessionExists()
        {
            var newSession = new AddSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>
                    {
                        new GetSessionDto
                        {
                            Id = 1
                        }
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.AddSession(newSession);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task AddSessionShouldReturnBadRequestWhenSessionDurationIsNegative()
        {
            var newSession = new AddSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.AddSession(newSession);

            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task AddSessionShouldReturnExpectedErrorMessageWhenSessionDurationIsNegative()
        {
            var newSession = new AddSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration,
                    Message = "EndDateTime cannot be earlier than StartDateTime."
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.AddSession(newSession);
            var objectResult = (BadRequestObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("EndDateTime cannot be earlier than StartDateTime."));
        }

        [Test]
        public async Task UpdateSessionShouldReturnOKWhenSessionExists()
        {
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.UpdateSession(updatedSession);

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSessionShouldReturnDataWhenSessionExists()
        {
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1
                    }
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.UpdateSession(updatedSession);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task UpdateSessionShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.UpdateSession(updatedSession);

            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSessionShouldReturnExpectedErrorMessageWhenSessionDoesNotExist()
        {
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Could not find session to update."
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.UpdateSession(updatedSession);
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Could not find session to update."));
        }

        [Test]
        public async Task UpdateSessionShouldReturnBadRequestWhenSessionDurationIsNegative()
        {
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.UpdateSession(updatedSession);

            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSessionShouldReturnExpectedErrorMessageWhenSessionDurationIsNegative()
        {
            var updatedSession = new UpdateSessionDto();
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration,
                    Message = "EndDateTime cannot be earlier than StartDateTime."
                });
            var controller = new SessionController(mockSessionService);
            var result = await controller.UpdateSession(updatedSession);
            var objectResult = (BadRequestObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("EndDateTime cannot be earlier than StartDateTime."));
        }

        [Test]
        public async Task DeleteSessionShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            var fakeSessionService = A.Fake<ISessionService>();
            A.CallTo(() => fakeSessionService.DeleteSession(A<int>.Ignored))
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var controller = new SessionController(fakeSessionService);
            var result = (IStatusCodeActionResult) await controller.Delete(1);

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task DeleteSessionShouldReturnExpectedErrorMessageWhenSessionDoesNotExist()
        {
            var fakeSessionService = A.Fake<ISessionService>();
            A.CallTo(() => fakeSessionService.DeleteSession(A<int>.Ignored))
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Could not find session to Delete."
                });
            var controller = new SessionController(fakeSessionService);
            var result = await controller.Delete(1);
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Could not find session to Delete."));
        }

        [Test]
        public async Task DeleteSessionShouldReturnOKWhenSessionDoesExist()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.DeleteSession(A<int>.Ignored))
                .ReturnsLazily((int sessionId) =>
                    new ServiceResponse<List<GetSessionDto>>
                    {
                        Data = new List<GetSessionDto>()
                        {
                            new GetSessionDto
                            {
                                Id = sessionId
                            }
                        }
                    });
            var controller = new SessionController(mockSessionService);
            var result = (IStatusCodeActionResult) await controller.Delete(1);

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task DeleteSessionShouldReturnDataWhenSessionDoesExist()
        {
            var mockSessionService = A.Fake<ISessionService>();
            A.CallTo(() => mockSessionService.DeleteSession(A<int>.Ignored))
                .ReturnsLazily((int sessionId) =>
                    new ServiceResponse<List<GetSessionDto>>
                    {
                        Data = new List<GetSessionDto>()
                        {
                            new GetSessionDto
                            {
                                Id = sessionId
                            }
                        }
                    });
            var controller = new SessionController(mockSessionService);
            var result = await controller.Delete(1);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }
    }
}