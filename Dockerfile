# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["PosSelection.csproj", "./"]
RUN dotnet restore "PosSelection.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "PosSelection.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PosSelection.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1


COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PosSelection.dll"]

