using BtDownload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AilianBT.Services
{
    public class DbService
    {
        public DownloadDbContext DownloadDbContext { get; }
        public DbService()
        {
            DownloadDbContext = new DownloadDbContext();
        }
    }
}
