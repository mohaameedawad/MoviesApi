using Microsoft.AspNetCore.Mvc;

namespace moviesApi.Services
{
    public interface IMoviesService
    {
        Task<IEnumerable<Movie>> GetAllAsync(byte genreId = 0);
        Task<Movie> GetById(int id);
        Task<IEnumerable<Movie>> GetByGenreIdAsync(byte id);
        Task<Movie> CreateMovie(Movie movie);
        Task<Movie> UpdateMovie(Movie movie);
        Movie DeleteMovie(Movie movie);
        Task<bool> IsValidGenre(byte id);
    }
}
