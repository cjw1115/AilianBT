using Microsoft.EntityFrameworkCore;

namespace AilianBT.Models
{
    public class DownloadDbContext:DbContext
    {
        public DbSet<DownloadedInfo> DownloadedInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Download.db");
        }
    }
}
