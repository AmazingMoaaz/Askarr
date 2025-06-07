# Environment Variables in Askarr

Askarr now supports configuration through environment variables, making it easier to deploy and customize in different environments, especially when using Docker.

## Available Environment Variables

| Variable | Description | Default Value | Example |
|----------|-------------|---------------|---------|
| `ASKARR_PORT` | The port Askarr will listen on | `4545` | `ASKARR_PORT=8080` |
| `ASKARR_BASE_URL` | Base URL path for the application | Empty string | `ASKARR_BASE_URL=/askarr` |
| `ASKARR_CONFIG_DIR` | Directory for configuration files | `./config` or `/root/config` in Docker | `ASKARR_CONFIG_DIR=/app/config` |
| `ASKARR_DISABLE_AUTHENTICATION` | Whether to disable authentication | `false` | `ASKARR_DISABLE_AUTHENTICATION=true` |

## Using Environment Variables

### With Docker

When using Docker, you can set environment variables in the `docker-compose.yml` file:

```yaml
services:
  askarr:
    image: askarr
    ports:
      - "8080:4545"
    environment:
      - ASKARR_PORT=4545
      - ASKARR_BASE_URL=/askarr
      - ASKARR_CONFIG_DIR=/root/config
```

Or when running with `docker run`:

```bash
docker run -p 8080:4545 -e ASKARR_PORT=4545 -e ASKARR_BASE_URL=/askarr -v ./config:/root/config askarr
```

### Without Docker

When running the application directly, you can set environment variables before starting the application:

#### Windows (PowerShell)

```powershell
$env:ASKARR_PORT = "8080"
$env:ASKARR_BASE_URL = "/askarr"
dotnet run
```

#### Linux/macOS

```bash
ASKARR_PORT=8080 ASKARR_BASE_URL=/askarr dotnet run
```

## Command Line Arguments vs Environment Variables

Command line arguments take precedence over environment variables. If both are specified, the command line arguments will be used.

For example, if you run:

```bash
ASKARR_PORT=8080 dotnet Askarr.WebApi.dll -p 9090
```

The application will use port 9090, not 8080.

## Order of Precedence

Configuration values are resolved in the following order (highest priority first):

1. Command line arguments
2. Environment variables
3. Settings in configuration file
4. Default values 