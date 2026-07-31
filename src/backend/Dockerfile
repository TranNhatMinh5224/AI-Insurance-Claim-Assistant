# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy các file .csproj để restore packages trước (giúp tận dụng Docker cache)
COPY src/Backend.Domain/*.csproj src/Backend.Domain/
COPY src/Backend.Application/*.csproj src/Backend.Application/
COPY src/Backend.Infrastructure/*.csproj src/Backend.Infrastructure/
COPY src/Backend.WebApi/*.csproj src/Backend.WebApi/

# Restore dependencies
RUN dotnet restore src/Backend.WebApi/Backend.WebApi.csproj

# Copy toàn bộ source code vào container
COPY src/ src/

# Build và publish project
RUN dotnet publish src/Backend.WebApi/Backend.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Run stage (sử dụng image nhỏ gọn chỉ chứa ASP.NET runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port mặc định của .NET 8/9/10 là 8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Backend.WebApi.dll"]
