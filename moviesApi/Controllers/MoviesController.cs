using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace moviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private new List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        private readonly MoviesDbContext _context;
        public MoviesController(MoviesDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies
                .OrderByDescending(x => x.Id)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    Year = m.Year,
                    Rate = m.Rate,
                    Title = m.Title,
                    Storeline = m.Storeline,
                })
                .ToListAsync();
            return Ok(movies);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m =>m.Id == id);

            if (movie == null)
                return NotFound();

            var dto = new MovieDetailsDto
            {
                Id = movie.Id,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Poster = movie.Poster,
                Year = movie.Year,
                Rate = movie.Rate,
                Title = movie.Title,
                Storeline = movie.Storeline,
            };
            return Ok(movie);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte id)
        {
            var genre = await _context.genres.FindAsync(id);
            if(genre == null)
                return NotFound();

            var movies = await _context.Movies
                .Where(m => m.GenreId == id)
                .OrderByDescending(x => x.Id)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    Year = m.Year,
                    Rate = m.Rate,
                    Title = m.Title,
                    Storeline = m.Storeline,
                })
                .ToListAsync();
            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovieAsync ( [FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Movie Poster is Required!");

            if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("only .png and .jpg extensions are allowed!");

            if(dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max poster size allowed is 1MB.");

            var isValidGenreId = await _context.genres.AnyAsync(g => g.Id == dto.GenreId);

            if (!isValidGenreId)
                return BadRequest("Invalid Genre Id");
                    
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = new Movie()
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Rate = dto.Rate,
                Poster = dataStream.ToArray(),
                Storeline = dto.Storeline,
                Year = dto.Year,
            };
            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateMovieAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"no movie was found with id {id}");

            var isValidGenreId = await _context.genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenreId)
                return BadRequest("Invalid Genre Id");

            if (dto.Poster != null)
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("only .png and .jpg extensions are allowed!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max poster size allowed is 1MB.");

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                
                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Storeline = dto.Storeline;
            movie.Rate = dto.Rate;
            movie.GenreId = dto.GenreId;

            _context.SaveChanges();
            return Ok(movie);


        }


        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null) 
                return NotFound($"no Movie was Found with id {id}");

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok($"The Movie with Id {id} is successfully deleted");
        }
    }
}
