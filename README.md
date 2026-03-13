# XBOL Admin API

API for the XBOL admin applications

## Development Setup

This refers to development using Visual Studio 2026 on Windows, or using the .NET 10 SDK through the command line.

### Requirements

- [Visual Studio 2026](https://visualstudio.microsoft.com/insiders/) (Windows)
- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (Linux)
- PostgreSQL

### Quick Start

In Visual Studio, press `F5` or the play button. For the command-line interface:

```powershell
dotnet watch --project Odasoft.XBOL.AdminAPI/Odasoft.XBOL.AdminAPI
```

### Configuration

Application settings are strongly typed via Options classes in `Odasoft.XBOL.AdminAPI/Options/`. All options are validated at startup using `DataAnnotations` and `ValidateOnStart()`.

A JSON Schema (`appsettings.schema.json`) is auto-generated on Debug builds for IDE autocomplete. To regenerate manually:

```bash
dotnet run --project Odasoft.XBOL.AdminAPI/Odasoft.XBOL.AdminAPI -- --generate-schema
```

## Deployment

The container is production-ready with:

- **Security**: Non-root `app` user
- **Optimization**: Release build with ReadyToRun compilation
- **Health checks**: Automatic container health monitoring
- **Restart policy**: `unless-stopped` for high availability
- **Environment**: `ASPNETCORE_ENVIRONMENT=Production`

#### Requirements

- Make
- [Podman](https://podman.io/) (or [Docker](https://www.docker.com/))
- [Podman Compose](https://docs.podman.io/en/latest/markdown/podman-compose.1.html) (or [Docker Compose](https://docs.docker.com/compose/))

#### Usage

```bash
make build    # Create the Docker container
make run      # Run the Docker Compose environment
```

**Access the containerized services**

- **API Base URL**: <http://localhost:8080>
- **API Health Check**: <http://localhost:8080/healthz>
