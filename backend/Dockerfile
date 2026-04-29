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

# Crée l'utilisateur seulement s'il n'existe pas déjà
RUN groupadd --system app || true
RUN useradd --system --gid app app || true

RUN mkdir -p /app/data && chown -R app:app /app/data

COPY --from=build --chown=app:app /app/publish ./

USER app

EXPOSE 5065
ENV ASPNETCORE_URLS=http://+:5065

ENTRYPOINT ["dotnet", "backend.dll"]