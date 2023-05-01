using Microsoft.EntityFrameworkCore;
using API_TestProject.DataBase.Model;
using API_TestProject.Core;

namespace API_TestProject.DataBase
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options) 
        {
            if (TestsService.ShouldDataBaseBeReinitialized)
            { this.Database.EnsureDeleted(); TestsService.IsDataBaseReinitialized = true; }

            this.Database.EnsureCreated();
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    }
}
