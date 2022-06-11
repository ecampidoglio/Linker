using System.IO;
using Linker.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

app.Run();
