using System.Collections.Generic;

namespace JsonPlaceholder.PlaceholderOperations
{
    /// <summary>
    /// Internal representation of an album with its photos
    /// </summary>
    internal sealed class Album
    {
        public Album(long userId, long id, string title)
        {
            UserId = userId;
            Id = id;
            Title = title;
        }

        public long UserId { get; }
        public long Id { get; }
        public string Title { get; }

        public List<Photo> Photos { get; set; } = new List<Photo>();
    }
}