# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the .csproj file for restore (for caching efficiency)
COPY ["AuthNode/AuthNode.csproj", "AuthNode/"]
RUN dotnet restore "AuthNode/AuthNode.csproj"

# Copy only the AuthNode source code
#COPY AuthNode/. ./AuthNode/
COPY . .
WORKDIR /src/AuthNode
RUN dotnet build "AuthNode.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthNode.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthNode.dll"]
