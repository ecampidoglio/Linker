FROM mcr.microsoft.com/dotnet/core/aspnet:6.0
WORKDIR /app
COPY ./publish ./
ENTRYPOINT ["dotnet", "Linker.dll"]
