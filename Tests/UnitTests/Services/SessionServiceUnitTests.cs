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
        [Test]
        public async Task DeleteSession_ReturnsNotFound_WhenSessionDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("SessionUnitTestDatabase")
                .Options;
            await using var dbContext = new DataContext(options);

            var sut = new SessionService(A.Fake<IMapper>(), dbContext);
            var result = await sut.DeleteSession(1, 42);

            result.StatusCode.Should().Be(CESManagerStatusCode.SessionNotFound);
        }
    }
}