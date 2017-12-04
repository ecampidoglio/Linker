using System;

namespace Linker.Model
{
    public struct Link
    {
        public Link(object id, string href)
            : this(id, new Uri(href))
        {
        }

        public Link(object id, Uri href)
        {
            Id = id;
            Href = href;
        }

        public object Id { get; }

        public Uri Href { get; }
    }
}
