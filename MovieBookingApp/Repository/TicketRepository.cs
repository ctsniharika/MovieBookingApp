using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieBookingApp.Interfaces.IRepository;
using MovieBookingApp.Models;

namespace MovieBookingApp.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IOptions<MongoDbConfig> _mongoDbConfig;
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly IMongoCollection<Movie> _movies;
        public TicketRepository(IOptions<MongoDbConfig> mongoDbConfig, IMongoClient mongoClient)
        {
            _mongoDbConfig = mongoDbConfig;

            var database = mongoClient.GetDatabase(_mongoDbConfig.Value.DatabaseName);
            _tickets = database.GetCollection<Ticket>(_mongoDbConfig.Value.TicketCollectionName);
            _movies = database.GetCollection<Movie>(_mongoDbConfig.Value.MovieCollectionName);
        }

        public async Task<bool> AddTicket(Ticket ticket)
        {
            try
            {
                await _tickets.InsertOneAsync(ticket);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public async Task<Movie> GetMovieByMovieName(string moviename)
        //{
        //    var movies = await _movies.FindAsync(movie => movie.Name.Equals(moviename));
        //    return await movies.FirstOrDefaultAsync();
        //}
    }
}
