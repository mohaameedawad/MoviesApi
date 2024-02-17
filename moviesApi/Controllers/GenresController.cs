using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using moviesApi.Models;
using moviesApi.Services;

namespace moviesApi.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        public readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genreService.GetAllGenres();

            return Ok(genres);
        }

        [HttpPost("CreateGenre")]
        public  async Task<IActionResult> CreateAsync( GenreDto genreDto)
        {
            var Genre = new Genre { Name = genreDto.Name };

            await _genreService.Add(Genre);

            return Ok(Genre);
        }

        [HttpPut("UpdateResource/id")]
        public async Task<IActionResult> UpdateGenre(byte id, GenreDto genreDto)
        {
            var genre = await _genreService.GetById(id);

            if(genre == null)
                return NotFound($"no genre was found with the ID: {id}");

            genre.Name = genreDto.Name;

            _genreService.Update(genre);

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(byte id)
        {
            var genre = await _genreService.GetById(id);

            if (genre == null)
                return NotFound($"no genre was found with the ID: {id}");

            _genreService.delete(genre);

            return Ok(genre);
        }

    }
}
