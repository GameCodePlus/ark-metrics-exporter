FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY source/*.sln .
COPY source/ark-metrics-exporter/*.csproj ./ark-metrics-exporter/
RUN dotnet restore

# copy everything else and build app
COPY source/ark-metrics-exporter/* ./ark-metrics-exporter/
WORKDIR /source/ark-metrics-exporter
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:3.1-alpine
WORKDIR /app
COPY --from=build /app ./
COPY docker/config.yaml ./
ENTRYPOINT ["dotnet", "ark-metrics-exporter.dll"]