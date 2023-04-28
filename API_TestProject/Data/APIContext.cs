using Microsoft.EntityFrameworkCore;
using API_TestProject.Model.Internal;

namespace API_TestProject.Data
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options) 
        {
            this.Database.EnsureCreated();   
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }

    }
}
