using AilianBT.Models;

namespace AilianBT.Services
{
    public class DbService
    {
        public DownloadDbContext DownloadDbContext { get; private set; } = new DownloadDbContext();
    }
}
