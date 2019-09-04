using System;

namespace JsonPlaceholder.Exceptions
{
    /// <summary>
    /// This exception is thrown when one of the JSON responses from jsonplaceholder.typicode.com contains invalid data.
    /// </summary>
    public class InvalidJsonException : Exception
    {
        public string Json { get; }

        public InvalidJsonException(string json) : base("Invalid json received")
        {
            Json = json;
        }
    }
}
