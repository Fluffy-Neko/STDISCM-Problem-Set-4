# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the project file first (cache optimization)
COPY ["StudentNode/StudentNode.csproj", "StudentNode/"]
RUN dotnet restore "StudentNode/StudentNode.csproj"

# Copy only the StudentNode project files
#COPY StudentNode/. ./StudentNode/
COPY . .
WORKDIR /src/StudentNode
RUN dotnet build "StudentNode.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish step
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StudentNode.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentNode.dll"]
