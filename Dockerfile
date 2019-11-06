FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY ./publish ./
ENTRYPOINT ["dotnet", "Linker.dll"]
