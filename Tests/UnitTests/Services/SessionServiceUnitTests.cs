using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CESManager.Data;
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

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("SessionUnitTestDatabase")
                .Options;
            var dbContext = new DataContext(options);

             _sut = new SessionService(A.Fake<IMapper>(), dbContext);
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

        }

    }
}