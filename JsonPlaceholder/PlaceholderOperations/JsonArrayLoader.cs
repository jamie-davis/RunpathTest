using System;
using System.Collections.Generic;
using JsonPlaceholder.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// This class accepts a string containing a JSON array and extracts the JObject instances found in the array. The array is assumes to comprise of JObjects, and
    /// any non-JObject elements are discarded and a warning is logged. If the text does not contain a JSON array, the error is logged and no objects will be returned.
    /// </summary>
    internal static class JsonArrayLoader
    {
        internal static IEnumerable<JObject> GetObjects(string jsonData, ILogger logger)
        {
            logger.LogInformation("Loading json array");

            JArray array;
            try
            {
                array = JArray.Parse(jsonData);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Invalid JSON received");
                throw new InvalidJsonException(jsonData);
            }

            if (array != null)
            {
                foreach (var token in array)
                {
                    if (token is JObject jobject)
                        yield return jobject;
                    else
                    {
                        logger.LogWarning("Received non-object element in JSON array {objectDef}", token.ToString());
                    }
                }
            }
        }
    }
}
