
using Microsoft.EntityFrameworkCore;
using moviesApi.Models;

namespace moviesApi.Services
{
    public class GenreService : IGenreService
    {
        private readonly MoviesDbContext _context;
        public GenreService(MoviesDbContext context)
        {
            _context = context;
        }
        
        public async Task<Genre> Add(Genre genre)
        {
            await _context.genres.AddAsync(genre);
            _context.SaveChanges();
            return genre;
        }

        public async Task<Genre> delete(Genre genre)
        {
            _context.genres.Remove(genre);
            _context.SaveChanges();
            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            return  await _context.genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre> GetById (byte id)
        {
           return await _context.genres.SingleOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Genre> Update(Genre genre)
        {
            _context.genres.Update(genre);
            _context.SaveChanges();
            return genre;
        }
    }
}
