FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore Algobot.Worker.csproj
RUN dotnet publish Algobot.Worker.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Algobot.Worker.dll"]

# docker build -t algobot-worker -f Dockerfile .
# docker-compose up