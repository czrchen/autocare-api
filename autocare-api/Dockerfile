# 1. Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# API listens on port 8080 inside the container
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# 2. Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore packages
COPY ["autocare-api.csproj", "./"]
RUN dotnet restore "autocare-api.csproj"

# Copy all source code into the container
COPY . .

# Build and publish the API
RUN dotnet publish "autocare-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Final runtime image
FROM base AS final
WORKDIR /app

# Bring the published build from previous stage
COPY --from=build /app/publish .

# Run the API
ENTRYPOINT ["dotnet", "autocare-api.dll"]
