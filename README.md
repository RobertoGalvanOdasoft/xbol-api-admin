# XBOL Admin API

API for the XBOL admin applications

## Development Setup

This refers to development using Visual Studio 2026 on Windows, or using the .NET 10 SDK through the command line.

### Requirements

- [Visual Studio 2026](https://visualstudio.microsoft.com/insiders/) (Windows)
- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (Linux)
- PostgreSQL
- [Docker](https://www.docker.com/) or [Podman](https://podman.io/) (for development services)

### Quick Start

In Visual Studio, set **Odasoft.XBOL.AdminAPI** and **Odasoft.XBOL.WorkerService** as Startup Projects and press `F5`.

For the command-line interface:

**Start the Admin API:**

```bash
dotnet watch --project Odasoft.XBOL.AdminAPI/Odasoft.XBOL.AdminAPI
```

**Start the Worker Service (in a separate terminal):**

```bash
dotnet run --project Odasoft.XBOL.AdminAPI/Odasoft.XBOL.WorkerService --launch-profile "Odasoft.XBOL.WorkerService"
```

### Build & Compilation

To build the entire solution (API, Worker, and shared libraries):

```bash
dotnet build Odasoft.XBOL.AdminAPI/Odasoft.XBOL.AdminAPI.slnx
```

### Email

Start the local SMTP server for email testing:

```bash
make dev
```

This runs [smtp4dev](https://github.com/rnwood/smtp4dev) in Docker. View captured emails at <http://localhost:8025>.

To stop: `make dev-stop`

### Configuration

Edit `appsettings.Development.json` for local settings (connection strings, service URLs, etc.). Settings cascade: `appsettings.json` → `appsettings.{Environment}.json` → environment variables. All settings are validated at startup.

IDE autocomplete is provided by `appsettings.schema.json`, which regenerates automatically on Debug builds.

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

#### Email

Configure SendGrid SMTP relay via environment variables:

```
Smtp__Host=smtp.sendgrid.net
Smtp__Port=587
Smtp__Username=apikey
Smtp__Password=<SendGrid API key>
Smtp__FromAddress=noreply@yourdomain.com
Smtp__FromName=XBOL
```
