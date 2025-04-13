# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the csproj first (for efficient caching of restore)
COPY ["BasicFacilitiesNode/BasicFacilitiesNode.csproj", "BasicFacilitiesNode/"]
RUN dotnet restore "BasicFacilitiesNode/BasicFacilitiesNode.csproj"

# Copy only the BasicFacilitiesNode code
#COPY BasicFacilitiesNode/. ./BasicFacilitiesNode/
COPY . .
WORKDIR /src/BasicFacilitiesNode
RUN dotnet build "BasicFacilitiesNode.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BasicFacilitiesNode.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasicFacilitiesNode.dll"]
