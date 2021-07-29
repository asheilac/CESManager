using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        private SessionService _sut;
        private User _validUser;
        private Session _existingSession;
        private DataContext _dbContext;
        
        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.CreateMap<Session, GetSessionDto>();
                opts.CreateMap<AddSessionDto, Session>();
            });
            var mapper = config.CreateMapper(); 

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("SessionUnitTestDatabase")
                .Options;
            _dbContext = new DataContext(options);

            _validUser = MakeUser("TestUser");

            _existingSession = new Session()
            {
                UserId = _validUser.Id,
                StartDateTime = new DateTime(2020, 11, 19, 14, 0, 0),
                EndDateTime = new DateTime(2020, 11, 19, 14, 30, 0)
            };
            _dbContext.Sessions.Add(_existingSession);
            _dbContext.SaveChanges();

            _sut = new SessionService(mapper, _dbContext);
        }

        public User MakeUser(string username)
        {
            var user = new User() { Username = username };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }

        [Test]
        public async Task DeleteSession_ReturnsNotFound_WhenSessionDoesNotExist()
        {
            var result = await _sut.DeleteSession(1, 42);

            result.StatusCode.Should().Be(CESManagerStatusCode.SessionNotFound);
        }

        [Test]
        public async Task DeleteSession_ReturnsExpectedErrorMessage_WhenSessionDoesNotExist()
        {
            var result = await _sut.DeleteSession(1, 42);

            result.Message.Should().Be("Could not find session to Delete.");
        }

        [Test]
        public async Task DeleteSession_ReturnsOK_WhenSessionExists()
        {
            var result = await _sut.DeleteSession(1, 1);

            result.StatusCode.Should().Be(CESManagerStatusCode.Ok);
        }

        [Test]
        public async Task AddSession_ReturnsNegativeDuration_WhenDurationIsNegative()
        {
            var session = new AddSessionDto()
            {
                StartDateTime = new DateTime(2020, 12, 1, 14, 30, 0),
                EndDateTime = new DateTime(2020, 12, 1, 14, 00, 0)
            };
            var result = await _sut.AddSession(session);

            result.StatusCode.Should().Be(CESManagerStatusCode.NegativeDuration);
        }

        [Test]
        public async Task AddSession_ReturnsExpectedErrorMessage_WhenDurationIsNegative()
        {
            var session = new AddSessionDto()
            {
                StartDateTime = new DateTime(2020, 12, 1, 14, 30, 0),
                EndDateTime = new DateTime(2020, 12, 1, 14, 00, 0)
            };
            var result = await _sut.AddSession(session);

            result.Message.Should().Be("EndDateTime cannot be earlier than StartDateTime.");
        }

        [Test]
        public async Task AddSession_ReturnsOK_WhenDurationIsPositive()
        {
            var session = new AddSessionDto()
            {
                UserId = _validUser.Id,
                StartDateTime = new DateTime(2020, 12, 1, 14, 00, 0),
                EndDateTime = new DateTime(2020, 12, 1, 14, 30, 0)
            };
            var result = await _sut.AddSession(session);

            result.StatusCode.Should().Be(CESManagerStatusCode.Ok);
        }

        [Test]
        public async Task GetAllSessions_ReturnsNotFound_WhenSessionsNotFound()
        {
            var newUser = MakeUser("NewUser");

            var result = await _sut.GetAllSessions(newUser.Id);

            result.StatusCode.Should().Be(CESManagerStatusCode.SessionNotFound);
        }

        [Test]
        public async Task GetAllSessions_ReturnsExpectedErrorMessage_WhenSessionsNotFound()
        {
            var newUser = MakeUser("NewUser");

            var result = await _sut.GetAllSessions(newUser.Id);

            result.Message.Should().Be("Could not find sessions.");
        }

        [Test]
        public async Task GetAllSessions_ReturnsOK_WhenSessionsExist()
        {
            var result = await _sut.GetAllSessions(_validUser.Id);

            result.StatusCode.Should().Be(CESManagerStatusCode.Ok);
        }


        [Test]
        public async Task GetSessionById_ReturnsNotFound_WhenSessionNotFound()
        {
            var result = await _sut.GetSessionById(42, _validUser.Id);

            result.StatusCode.Should().Be(CESManagerStatusCode.SessionNotFound);
        }

        [Test]
        public async Task GetSessionById_ReturnsExpectedErrorMessage_WhenSessionNotFound()
        {
            var result = await _sut.GetSessionById(42, _validUser.Id);

            result.StatusCode.Should().Be(CESManagerStatusCode.SessionNotFound);
        }

        [Test]
        public async Task GetSessionById_ReturnsOK_WhenSessionExists()
        {
            var result = await _sut.GetSessionById(_existingSession.Id,_validUser.Id);

            result.StatusCode.Should().Be(CESManagerStatusCode.Ok);
        }

        [Test]
        public async Task UpdateSession_ReturnsNegativeDuration_WhenSessionDurationIsNegative()
        {
            var session = new UpdateSessionDto()
            {
                UserId = _validUser.Id,
                Id = _existingSession.Id,
                StartDateTime = new DateTime(2020, 11, 19, 14, 0, 0),
                EndDateTime = new DateTime(2020, 11, 19, 13, 30, 0)
            };
            var result = await _sut.UpdateSession(session);

            result.StatusCode.Should().Be(CESManagerStatusCode.NegativeDuration);
        }

        [Test]
        public async Task UpdateSession_ReturnsExpectedErrorMessage_WhenSessionDurationIsNegative()
        {
            var session = new UpdateSessionDto()
            {
                UserId = _validUser.Id,
                Id = _existingSession.Id,
                StartDateTime = new DateTime(2020, 11, 19, 14, 0, 0),
                EndDateTime = new DateTime(2020, 11, 19, 13, 30, 0)
            };
            var result = await _sut.UpdateSession(session);

            result.Message.Should().Be("EndDateTime cannot be earlier than StartDateTime.");
        }

        [Test]
        public async Task UpdateSession_ReturnsNotFound_WhenSessionDoesNotExist()
        {
            var session = new UpdateSessionDto();
            var result = await _sut.UpdateSession(session);

            result.StatusCode.Should().Be(CESManagerStatusCode.SessionNotFound);
        }

        [Test]
        public async Task UpdateSession_ReturnsExpectedErrorMessage_WhenSessionDoesNotExist()
        {
            var session = new UpdateSessionDto();
            var result = await _sut.UpdateSession(session);

            result.Message.Should().Be("Could not find session to update.");
        }

        [Test]
        public async Task UpdateSession_ReturnsOK_WhenSessionExists()
        {
            var session = new UpdateSessionDto()
            {
                UserId = _validUser.Id,
                Id = _existingSession.Id,
                StartDateTime = new DateTime(2020, 11, 19, 13, 0, 0),
                EndDateTime = new DateTime(2020, 11, 19, 13, 30, 0)
            };
            var result = await _sut.UpdateSession(session);

            result.StatusCode.Should().Be(CESManagerStatusCode.Ok);
        }
    }
}