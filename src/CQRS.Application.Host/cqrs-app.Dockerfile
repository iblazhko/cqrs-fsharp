FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY ./bin/publish ./
EXPOSE 17321
ENTRYPOINT ["dotnet","./CQRS.Application.Host.dll"]
