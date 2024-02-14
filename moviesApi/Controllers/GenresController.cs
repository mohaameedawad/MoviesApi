using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using moviesApi.Models;

namespace moviesApi.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        public readonly MoviesDbContext _context;

        public GenresController( MoviesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _context.genres.OrderBy(g => g.Name).ToListAsync();

            return Ok(genres);
        }

        [HttpPost("CreateGenre")]
        public  async Task<IActionResult> CreateAsync( GenreDto genreDto)
        {
            var Genre = new Genre
            {
                Name = genreDto.Name
            };

            await _context.genres.AddAsync(Genre);
            _context.SaveChanges();
            return Ok(Genre);
        }

        [HttpPut("UpdateResource/{id}")]
        public async Task<IActionResult> UpdateGenre(int id, GenreDto genreDto)
        {
            var genre = await _context.genres.SingleOrDefaultAsync(g => g.Id == id);

            if(genre == null)
            {
                return NotFound($"no genre was found with the ID: {id}");
            }
            genre.Name = genreDto.Name;
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.genres.SingleOrDefaultAsync(g => g.Id == id);

            if (genre == null)
            {
                return NotFound($"no genre was found with the ID: {id}");
            }
            _context.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

    }
}
