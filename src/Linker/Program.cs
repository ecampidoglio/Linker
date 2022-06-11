using System;
using System.IO;
using Linker.Model;
using Linker.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddLinkInMemoryStore();

builder.Host
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureLogging(log => log.AddConsole());

builder.WebHost
    .UseKestrel()
    .UseIISIntegration();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapGet("/{id}", (string id, IRetrieveLinks getLink) =>
{
    var link = getLink.WithId(id);

    return link.HasValue
        ? Results.Redirect(link.Value.Href.AbsoluteUri, true)
        : Results.NotFound();
}).WithName("Follow");

app.MapGet("/link/{id}", (string id, IRetrieveLinks getLink) =>
{
    var link = getLink.WithId(id);

    return link.HasValue
        ? Results.Ok(link)
        : Results.NotFound();
}).WithName("Metadata");

app.MapPut("/link/{id}", (string id, HttpContext ctx, ISaveLinks saveLink)
    => Create(id, ctx.Request.Form["url"], saveLink));

app.MapPost("/link", (HttpContext ctx, ISaveLinks saveLink) =>
{
    var url = ctx.Request.Form["url"];

    return Create(UniqueId(), url, saveLink);

    string UniqueId() => Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
});

static IResult Create(string id, string url, ISaveLinks saveLink)
{
    if (IsNotAbsoluteUri(url))
    {
        return Results.BadRequest("Invalid or missing URL");
    }

    saveLink.WithIdAndUrl(id, new Uri(url));

    return Results.CreatedAtRoute("Follow", new { id }, url);

    bool IsNotAbsoluteUri(string uri) => !Uri.IsWellFormedUriString(uri, UriKind.Absolute);
}

app.Run();
