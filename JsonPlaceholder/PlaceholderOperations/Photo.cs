using System;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// Internal representation of a photo.
    /// </summary>
    internal sealed class Photo
    {
        public Photo(long id, string title, Uri uri, Uri thumbnailUri)
        {
            Id = id;
            Title = title;
            Uri = uri;
            ThumbnailUri = thumbnailUri;
        }

        public long Id { get; }
        public string Title { get; }
        public Uri Uri { get; }
        public Uri ThumbnailUri { get; }
    }
}