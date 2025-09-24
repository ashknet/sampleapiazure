# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Emr.Api/Emr.Api.csproj", "Emr.Api/"]
COPY ["Emr.Application/Emr.Application.csproj", "Emr.Application/"]
COPY ["Emr.Domain/Emr.Domain.csproj", "Emr.Domain/"]
COPY ["Emr.Infrastructure/Emr.Infrastructure.csproj", "Emr.Infrastructure/"]

RUN dotnet restore "Emr.Api/Emr.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Emr.Api"
RUN dotnet build "Emr.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Emr.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -g 1000 dotnet && \
    useradd -r -u 1000 -g dotnet dotnet

# Copy published files
COPY --from=publish /app/publish .

# Change ownership
RUN chown -R dotnet:dotnet /app

# Switch to non-root user
USER dotnet

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Emr.Api.dll"]