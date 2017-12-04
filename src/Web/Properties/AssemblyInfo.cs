using System.Reflection;
using System.Runtime.InteropServices;
using Linker.Web;
using Microsoft.Owin;

[assembly: AssemblyTitle("Linker.Web")]
[assembly: AssemblyCompany("Enrico Campidoglio")]
[assembly: AssemblyProduct("Linker")]
[assembly: AssemblyCopyright("Creative Commons Attribution 4.0 International License")]
[assembly: Guid("66ed5458-5e26-4bcf-87a4-0ab3c41eefb3")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: OwinStartup(typeof(Startup))]
