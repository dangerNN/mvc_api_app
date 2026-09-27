using Microsoft.EntityFrameworkCore;
using mvc_api_app.Models;

public class StudentsContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public StudentsContext(DbContextOptions<StudentsContext> options) : base() { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        optionsBuilder.UseSqlServer(@"Server=WIN-8G40ARFN4JP\DANGERNN;Database=StudentsDB;Integrated Security=True;TrustServerCertificate=True;");
    }
}
