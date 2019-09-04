using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using JsonPlaceholder.Exceptions;
using JsonPlaceholder.PlaceholderOperations;
using JsonPlaceholderTests.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonPlaceholderTests.PlaceholderOperations
{
    public class TestJsonArrayLoader
    {
        [Fact]
        public void JsonObjectsAreReturned()
        {
            //Arrange
            var fetcher = new FakePlaceholderFetcher();
            var json = fetcher.GetAlbumsAsync().Result;
            var array = JArray.Parse(json);

            //Act
            var objects = JsonArrayLoader.GetObjects(json, NullLogger.Instance); 

            //Assert
            objects.Count().Should().Be(array.Count);
        }

        [Fact]
        public void ExceptionIsThrownForInvalidJson()
        {
            //Arrange
            var json = "invalid";

            //Act/Assert
            var action = new Action(() => JsonArrayLoader.GetObjects(json, NullLogger.Instance).ToList());
            action.Should().Throw<InvalidJsonException>();
        }

        [Fact]
        public void OnlyJObjectsAreReturned()
        {
            //Arrange
            var json = "[ 1, 2, 3, {a:4}, 5]";

            //Act
            var result = JsonArrayLoader.GetObjects(json, NullLogger.Instance).ToList();

            //Assert
            var expected = new object[] {JObject.Parse("{a:4}")};
            result.Should().BeEquivalentTo(expected);
        }
    }
}
