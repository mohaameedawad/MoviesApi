using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moviesApi.Services;

namespace moviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private new List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        private readonly IMoviesService _moviesService;
        private readonly IGenreService _genreService;
        private readonly IMapper _mapper;


        public MoviesController(IMoviesService moviesService, IGenreService genreService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genreService = genreService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesService.GetAllAsync();
            
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);

            return Ok(data);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
                return NotFound();

            var dto = _mapper.Map<MovieDetailsDto>(movie);
           
            return Ok(dto);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte id)
        {
            var genre = await _genreService.GetById(id);
            if(genre == null)
                return NotFound();

            var movies = await _moviesService.GetByGenreIdAsync(id);

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);

            return Ok(data);
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

            var isValidGenreId = await _moviesService.IsValidGenre(dto.GenreId);

            if (!isValidGenreId)
                return BadRequest("Invalid Genre Id");
                    
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();
           

            await _moviesService.CreateMovie(movie);

            var moviex = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(moviex);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateMovieAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound($"no movie was found with id {id}");

            var isValidGenreId = await _moviesService.IsValidGenre(dto.GenreId);
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

            await _moviesService.UpdateMovie(movie);
            var moviex = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(moviex);
        }


        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteMovieAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null) 
                return NotFound($"no Movie was Found with id {id}");

            _moviesService.DeleteMovie(movie);
            return Ok($"The Movie with Id {id} is successfully deleted");
        }
    }
}
