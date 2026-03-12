# =========================
# Base runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# =========================
# Tailwind build stage
# =========================
FROM node:22-alpine AS tailwind
WORKDIR /app

COPY NewsHub.Web/package*.json ./
RUN npm install

COPY NewsHub.Web/ ./

RUN chmod +x ./node_modules/.bin/tailwindcss

RUN npx @tailwindcss/cli \
    -i ./wwwroot/css/site.css \
    -o ./wwwroot/css/output.css \
    --minify


# =========================
# .NET build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["NewsHub.Web/NewsHub.Web.csproj", "NewsHub.Web/"]
COPY ["NewsHub.Application/NewsHub.Application.csproj", "NewsHub.Application/"]
COPY ["NewsHub.Domain/NewsHub.Domain.csproj", "NewsHub.Domain/"]
COPY ["NewsHub.Infrastructure/NewsHub.Infrastructure.csproj", "NewsHub.Infrastructure/"]

RUN dotnet restore "NewsHub.Web/NewsHub.Web.csproj"

COPY . .

WORKDIR "/src/NewsHub.Web"

# Copiar CSS compilado desde Tailwind
COPY --from=tailwind /app/wwwroot/css/output.css ./wwwroot/css/output.css

RUN dotnet publish "NewsHub.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Final runtime
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NewsHub.Web.dll"]