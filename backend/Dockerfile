# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend.csproj", "./"]
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN groupadd -r app && useradd -r -g app app
RUN mkdir -p /app/data && chown -R app:app /app/data

USER app

COPY --from=build --chown=app:app /app/publish ./

EXPOSE 5065
ENV ASPNETCORE_URLS=http://+:5065

ENTRYPOINT ["dotnet", "backend.dll"]
