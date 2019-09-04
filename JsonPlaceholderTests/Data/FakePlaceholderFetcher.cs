using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsonPlaceholder.PlaceholderOperations;
using Newtonsoft.Json.Linq;

namespace JsonPlaceholderTests.Data
{
    /// <summary>
    /// This class stands in for <see cref="PlaceholderFetcher"/> in unit tests. The actual datasets returned by jsonplaceholder.typicode.com have been captured
    /// and added to the test suite as embedded resources.
    /// </summary>
    public class FakePlaceholderFetcher : IPlaceholderFetcher
    {
        private readonly string _albums;
        private readonly string _photos;

        public FakePlaceholderFetcher(Func<IEnumerable<JObject>, IEnumerable<JObject>> albumFilter = null, Func<IEnumerable<JObject>, IEnumerable<JObject>> photoFilter = null)
        {
            var assembly = typeof(FakePlaceholderFetcher).Assembly;
            var albumsResource = "JsonPlaceholderTests.Data.albums.json";
            var photosResource = "JsonPlaceholderTests.Data.photos.json";

            _albums = LoadJArrayResource(assembly, albumsResource, albumFilter);
            _photos = LoadJArrayResource(assembly, photosResource, photoFilter);
        }

        private static string LoadJArrayResource(Assembly assembly, string resourceName, Func<IEnumerable<JObject>, IEnumerable<JObject>> filter)
        {
            string data;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            if (filter != null)
            {
                var filtered = new JArray(filter(JArray.Parse(data).OfType<JObject>()));
                return filtered.ToString();
            }

            return data;
        }

        /// <summary>
        /// Return the fake photos dataset.
        /// </summary>
        public Task<string> GetPhotosAsync()
        {
            return Task.FromResult(_photos);
        }

        /// <summary>
        /// Return the fake photos dataset.
        /// </summary>
        public Task<string> GetAlbumsAsync()
        {
            return Task.FromResult(_albums);
        }
    }
}
