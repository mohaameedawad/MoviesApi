using Microsoft.EntityFrameworkCore;

namespace moviesApi.Models
{
    public class MoviesDbContext : DbContext 
    {
        public MoviesDbContext(DbContextOptions<MoviesDbContext> options) : base(options)
        {
            
        }
    }
}
