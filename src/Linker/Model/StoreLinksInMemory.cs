using System;
using System.Collections.Generic;

namespace Linker.Model
{
    public class StoreLinksInMemory
        : ISaveLinks, IRetrieveLinks
    {
        private readonly Dictionary<object, Uri> links;

        public StoreLinksInMemory()
        {
            links = new Dictionary<object, Uri>
            {
                ["known-link-id"] = new Uri("http://example.com")
            };
        }

        public void WithIdAndUrl(string id, Uri url)
        {
            links[id] = url;
        }

        public Link? WithId(object id)
        {
            return links.ContainsKey(id)
                ? Link(id)
                : null;
        }

        private Link? Link(object id)
        {
            return new Link(id, links[id]);
        }
    }
}
