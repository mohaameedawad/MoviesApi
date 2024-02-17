namespace moviesApi.Services
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllGenres();
        Task<Genre> Add(Genre genre);
        Task<Genre> Update(Genre genre);
        Task<Genre> GetById(byte id);
        Task<Genre> delete(Genre genre);
    }
}
