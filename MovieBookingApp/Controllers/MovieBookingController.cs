using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingApp.Filters;
using MovieBookingApp.Interfaces.IBusiness;
using MovieBookingApp.Models;
using System.Net.Sockets;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovieBookingController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IMovieBusiness _movieBusiness;
        private readonly ITicketBusiness _ticketBusiness;

        public MovieBookingController(IUserBusiness userBusiness, IMovieBusiness movieBusiness, ITicketBusiness ticketBusiness)
        {
            _userBusiness = userBusiness;
            _movieBusiness = movieBusiness;
            _ticketBusiness = ticketBusiness;
        }

        [HttpPost("Register"), AllowAnonymous]
        [ServiceFilter(typeof(NullCheckFilter))]
        public async Task<ActionResult> Register(UserDto user)
        {
            var userId = await _userBusiness.AddUser(user);

            if (!string.IsNullOrEmpty(userId))
            {
                return Created("", userId);
            }
            else
            {
                return BadRequest("User already exists");
            }
        }

        [HttpGet("Login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login(string loginId, string password)
        {
            var token = await _userBusiness.GetUserToken(loginId, password);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(token);
            }
            else
            {
                return BadRequest("Incorrect LoginId or Password");
            }
        }

        [HttpGet("{loginId}/Forgot"), AllowAnonymous]
        public async Task<ActionResult<string>> Forgot(string loginId, string newPassword)
        {
            var passwordChangedStatus = await _userBusiness.ChangePassword(loginId, newPassword);

            if (!string.IsNullOrEmpty(passwordChangedStatus))
            {
                return Ok(passwordChangedStatus);
            }
            return BadRequest(passwordChangedStatus);
        }

        [HttpGet("All")]
        public async Task<ActionResult<List<MovieDto>>> ViewAllMovies()
        {
            List<MovieDto>? movies = await _movieBusiness.GetMovies();

            if (movies is not null && movies.Count > 0)
            {
                return Ok(movies);
            }
            return NoContent();
        }

        [HttpGet("Movies/Search/MovieName")]
        public async Task<ActionResult<MovieDto>> SearchMovie(string movieName)
        {
            var movies = await _movieBusiness.SearchMovie(movieName);

            if (movies is not null && movies.Count > 0)
            {
                return Ok(movies);
            }
            return NoContent();
        }

        [HttpPost("{moviename}/bookticket")]
        [ServiceFilter(typeof(NullCheckFilter))]
        public async Task<ActionResult<string>> AddTickets(TicketDto ticket)
        {
            var status = await _ticketBusiness.AddTicket(ticket);

            if (!string.IsNullOrEmpty(status))
            {
                return Ok(status);
            }
            return BadRequest(status);
        }

        [HttpPut("{moviename}/update/{ticket}")]
        [ServiceFilter(typeof(NullCheckFilter))]
        public async Task<IActionResult> UpdateMovieTicketStatus(string moviename, string ticket)
        {
            TicketStatusResponse response = new TicketStatusResponse();
            try
            {
                response = await _movieBusiness.UpdateMovieTicketStatus(moviename, ticket);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete("{moviename}/delete/{id}")]
        public async Task<IActionResult> DeleteMovieById(string moviename, string id)
        {
            DeleteMovieByNameAndIdResponse response = new DeleteMovieByNameAndIdResponse();
            try
            {
                response = await _movieBusiness.DeleteMovieByNameAndId(moviename, id);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }
    }
}
