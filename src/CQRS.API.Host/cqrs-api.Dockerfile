FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./bin/publish ./
EXPOSE 17322
ENTRYPOINT ["dotnet","./CQRS.API.Host.dll"]
