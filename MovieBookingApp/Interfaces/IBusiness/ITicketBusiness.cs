using MovieBookingApp.Models;

namespace MovieBookingApp.Interfaces.IBusiness
{
    public interface ITicketBusiness
    {
        public Task<string> AddTicket(TicketDto ticket);
    }
}
