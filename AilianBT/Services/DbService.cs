using BtDownload.Models;

namespace AilianBT.Services
{
    public class DbService
    {
        public DownloadDbContext DownloadDbContext { get; private set; } = new DownloadDbContext();

        public DbService()
        {
        }
    }
}
