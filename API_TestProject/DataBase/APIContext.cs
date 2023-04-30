using Microsoft.EntityFrameworkCore;
using API_TestProject.DataBase.Model;

namespace API_TestProject.DataBase
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
