using System;
using System.IO;
using System.Web.Http;
using Linker.Model;

namespace Linker.Web.Controllers
{
    public class LinkController : ApiController
    {
        private readonly IRetrieveLinks _getLink;
        private readonly ISaveLinks _saveLink;

        public LinkController(
            IRetrieveLinks getLink,
            ISaveLinks saveLink)
        {
            _getLink = getLink;
            _saveLink = saveLink;
        }

        [HttpGet, Route("~/{id}", Name = "Follow")]
        public IHttpActionResult Follow(string id)
        {
            var link = _getLink.WithId(id);

            if (link.HasValue)
            {
                return Redirect(link.Value.Href.AbsoluteUri);
            }

            return NotFound();
        }

        [HttpGet, Route("link/{id}", Name = "Metadata")]
        public IHttpActionResult Metadata(string id)
        {
            var link = _getLink.WithId(id);

            if (link.HasValue)
            {
                return Ok(link.Value);
            }

            return NotFound();
        }

        [HttpPost, Route("link")]
        public IHttpActionResult Create([FromBody]string url)
        {
            return Create(UniqueId(), url);

            string UniqueId()
            {
                return Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            }
        }

        [HttpPut, Route("link/{id}")]
        public IHttpActionResult Create(string id, [FromBody]string url)
        {
            _saveLink.WithIdAndUrl(id, new Uri(url));

            return CreatedAtRoute("Follow", new { id }, url);
        }
    }
}
