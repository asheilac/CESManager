using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CESManager.Controllers;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using FakeItEasy;
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
        private ISessionService _mockSessionService;
        private SessionController _controller;
        private const int AuthorisedUser = 42;

        [SetUp]
        public void Setup()
        {
            var fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => fakeHttpContextAccessor.HttpContext.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.NameIdentifier, AuthorisedUser.ToString())})));

            _mockSessionService = A.Fake<ISessionService>();
            _controller = new SessionController(_mockSessionService, fakeHttpContextAccessor);
        }

        [Test]
        public async Task GetAllShouldReturnOKWhenSessionExists()
        {
            A.CallTo(() => _mockSessionService.GetAllSessions(AuthorisedUser)).Returns(
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
            var result = await _controller.Get();

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task GetAllShouldReturnExpectedNumberOfSessionsWhenSessionExists()
        {
            A.CallTo(() => _mockSessionService.GetAllSessions(AuthorisedUser)).Returns(
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

            var result = await _controller.Get();
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
            A.CallTo(() => _mockSessionService.GetAllSessions(AuthorisedUser)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var result = await _controller.Get();

            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task GetAllSessionShouldReturnExpectedErrorMessageWhenSessionDoesNotExist()
        {
            A.CallTo(() => _mockSessionService.GetAllSessions(AuthorisedUser))
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Could not find sessions."
                });
            var result = await _controller.Get();
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Could not find sessions."));
        }

        [Test]
        public async Task GetSingleShouldReturnOKWhenSessionExists()
        {
            A.CallTo(() => _mockSessionService.GetSessionById(1, AuthorisedUser)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1
                    }
                });
            var result = await _controller.Get();

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task GetSingleShouldReturnDataWhenSessionExists()
        {
            A.CallTo(() => _mockSessionService.GetSessionById(1, AuthorisedUser)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1
                    }
                });
            var result = await _controller.GetSingle(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSingleShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            A.CallTo(() => _mockSessionService.GetSessionById(1, AuthorisedUser)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var result = await _controller.GetSingle(1);

            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task GetSingleShouldReturnExpectedErrorMessageWhenSessionNotFound()
        {
            A.CallTo(() => _mockSessionService.GetSessionById(1, AuthorisedUser))
                .Returns(new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Session not found."
                });
            var result = await _controller.GetSingle(1);
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Session not found."));
        }

        [Test]
        public async Task AddSessionShouldReturnOkWhenSessionExists()
        {
            var newSession = new AddSessionDto();
            A.CallTo(() => _mockSessionService.AddSession(newSession)).Returns(
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
            var result = await _controller.AddSession(newSession);

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task AddSessionShouldReturnDataWhenSessionExists()
        {
            var newSession = new AddSessionDto();
            A.CallTo(() => _mockSessionService.AddSession(newSession)).Returns(
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
            var result = await _controller.AddSession(newSession);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task AddSessionShouldReturnBadRequestWhenSessionDurationIsNegative()
        {
            var newSession = new AddSessionDto();
            A.CallTo(() => _mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration
                });
            var result = await _controller.AddSession(newSession);

            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task AddSessionShouldReturnExpectedErrorMessageWhenSessionDurationIsNegative()
        {
            var newSession = new AddSessionDto();
            A.CallTo(() => _mockSessionService.AddSession(newSession)).Returns(
                new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration,
                    Message = "EndDateTime cannot be earlier than StartDateTime."
                });
            var result = await _controller.AddSession(newSession);
            var objectResult = (BadRequestObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("EndDateTime cannot be earlier than StartDateTime."));
        }

        [Test]
        public async Task UpdateSessionShouldReturnOKWhenSessionExists()
        {
            var updatedSession = new UpdateSessionDto();
            A.CallTo(() => _mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1
                    }
                });
            var result = await _controller.UpdateSession(updatedSession);

            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSessionShouldReturnDataWhenSessionExists()
        {
            var updatedSession = new UpdateSessionDto();
            A.CallTo(() => _mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    Data = new GetSessionDto
                    {
                        Id = 1
                    }
                });
            var result = await _controller.UpdateSession(updatedSession);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task UpdateSessionShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            var updatedSession = new UpdateSessionDto();
            A.CallTo(() => _mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var result = await _controller.UpdateSession(updatedSession);

            Assert.AreEqual(typeof(NotFoundObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSessionShouldReturnExpectedErrorMessageWhenSessionDoesNotExist()
        {
            var updatedSession = new UpdateSessionDto();
            A.CallTo(() => _mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Could not find session to update."
                });
            var result = await _controller.UpdateSession(updatedSession);
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Could not find session to update."));
        }

        [Test]
        public async Task UpdateSessionShouldReturnBadRequestWhenSessionDurationIsNegative()
        {
            var updatedSession = new UpdateSessionDto();
            A.CallTo(() => _mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration
                });
            var result = await _controller.UpdateSession(updatedSession);

            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task UpdateSessionShouldReturnExpectedErrorMessageWhenSessionDurationIsNegative()
        {
            var updatedSession = new UpdateSessionDto();
            A.CallTo(() => _mockSessionService.UpdateSession(updatedSession)).Returns(
                new ServiceResponse<GetSessionDto>
                {
                    StatusCode = CESManagerStatusCode.NegativeDuration,
                    Message = "EndDateTime cannot be earlier than StartDateTime."
                });
            var result = await _controller.UpdateSession(updatedSession);
            var objectResult = (BadRequestObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("EndDateTime cannot be earlier than StartDateTime."));
        }

        [Test]
        public async Task DeleteSessionShouldReturnNotFoundWhenSessionDoesNotExist()
        {
            A.CallTo(() => _mockSessionService.DeleteSession(1, AuthorisedUser))
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound
                });
            var result = (IStatusCodeActionResult) await _controller.Delete(1);

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task DeleteSessionShouldReturnExpectedErrorMessageWhenSessionDoesNotExist()
        {
            A.CallTo(() => _mockSessionService.DeleteSession(1, AuthorisedUser))
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    StatusCode = CESManagerStatusCode.SessionNotFound,
                    Message = "Could not find session to Delete."
                });
            var result = await _controller.Delete(1);
            var objectResult = (NotFoundObjectResult) result;

            Assert.That(objectResult.Value, Is.EqualTo("Could not find session to Delete."));
        }

        [Test]
        public async Task DeleteSessionShouldReturnOKWhenSessionDoesExist()
        {
            A.CallTo(() => _mockSessionService.DeleteSession(1, AuthorisedUser))
                .Returns(new ServiceResponse<List<GetSessionDto>>
                {
                    Data = new List<GetSessionDto>
                    {
                        new GetSessionDto
                        {
                            Id = 1
                        }
                    }
                });
            var result = (IStatusCodeActionResult) await _controller.Delete(1);

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task DeleteSessionShouldReturnDataWhenSessionDoesExist()
        {
            A.CallTo(() => _mockSessionService.DeleteSession(1, AuthorisedUser))
                .Returns(
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
            var result = await _controller.Delete(1);

            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }
    }
}