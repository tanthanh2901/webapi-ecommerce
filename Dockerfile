# Use the official ASP.NET Core runtime for development
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7226

# Use the .NET SDK for building and development
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files for restoring dependencies
COPY ["FoodShop.API/FoodShop.API.csproj", "FoodShop.API/"]
COPY ["FoodShop.Application/FoodShop.Application.csproj", "FoodShop.Application/"]
COPY ["FoodShop.Domain/FoodShop.Domain.csproj", "FoodShop.Domain/"]
COPY ["FoodShop.Infrastructure/FoodShop.Infrastructure.csproj", "FoodShop.Infrastructure/"]
COPY ["FoodShop.Persistence/FoodShop.Persistence.csproj", "FoodShop.Persistence/"]

# Restore dependencies
RUN dotnet restore "FoodShop.API/FoodShop.API.csproj"

# Copy the source code for hot reloading
COPY . .

# Set working directory to the API project
WORKDIR "/src/FoodShop.API"

# Use dotnet watch for local development
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:7226"]
