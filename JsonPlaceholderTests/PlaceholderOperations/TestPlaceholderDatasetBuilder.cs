using System.Collections.Generic;
using System.Linq;
using JsonPlaceholder.PlaceholderOperations;
using JsonPlaceholderTests.Data;
using JsonPlaceholderTests.TestUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using TestConsoleLib.Testing;
using Xunit;

namespace JsonPlaceholderTests.PlaceholderOperations
{
    public class TestPlaceholderDatasetBuilder
    {
        private PlaceholderDatasetCombiner _combiner;
        private ILogger<PlaceholderDatasetBuilder> _logger;
        private FakeLog _fakeLog;
        private FakePlaceholderFetcher _fetcher;

        public TestPlaceholderDatasetBuilder()
        {
            //Trim the test data
            IEnumerable<JObject> AlbumFilter(IEnumerable<JObject> albums)
            {
                return albums.GroupBy(a => a["userId"])
                    .Take(2) //only take two users
                    .SelectMany(g => g.Take(2)); //only take the first two albums for each user
            }

            IEnumerable<JObject> PhotoFilter(IEnumerable<JObject> photos)
            {
                return photos.GroupBy(p => p["albumId"])
                    .SelectMany(g => g.Take(2)); //Take two photos from each album
            }

            //The test data has been trimmed to just the first 2 photos in each album, and the albums have been trimmed to 2 users with 2 albums each.
            _fetcher = new FakePlaceholderFetcher(AlbumFilter, PhotoFilter);

            _fakeLog = new FakeLog(l => true);
            _logger = new FakeLogger<PlaceholderDatasetBuilder>(_fakeLog, "Test");
        }

        [Fact]
        public void FormatAllData()
        {
            //Arrange
            var builder = new PlaceholderDatasetBuilder(_fetcher, _logger);

            //Act
            var result = builder.BuildAsync().Result; //data is limited to 2 users, 2 albums each with 2 photos in each album, but we are selecting them all

            //Assert
            JArray.FromObject(result).ToString().Verify(); // simulate what the JsonResult will do and verify the output
        }

        [Fact]
        public void FormatAllDataForUser()
        {
            //Arrange
            var builder = new PlaceholderDatasetBuilder(_fetcher, _logger);

            //Act
            var result = builder.BuildAsync(2).Result; //data is limited to 2 users, 2 albums each with 2 photos in each album, we are only selecting one user.

            //Assert
            JArray.FromObject(result).ToString().Verify(); // simulate what the JsonResult will do and verify the output
        }
    }
}
