FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ./bin/publish ./
EXPOSE 17322
ENTRYPOINT ["dotnet","./CQRS.API.Host.dll"]
