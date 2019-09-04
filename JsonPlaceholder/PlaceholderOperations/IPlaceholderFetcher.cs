using System.Threading.Tasks;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// This interface defines placeholder operations. It is implemented by the untestable <see cref="PlaceholderFetcher"/> class which has a dependency on HttpClient,
    /// and a fake defined in the test suite.
    /// </summary>
    public interface IPlaceholderFetcher
    {
        /// <summary>
        /// Retrieve the content of the /photos call and return it as a string.
        /// </summary>
        Task<string> GetPhotosAsync();

        /// <summary>
        /// Retrieve the content of the /albums call and return it as a string.
        /// </summary>
        Task<string> GetAlbumsAsync();
    }
}