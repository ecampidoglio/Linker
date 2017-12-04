using System;

namespace Linker.Model
{
    public interface ISaveLinks
    {
        void WithIdAndUrl(string id, Uri url);
    }
}
