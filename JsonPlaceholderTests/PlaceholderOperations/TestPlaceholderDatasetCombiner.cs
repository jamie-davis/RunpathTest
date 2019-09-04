using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using JsonPlaceholder.PlaceholderOperations;
using JsonPlaceholderTests.Data;
using JsonPlaceholderTests.TestUtils;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;
using Xunit;

namespace JsonPlaceholderTests.PlaceholderOperations
{
    public class TestPlaceholderDatasetCombiner
    {
        private NullLogger _logger;
        private List<JObject> _photos;
        private List<JObject> _albums;

        public TestPlaceholderDatasetCombiner()
        {
            var fetcher = new FakePlaceholderFetcher();
            _logger = NullLogger.Instance;
            _albums = JsonArrayLoader.GetObjects(fetcher.GetAlbumsAsync().Result, _logger).ToList();
            _photos = JsonArrayLoader.GetObjects(fetcher.GetPhotosAsync().Result, _logger).ToList();
        }

        [Fact]
        public void AllAlbumsAreExtracted()
        {
            //Act
            var result = new PlaceholderDatasetCombiner(_albums, _photos, _logger);

            //Assert
            result.Dataset.Count.Should().Be(_albums.Count);
        }

        [Fact]
        public void AllPhotosAreExtracted()
        {
            //Act
            var result = new PlaceholderDatasetCombiner(_albums, _photos, _logger);

            //Assert
            result.Dataset.SelectMany(a => a.Photos).Count().Should().Be(_photos.Count);
        }

        [Fact]
        public void AlbumDetailsAreCorrect()
        {
            //Arrange
            var albumSubset = _albums.Where(a => a["userId"].Value<int>() == 5).Take(2)
                .Concat(_albums.Where(a => a["userId"].Value<int>() == 8).Take(2))
                .ToList();

            //Act
            var result = new PlaceholderDatasetCombiner(albumSubset, _photos, _logger);

            //Assert
            var output = new Output();
            output.WrapLine("Input JSON:");
            output.WriteLine();
            foreach (var albumJson in albumSubset)
            {
                output.WrapLine(albumJson.ToString(Formatting.Indented));
            }

            output.WriteLine();
            output.WriteLine();

            output.WrapLine("Extracted Album Data:");
            output.WriteLine();
            output.FormatTable(result.Dataset.Select(album => new {album.Id, album.UserId, album.Title}));
            output.Report.Verify();
        }

        /// <summary>
        /// Note that this test confirms that the photos went into the correct album by displaying the album ID from
        /// the container album. It is not recorded on the photo item itself once assigned to an album, so it can only
        /// have a matching value if it was correctly assigned.
        /// </summary>
        [Fact]
        public void PhotoDetailsAreCorrect()
        {
            //Arrange
            var photoSubset = _photos.Where(p => p["albumId"].Value<int>() == 10).Take(2)
                .Concat(_photos.Where(p => p["albumId"].Value<int>() == 75).Take(2))
                .ToList();

            //Act
            var result = new PlaceholderDatasetCombiner(_albums, photoSubset, _logger);

            //Assert
            var buffer = new OutputBuffer() {BufferWidth = 132};
            var output = new Output(buffer);
            output.WrapLine("Input JSON:");
            output.WriteLine();
            foreach (var albumJson in photoSubset)
            {
                output.WrapLine(albumJson.ToString(Formatting.Indented));
            }

            output.WriteLine();
            output.WriteLine();

            output.WrapLine("Extracted Photo Data:");
            output.WriteLine();
            var photoDetails = result.Dataset
                .SelectMany(album => album.Photos
                    .Select(p => new {AlbumId = album.Id, PhotoId = p.Id, p.Title, p.ThumbnailUri, p.Uri}));
            output.FormatTable(photoDetails);
            output.Report.Verify();
        }

        [Fact]
        public void MissingDataIsLogged()
        {
            //Arrange
            var albumProperties = _albums.SelectMany(obj => obj.Properties().Select(p => p.Name)).Distinct().ToList();
            var photoProperties = _photos.SelectMany(obj => obj.Properties().Select(p => p.Name)).Distinct().ToList();

            var goodAlbum = _albums.Skip(albumProperties.Count).Take(1).ToList();

            var brokenAlbums = _albums.Take(albumProperties.Count)
                .Zip(albumProperties, (jObj, prop) =>
                {
                    jObj.Remove(prop);
                    return jObj;
                })
                .Concat(goodAlbum)
                .ToList();

            var goodAlbumId = goodAlbum[0]["id"];
            var goodAlbumPhotos = _photos.Where(p => p["albumId"].Equals(goodAlbumId));
            var brokenPhotos = goodAlbumPhotos.Take(photoProperties.Count)
                .Zip(photoProperties, (jObj, prop) =>
                {
                    jObj.Remove(prop);
                    return jObj;
                })
                .ToList();

            var fakeLog = new FakeLog(l => true);
            var recordingLogger = new FakeLogger(fakeLog, "test");

            //Act
            var result = new PlaceholderDatasetCombiner(brokenAlbums, brokenPhotos, recordingLogger);

            //Assert
            fakeLog.Report.Verify();
        }

        [Fact]
        public void InvalidDataIsLogged()
        {
            //Arrange
            var albumProperties = _albums.SelectMany(obj => obj.Properties().Select(p => p.Name)).Distinct().ToList();
            var photoProperties = _photos.SelectMany(obj => obj.Properties().Select(p => p.Name)).Distinct().ToList();

            var goodAlbum = _albums.Skip(albumProperties.Count).Take(1).ToList();

            JObject BreakJProperty(JObject jObj, string prop)
            {
                if (jObj[prop] is JValue valueToken)
                {
                    switch (valueToken.Type)
                    {
                        case JTokenType.String:
                            jObj[prop] = 1;
                            break;

                        case JTokenType.Uri:
                        case JTokenType.Integer:
                            jObj[prop] = "bad";
                            break;
                    }
                }
                return jObj;
            };
            var brokenAlbums = _albums.Take(albumProperties.Count)
                .Zip(albumProperties, BreakJProperty)
                .Concat(goodAlbum)
                .ToList();

            var goodAlbumId = goodAlbum[0]["id"];
            var goodAlbumPhotos = _photos.Where(p => p["albumId"].Equals(goodAlbumId));
            var brokenPhotos = goodAlbumPhotos.Take(photoProperties.Count)
                .Zip(photoProperties, BreakJProperty)
                .ToList();

            var fakeLog = new FakeLog(l => true);
            var recordingLogger = new FakeLogger(fakeLog, "test");

            //Act
            var result = new PlaceholderDatasetCombiner(brokenAlbums, brokenPhotos, recordingLogger);

            //Assert
            fakeLog.Report.Verify();
        }
    }
}
