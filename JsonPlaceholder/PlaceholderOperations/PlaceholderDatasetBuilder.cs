using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// This class is responsible for extracting album data.
    /// </summary>
    public class PlaceholderDatasetBuilder
    {
        private readonly IPlaceholderFetcher _fetcher;
        private readonly ILogger<PlaceholderDatasetBuilder> _logger;

        public PlaceholderDatasetBuilder(IPlaceholderFetcher fetcher, ILogger<PlaceholderDatasetBuilder> logger)
        {
            _fetcher = fetcher;
            _logger = logger;
        }

        public async Task<object> BuildAsync()
        {
            var combiner = await MakeCombiner();
            return combiner.Dataset;
        }

        public async Task<object> BuildAsync(long userId)
        {
            var combiner = await MakeCombiner();
            return combiner.Dataset.Where(a => a.UserId == userId).ToList();
        }

        private async Task<PlaceholderDatasetCombiner> MakeCombiner()
        {
            var albums = JsonArrayLoader.GetObjects(await _fetcher.GetAlbumsAsync(), _logger);
            var photos = JsonArrayLoader.GetObjects(await _fetcher.GetPhotosAsync(), _logger);
            return new PlaceholderDatasetCombiner(albums, photos, _logger);
        }
    }
}
