using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtDownload.Models
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
