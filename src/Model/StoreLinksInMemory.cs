using System;
using System.Collections.Generic;

namespace Linker.Model
{
    public class StoreLinksInMemory
        : ISaveLinks, IRetrieveLinks
    {
        private readonly Dictionary<object, Uri> _links;

        public StoreLinksInMemory()
        {
            _links = new Dictionary<object, Uri>
            {
                ["known-link-id"] = new Uri("http://example.com")
            };
        }

        public void WithIdAndUrl(string id, Uri url)
        {
            _links[id] = url;
        }

        public Link? WithId(object id)
        {
            return _links.ContainsKey(id)
                ? Link(id)
                : null;
        }

        private Link? Link(object id)
        {
            return new Link(id, _links[id]);
        }
    }
}
