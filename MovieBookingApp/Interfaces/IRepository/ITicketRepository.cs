using MovieBookingApp.Models;

namespace MovieBookingApp.Interfaces.IRepository
{
    public interface ITicketRepository
    {
        public Task<bool> AddTicket(Ticket ticket);

        //public Task<Movie> GetMovieByMovieName(string movieName);
    }
}
