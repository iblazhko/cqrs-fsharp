FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ./bin/publish ./
ENTRYPOINT ["dotnet","./CQRS.CLI.dll"]
