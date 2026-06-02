using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Tasks.Models;

namespace Tasks.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablas de la base de datos
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}