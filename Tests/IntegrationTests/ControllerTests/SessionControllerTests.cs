using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CESManager.Data;
using CESManager.Dtos.Session;
using CESManager.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.IntegrationTests.ControllerTests
{
    [TestFixture]
    public class SessionControllerTests
    {
        private string _bearerToken;
        private DataContext _dataContext;
        private List<GetSessionDto> _sessions;
        private string _urlGetAll = "https://localhost:44363/session/getall";
        private string _urlLogin = "https://localhost:44363/auth/login";
        private string _urlRegister = "https://localhost:44363/auth/register";
        private string _urlSession = "https://localhost:44363/session/";
        private HttpClient _httpClient;

        [SetUp]
        public async Task Setup()
        {
            _dataContext = CreateContext();

            Utility.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User { Username = "Sheila", PasswordHash = passwordHash, PasswordSalt = passwordSalt };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            _dataContext.Sessions.Add(new Session()
            {
                UserId = user.Id,
                StartDateTime = new DateTime(2020, 11, 19, 14, 0, 0),
                EndDateTime = new DateTime(2020, 11, 19, 14, 30, 0)
            });
            _dataContext.SaveChanges();

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);

            await GetBearerToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

            await GetSessions();
        }

        public DataContext CreateContext()
        {
            var builder = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer("Server=HXDTENGMS311\\SQLEXPRESS; Database=CESManagerTest; Trusted_Connection=true;");
            return new DataContext(builder.Options);
        }

        public async Task GetBearerToken()
        {
            var json = new
            {
                username = "Sheila",
                password = "123456",
            };

            var content = GetStringContent(json);
            var response = await _httpClient.PostAsync(_urlLogin, content);
            var bearerJson = await GetObjectFromResponse<ServiceResponse<string>>(response);
            _bearerToken = bearerJson.Data;
        }

        public async Task GetSessions()
        {
            var response = await _httpClient.GetAsync(_urlGetAll);
            var sessionId = await GetObjectFromResponse<ServiceResponse<List<GetSessionDto>>>(response);
            _sessions = sessionId.Data;
        }

        [Test]
        public async Task GetAllSessionsShouldReturn200Ok()
        {
            // Arrange
            var expectedDateTime = new DateTime(2020, 11, 19, 14, 30, 0);

            // Act
            var response = await _httpClient.GetAsync(_urlGetAll);
            var session = await GetObjectFromResponse<ServiceResponse<List<GetSessionDto>>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            session.Data.Count.Should().Be(1);
            session.Data.First().EndDateTime.Should().Be(expectedDateTime);
        }

        [Test]
        public async Task GetSessionByIdShouldReturn200Ok()
        {
            // Arrange
            var expectedDateTime = new DateTime(2020, 11, 19, 14, 30, 0);

            // Act
            var response = await _httpClient.GetAsync(_urlSession + _sessions.First().Id);
            var session = await GetObjectFromResponse<ServiceResponse<GetSessionDto>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            session.Data.EndDateTime.Should().Be(expectedDateTime);
        }

        [Test]
        public async Task IncorrectBearerTokenGetSessionByIdReturnsUnAuthorized()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

            // Act
            var response = await _httpClient.GetAsync(_urlSession + _sessions.First().Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetSessionByInvalidIdReturnsNotFound()
        {
            // Arrange
            int sessionId = 1;

            // Act
            var response = await _httpClient.GetAsync(_urlSession + sessionId);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task RegisterUserShouldReturn200Ok()
        {
            // Arrange
            var json = new
            {
                username = "Seb",
                password = "000000"
            };

            //Act
            var content = GetStringContent(json);
            var responsePost = await _httpClient.PostAsync(_urlRegister, content);

            // Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LoginUserShouldReturn200Ok()
        {
            // Arrange
            var json = new
            {
                username = "Sheila",
                password = "123456"
            };

            // Act
            var content = GetStringContent(json);
            var responsePost = await _httpClient.PostAsync(_urlLogin, content);

            // Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task RecordNewSessionShouldReturn200Ok()
        {
            // Arrange
            var json = new
            {
                startdatetime = "2020-11-30T12:00:00",
                enddatetime = "2020-11-30T12:30:00"
            };

            // Act
            var content = GetStringContent(json);
            var response = await _httpClient.PostAsync(_urlSession, content);
            var session = await GetObjectFromResponse<ServiceResponse<List<GetSessionDto>>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            session.Data.Count.Should().Be(2);
        }

        [Test]
        public async Task NegativeDurationRecordNewSessionShouldReturn400BadRequest()
        {
            // Arrange
            var json = new
            {
                startdatetime = "2020-11-30T12:00:00",
                enddatetime = "2020-11-30T11:30:00"
            };

            // Act
            var content = GetStringContent(json);
            var response = await _httpClient.PostAsync(_urlSession, content);
            var session = await GetObjectFromResponse<ServiceResponse<List<GetSessionDto>>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            session.Message.Should().Be("EndDateTime cannot be earlier than StartDateTime.");
        }

        [Test]
        public async Task UpdateSessionShouldReturn200Ok()
        {
            // Arrange
            var expectedDateTime = new DateTime(2020, 11, 30, 12, 15, 0);
            var json = new
            {
                id = _sessions.First().Id,
                startdatetime = "2020-11-30T12:00:00",
                enddatetime = "2020-11-30T12:15:00"
            };

            // Act
            var content = GetStringContent(json);
            var response = await _httpClient.PutAsync(_urlSession, content);
            var session = await GetObjectFromResponse<ServiceResponse<GetSessionDto>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            session.Data.EndDateTime.Should().Be(expectedDateTime);
        }

        [Test]
        public async Task UpdateSessionByInvalidIdReturnsNotFound()
        {
            // Arrange
            var json = new
            {
                id = 1,
                startdatetime = "2020-11-30T12:00:00",
                enddatetime = "2020-11-30T12:15:00"
            };

            // Act
            var content = GetStringContent(json);
            var response = await _httpClient.PutAsync(_urlSession, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task NegativeDurationUpdateSessionShouldReturn400BadRequest()
        {
            // Arrange
            var json = new
            {
                id = _sessions.First().Id,
                startdatetime = "2020-11-30T12:00:00",
                enddatetime = "2020-11-30T11:15:00"
            };

            // Act
            var content = GetStringContent(json);
            var response = await _httpClient.PutAsync(_urlSession, content);
            var session = await GetObjectFromResponse<ServiceResponse<GetSessionDto>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            session.Message.Should().Be("EndDateTime cannot be earlier than StartDateTime.");
        }

        [Test]
        public async Task DeleteSessionShouldReturn200Ok()
        {
            // Act
            var responsePost = await _httpClient.DeleteAsync(_urlSession + _sessions.First().Id);

            // Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext = CreateContext();

            var sessions = _dataContext.Sessions;
            if (sessions.Any())
                _dataContext.Sessions.RemoveRange(sessions);
            _dataContext.SaveChanges();

            var users = _dataContext.Users;
            _dataContext.Users.RemoveRange(users);
            _dataContext.SaveChanges();
        }

        private StringContent GetStringContent(dynamic payload)
        {
            var body = JsonConvert.SerializeObject(payload, Formatting.None);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return content;
        }

        private async Task<T> GetObjectFromResponse<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}