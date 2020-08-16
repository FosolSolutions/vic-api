ARG BUILD_CONFIGURATION=Release
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 443 80

# Copy csproj and restore as distinct layers
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY *.sln .
COPY ./src/*.csproj api/

WORKDIR /src/api
RUN dotnet restore
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 3.1.0
# Copy everything else and build

COPY ./src .
RUN dotnet build "Vic.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
RUN dotnet publish "Vic.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish

# Runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh .
RUN chmod +x /app/entrypoint.sh

# CMD tail -f /dev/null
ENTRYPOINT ["/app/entrypoint.sh"]