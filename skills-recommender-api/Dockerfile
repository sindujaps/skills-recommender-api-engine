#build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
EXPOSE 443
COPY . /src
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

# release
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "skills-recommender-api.dll"]