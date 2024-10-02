using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieBookingApp.Controllers;
using MovieBookingApp.Models;
//using MovieBookingApp.Services;
using Moq;
using NUnit.Framework;
using MovieBookingApp.Interfaces.IBusiness;

namespace MovieBookingAppTests
{
    public class MovieBookingControllerTests
    {
        private MovieBookingController _controller;
        private Mock<IUserBusiness> _userBusinessMock;
        private Mock<IMovieBusiness> _movieBusinessMock;
        private Mock<ITicketBusiness> _ticketBusinessMock;

        [SetUp]
        public void Setup()
        {
            _userBusinessMock = new Mock<IUserBusiness>();
            _movieBusinessMock = new Mock<IMovieBusiness>();
            _ticketBusinessMock = new Mock<ITicketBusiness>();

            _controller = new MovieBookingController(_userBusinessMock.Object, _movieBusinessMock.Object, _ticketBusinessMock.Object);
        }

        [Test]
        public async Task Register_ValidUser_ReturnsCreatedResult()
        {
            // Arrange
            var userDto = new UserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                LoginId = "john.doe",
                Password = "password",
                Contact = "1234567890"
            };
            _userBusinessMock.Setup(x => x.AddUser(userDto)).ReturnsAsync("12345");

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsInstanceOf<CreatedResult>(result);
            var createdResult = result as CreatedResult;
            Assert.AreEqual("12345", createdResult.Value);
        }

        [Test]
        public async Task Register_ExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                LoginId = "john.doe",
                Password = "password",
                Contact = "1234567890"
            };
            _userBusinessMock.Setup(x => x.AddUser(userDto)).ReturnsAsync(string.Empty);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            string loginId = "testuser";
            string password = "password123";
            string expectedToken = "sample_token";

            _userBusinessMock.Setup(mock => mock.GetUserToken(loginId, password))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _controller.Login(loginId, password);

            // Assert
            var okObjectResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);
            Assert.AreEqual(expectedToken, okObjectResult.Value);
        }

        [Test]
        public async Task Forgot_ValidLoginIdAndNewPassword_ReturnsPasswordChangedStatus()
        {
            // Arrange
            string loginId = "testuser";
            string newPassword = "newpassword";
            string expectedPasswordChangedStatus = "Password changed successfully";

            _userBusinessMock.Setup(mock => mock.ChangePassword(loginId, newPassword))
                .ReturnsAsync(expectedPasswordChangedStatus);

            // Act
            var result = await _controller.Forgot(loginId, newPassword);

            // Assert
            var okObjectResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);
            Assert.AreEqual(expectedPasswordChangedStatus, okObjectResult.Value);
        }

        [Test]
        public async Task ViewAllMovies_ReturnsListOfMovies()
        {
            // Arrange
            List<MovieDto> expectedMovies = new List<MovieDto>
            {
                new MovieDto { Name = "Movie 1", TheatreName = "Theatre 1", IsAvailable = true },
                new MovieDto { Name = "Movie 2", TheatreName = "Theatre 2", IsAvailable = false }
            };

            _movieBusinessMock.Setup(mock => mock.GetMovies())
                .ReturnsAsync(expectedMovies);

            // Act
            var result = await _controller.ViewAllMovies();

            // Assert
            var okObjectResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);

            var movies = okObjectResult.Value as List<MovieDto>;
            Assert.NotNull(movies);
            Assert.AreEqual(expectedMovies.Count, movies.Count);
        }

        [Test]
        public async Task SearchMovie_ReturnsMatchingMovies()
        {
            // Arrange
            string movieName = "Movie 1";
            List<MovieDto> expectedMovies = new List<MovieDto>
            {
                new MovieDto { Name = "Movie 1", TheatreName = "Theatre 1", IsAvailable = true },
                new MovieDto { Name = "Movie 1 - Sequel", TheatreName = "Theatre 2", IsAvailable = false }
            };

            _movieBusinessMock.Setup(mock => mock.SearchMovie(movieName))
                .ReturnsAsync(expectedMovies);

            // Act
            var result = await _controller.SearchMovie(movieName);

            // Assert
            var okObjectResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);

            var movies = okObjectResult.Value as List<MovieDto>;
            Assert.NotNull(movies);
            Assert.AreEqual(expectedMovies.Count, movies.Count);
        }

        [Test]
        public async Task AddTickets_ValidTicket_ReturnsOkResult()
        {
            // Arrange
            TicketDto ticket = new TicketDto
            {
                MovieName = "Movie 1",
                TheatreName = "Theatre 1",
                NumberOfTickets = 2,
                SeatNumber = "A1,A2"
            };

            _ticketBusinessMock.Setup(mock => mock.AddTicket(ticket))
                .ReturnsAsync("Ticket booked successfully");

            // Act
            var result = await _controller.AddTickets(ticket);

            // Assert
            var okObjectResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);

            var status = okObjectResult.Value as string;
            Assert.NotNull(status);
            Assert.AreEqual("Ticket booked successfully", status);
        }

        [Test]
        public async Task UpdateMovieTicketStatus_ValidTicket_ReturnsOkResult()
        {
            // Arrange
            string movieName = "Movie 1";
            string ticket = "Ticket 123";

            var response = new TicketStatusResponse
            {
                IsSuccess = true,
                Message = "Movie ticket status updated"
            };

            _movieBusinessMock.Setup(mock => mock.UpdateMovieTicketStatus(movieName, ticket))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateMovieTicketStatus(movieName, ticket);

            // Assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);

            var responseData = okObjectResult.Value as TicketStatusResponse;
            Assert.NotNull(responseData);
            Assert.AreEqual(response.IsSuccess, responseData.IsSuccess);
            Assert.AreEqual(response.Message, responseData.Message);
        }

        [Test]
        public async Task DeleteRecordById_ValidId_ReturnsOkResult()
        {
            // Arrange
            string movieName = "Movie 1";
            string id = "123";

            var response = new DeleteMovieByNameAndIdResponse
            {
                IsSuccess = true,
                Message = $"Movie {movieName} deleted successfully"
            };

            _movieBusinessMock.Setup(mock => mock.DeleteMovieByNameAndId(movieName, id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteMovieById(movieName, id);

            // Assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.AreEqual(200, okObjectResult.StatusCode);

            var responseData = okObjectResult.Value as DeleteMovieByNameAndIdResponse;
            Assert.NotNull(responseData);
            Assert.AreEqual(response.IsSuccess, responseData.IsSuccess);
            Assert.AreEqual(response.Message, responseData.Message);
        }
    }
}