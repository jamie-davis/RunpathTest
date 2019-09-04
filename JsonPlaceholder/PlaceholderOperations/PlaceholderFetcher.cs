using System.Net.Http;
using System.Threading.Tasks;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// This class encapsulates the calls to fetch the source data for the API. Since a HttpClient is used to go directly against jsonplaceholder.typicode.com, this class
    /// cannot be unit tested. To minimise untestable code, the results of the calls are passed back as strings, and testable code can be responsible for parsing and
    /// handling the data that came back.
    /// </summary>
    internal  class PlaceholderFetcher : IPlaceholderFetcher
    {
        /// <summary>
        /// HttpClient is thread safe and intended to be used in this way. In fact, creating them to order is bad and can result in
        /// socket exhaustion as each socket will last 240 seconds.
        /// For more info, see https://blogs.msdn.microsoft.com/shacorn/2016/10/21/best-practices-for-using-httpclient-on-services/
        /// </summary>
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Retrieve the content of the /photos call and return it as a string.
        /// </summary>
        /// <returns>The content from the HttpResponseMessage returned by the HttpClient call.</returns>
        public async Task<string> GetPhotosAsync()
        {
            var response = await Client.GetAsync("http://jsonplaceholder.typicode.com/photos");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        /// <summary>
        /// Retrieve the content of the /albums call and return it as a string.
        /// </summary>
        /// <returns>The content from the HttpResponseMessage returned by the HttpClient call.</returns>
        public async Task<string> GetAlbumsAsync()
        {
            var response = await Client.GetAsync("http://jsonplaceholder.typicode.com/albums");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }
}
