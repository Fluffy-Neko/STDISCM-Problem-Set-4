# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the project file to optimize restore caching
COPY ["InstructorNode/InstructorNode.csproj", "InstructorNode/"]
RUN dotnet restore "InstructorNode/InstructorNode.csproj"

# Copy ONLY the source code for InstructorNode
# COPY InstructorNode/. ./InstructorNode/
COPY . .
WORKDIR /src/InstructorNode
RUN dotnet build "InstructorNode.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "InstructorNode.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InstructorNode.dll"]
