using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moviesApi.Models;

namespace moviesApi.Services
{
    
    public class MoviesService : IMoviesService
    {
        private readonly MoviesDbContext _context;
        public MoviesService(MoviesDbContext context)
        {
            _context = context;
        }
        public async Task<Movie> CreateMovie(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();
            var movieincludeGenre = await GetById(movie.Id);
            return (movieincludeGenre);
        }

        public Movie DeleteMovie(Movie movie)
        {
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return(movie);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(byte genreId = 0)
        {
            return await _context.Movies
                .Where(m => m.GenreId == genreId || genreId == 0)
                .OrderByDescending(m => m.Id)
                .Include(m => m.Genre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetByGenreIdAsync(byte id)
        {
            return await GetAllAsync(id);
        }

        public async Task<Movie> GetById(int id)
        {
           return await _context.Movies
                   .Include(m => m.Genre)
                   .SingleOrDefaultAsync(m => m.Id == id);
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            _context.Movies.Update(movie);
            _context.SaveChanges();
            var movieincludeGenre = GetById(movie.Id);
            return (movieincludeGenre);
        }

        public async Task<bool> IsValidGenre(byte id)
        {
            return await _context.genres.AnyAsync(x => x.Id == id);
        }
    }
}
