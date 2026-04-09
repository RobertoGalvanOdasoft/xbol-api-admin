# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files for restore
COPY ["Odasoft.XBOL.WorkerService/Odasoft.XBOL.WorkerService.csproj", "Odasoft.XBOL.WorkerService/"]
COPY ["Odasoft.XBOL.Business/Odasoft.XBOL.Business.csproj", "Odasoft.XBOL.Business/"]
COPY ["Odasoft.XBOL.Data/Odasoft.XBOL.Data.csproj", "Odasoft.XBOL.Data/"]
COPY ["Odasoft.XBOL.Models/Odasoft.XBOL.Models.csproj", "Odasoft.XBOL.Models/"]
COPY ["Odasoft.XBOL.DTO/Odasoft.XBOL.DTO.csproj", "Odasoft.XBOL.DTO/"]
COPY ["Odasoft.XBOL.Commons/Odasoft.XBOL.Commons.csproj", "Odasoft.XBOL.Commons/"]

# Restore dependencies
RUN dotnet restore "Odasoft.XBOL.WorkerService/Odasoft.XBOL.WorkerService.csproj"

# Copy entire source tree
COPY . .

# Build the worker project
WORKDIR "/src/Odasoft.XBOL.WorkerService"
RUN dotnet build "Odasoft.XBOL.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Odasoft.XBOL.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

ARG DOCKER_IMAGE_VERSION="0.0.0.0.1"
ENV DOCKER_IMAGE_VERSION=$DOCKER_IMAGE_VERSION

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD pgrep -f "Odasoft.XBOL.WorkerService" || exit 1

ENTRYPOINT ["dotnet", "Odasoft.XBOL.WorkerService.dll"]
