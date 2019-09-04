using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// This class analyses the input albums and photos and produces a set of albums with photos in the internal format. Albums and photos not well
    /// formed are skipped and information records are written to the log referencing the problems. Info messages where chosen because in the event
    /// of an input error, very large amounts of log data may be generated, and therefore you would only want to turn on info logging for diagnostic
    /// purposes. An alternative approach would be to fail the extract entirely in the event that a data error is detected, in which case the logs
    /// should be errors. The decision as to which way to go depends on the trust level in the source data.
    /// </summary>
    public sealed class PlaceholderDatasetCombiner
    {
        internal List<Album> Dataset { get; }

        /// <summary>
        /// Combine the datasets and cache the result.
        /// </summary>
        /// <param name="albums">The raw album objects</param>
        /// <param name="photos">The raw photo objects</param>
        /// <param name="logger">The logger</param>
        public PlaceholderDatasetCombiner(IEnumerable<JObject> albums, IEnumerable<JObject> photos, ILogger logger)
        {
            var allPhotos = photos
                .Where(p => p.ContainsKey("albumId"))
                .GroupBy(p => p["albumId"])
                .ToDictionary(grp => grp.Key, grp => grp.ToList());

            Dataset = MakeAlbums(albums, allPhotos, logger).ToList();
        }

        private IEnumerable<Album> MakeAlbums(IEnumerable<JObject> albums, Dictionary<JToken, List<JObject>> allPhotos, ILogger logger)
        {
            foreach (var albumObj in albums)
            {
                var album = MakeAlbum(albumObj, logger);
                if (album != null)
                {
                    if (allPhotos.TryGetValue(albumObj["id"], out var photos))
                    {
                        album.Photos = photos
                            .Select(p => MakePhoto(p, logger))
                            .Where(p => p != null)
                            .ToList();
                    }
                    yield return album;
                }
            }
        }

        private Album MakeAlbum(JObject albumObj, ILogger logger)
        {
            if (TryGetLong(albumObj, "album", "id", logger, out var id)
                && TryGetLong(albumObj, "album", "userId", logger, out var userId)
                && TryGetString(albumObj, "album", "title", logger, out var title))
            {
                return new Album(userId, id, title);
            }

            return null;
        }

        private Photo MakePhoto(JObject photoObj, ILogger logger)
        {
            if (TryGetLong(photoObj, "photo", "id", logger, out var id)
                && TryGetString(photoObj, "photo", "title", logger, out var title)
                && TryGetUri(photoObj, "photo", "url", logger, out var uri)
                && TryGetUri(photoObj, "photo", "thumbnailUrl", logger, out var thumbUri))
            {
                return new Photo(id, title, uri, thumbUri);
            }

            return null;
        }

        private bool TryGetLong(JObject obj, string objectType, string property, ILogger logger, out long result)
        {
            if (obj.TryGetValue(property, out var token) && long.TryParse(token.ToString(), out result))
            {
                return true;
            }

            logger.LogInformation("Cannot extract {property} from {objectType}. Json: {json}", property, objectType, obj.ToString(Formatting.None));
            
            result = 0L;
            return false;
        }

        private bool TryGetUri(JObject obj, string objectType, string property, ILogger logger, out Uri result)
        {
            if (obj.TryGetValue(property, out var token) && Uri.IsWellFormedUriString(token.ToString(), UriKind.Absolute))
            {
                result = new Uri(token.ToString());
                return true;
            }

            logger.LogInformation("Cannot extract {property} from {objectType}. Json: {json}", property, objectType, obj.ToString(Formatting.None));
            
            result = null;
            return false;
        }

        private bool TryGetString(JObject obj, string objectType, string property, ILogger logger, out string result)
        {
            if (obj.TryGetValue(property, out var token))
            {
                if (token is JValue value && value.Type == JTokenType.String)
                {
                    result = token.ToString();
                    return true;
                }
            }

            logger.LogInformation("Cannot extract {property} from {objectType}. Json: {json}", property, objectType, obj.ToString(Formatting.None));

            result = null;
            return false;
        }
    }
}
