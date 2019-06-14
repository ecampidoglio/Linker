using System;
using Microsoft.AspNetCore.Mvc;
using Linker.Model;
using System.IO;

namespace Linker.Web.Controllers
{
    [Route("link")]
    public class LinkController : Controller
    {
        private readonly IRetrieveLinks getLink;
        private readonly ISaveLinks saveLink;

        public LinkController(
            IRetrieveLinks getLink,
            ISaveLinks saveLink)
        {
            this.getLink = getLink;
            this.saveLink = saveLink;
        }

        [HttpGet("/{id}", Name = "Follow")]
        public IActionResult Follow(string id)
        {
            var link = getLink.WithId(id);

            if (link.HasValue)
            {
                return RedirectPermanent(link.Value.Href.AbsoluteUri);
            }

            return NotFound();
        }

        [HttpGet("{id}", Name = "Metadata")]
        public IActionResult Metadata(string id)
        {
            var link = getLink.WithId(id);

            if (link.HasValue)
            {
                return Ok(link);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Create(string url)
        {
            return Create(UniqueId(), url);

            string UniqueId()
                => Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        }

        [HttpPut("{id}")]
        public IActionResult Create(string id, string url)
        {
            if (IsNotAbsoluteUri(url))
            {
                return BadRequest("Invalid or missing URL");
            }

            saveLink.WithIdAndUrl(id, new Uri(url));

            return CreatedAtRoute("Follow", new { id }, url);

            bool IsNotAbsoluteUri(string uri)
                => !Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }
    }
}
