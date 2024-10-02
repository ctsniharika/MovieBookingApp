using AutoMapper;
using MovieBookingApp.Interfaces.IBusiness;
using MovieBookingApp.Interfaces.IRepository;
using MovieBookingApp.Models;
using MovieBookingApp.Repository;

namespace MovieBookingApp.Business
{
    public class TicketBusiness : ITicketBusiness
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;
        public TicketBusiness(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<string> AddTicket(TicketDto ticket)
        {
            string status = string.Empty;
            try
            {
                var ticketModel = _mapper.Map<Ticket>(ticket);
                var isTicketInserted = await _ticketRepository.AddTicket(ticketModel);

                if (isTicketInserted)
                {
                    status = "Ticket booked successfully";
                }
                else
                {
                    status = "Ticket booking failed";
                }
                
            }
            catch (Exception)
            {
                status = string.Empty;
            }

            return status;
        }
    }
}
