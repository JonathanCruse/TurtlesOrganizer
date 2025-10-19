# Docker Setup Guide

This guide explains how to run the Turtles Organizer application using Docker and Docker Compose.

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose (included with Docker Desktop)

## Quick Start

### 1. Build and Run with Docker Compose

From the root directory of the solution:

```bash
docker-compose up --build
```

This will:
- Start a PostgreSQL database container
- Build the web application
- Run database migrations automatically
- Start the web application

### 2. Access the Application

Open your browser and navigate to:
- **HTTP**: http://localhost:8080
- **HTTPS**: https://localhost:8081

### 3. Stop the Application

```bash
docker-compose down
```

To also remove the database volume:

```bash
docker-compose down -v
```

## Docker Configuration

### PostgreSQL Database

The PostgreSQL container is configured with:
- **Database Name**: `turtlesorganizer`
- **Username**: `turtlesadmin`
- **Password**: `TurtlesPass123!`
- **Port**: `5432` (mapped to host)
- **Volume**: `postgres-data` (persists data)

### Web Application

The web application:
- Runs on ports `8080` (HTTP) and `8081` (HTTPS)
- Automatically waits for PostgreSQL to be healthy
- **Runs database migrations on startup** (in Program.cs using EF Core's `Database.Migrate()`)
- Connects to PostgreSQL using the service name `postgres`

**Note**: Database migrations are handled in application startup code, not in the Dockerfile. This approach is:
- More idiomatic for .NET applications
- Cleaner and more maintainable
- Easier to debug and monitor
- Properly integrated with EF Core's migration tracking
- Safe for scale-out scenarios (EF Core handles concurrent migration attempts with locking)

## Docker Compose Services

### postgres
- Image: `postgres:16-alpine`
- Container Name: `turtlesorganizer-postgres`
- Healthcheck: Ensures database is ready before starting the web app
- Network: `turtles-network`

### turtlesorganizer.web
- Built from: `src/TurtlesOrganizer.Web/Dockerfile`
- Container Name: `turtlesorganizer-web`
- Depends on: `postgres` (waits for health check)
- Network: `turtles-network`

## Environment Variables

### Development (docker-compose.override.yml)

The following environment variables are set for development:
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_HTTP_PORTS=8080`
- `ASPNETCORE_HTTPS_PORTS=8081`
- `ConnectionStrings__DefaultConnection=Host=postgres;Database=turtlesorganizer;Username=turtlesadmin;Password=TurtlesPass123!`

### Production

For production, create a `docker-compose.prod.yml`:

```yaml
services:
  postgres:
    environment:
      POSTGRES_PASSWORD: <your-secure-password>
  
  turtlesorganizer.web:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=turtlesorganizer;Username=turtlesadmin;Password=<your-secure-password>
```

Run with:
```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Common Docker Commands

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f turtlesorganizer.web
docker-compose logs -f postgres
```

### Restart Services

```bash
# Restart all
docker-compose restart

# Restart specific service
docker-compose restart turtlesorganizer.web
```

### Execute Commands in Containers

```bash
# Access PostgreSQL
docker-compose exec postgres psql -U turtlesadmin -d turtlesorganizer

# Access web app container
docker-compose exec turtlesorganizer.web /bin/sh
```

### View Running Containers

```bash
docker-compose ps
```

### Rebuild Without Cache

```bash
docker-compose build --no-cache
docker-compose up
```

## Database Management

### About Automatic Migrations

The application automatically applies pending database migrations on startup (in `Program.cs`). This happens:
- Before the application starts accepting requests
- With proper error handling and logging
- Using EF Core's built-in migration locking to prevent concurrent execution issues

**Advantages of this approach:**
- No need for separate migration scripts or init containers
- Consistent with .NET best practices
- Works seamlessly in both development and production
- Properly logged and traceable
- Fails fast if migrations fail (application won't start)

### Manually Run Migrations (Optional)

If you prefer to run migrations manually (e.g., in production pipelines), you can disable automatic migrations in `Program.cs` and use:

```bash
dotnet ef database update --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web
```

### Backup Database

```bash
docker-compose exec postgres pg_dump -U turtlesadmin turtlesorganizer > backup.sql
```

### Restore Database

```bash
cat backup.sql | docker-compose exec -T postgres psql -U turtlesadmin turtlesorganizer
```

### Connect to Database from Host

The PostgreSQL port 5432 is exposed, so you can connect from your host machine:

```
Host: localhost
Port: 5432
Database: turtlesorganizer
Username: turtlesadmin
Password: TurtlesPass123!
```

## Troubleshooting

### Port Already in Use

If ports 8080 or 8081 are already in use, modify `docker-compose.override.yml`:

```yaml
services:
  turtlesorganizer.web:
    ports:
      - "9080:8080"
      - "9081:8081"
```

### Database Connection Issues

Check if PostgreSQL is healthy:
```bash
docker-compose ps
```

View PostgreSQL logs:
```bash
docker-compose logs postgres
```

### Migration Failures

If automatic migrations fail, run them manually:

```bash
docker-compose exec turtlesorganizer.web dotnet ef database update --verbose
```

### Reset Everything

To start fresh:

```bash
# Stop and remove containers, networks, and volumes
docker-compose down -v

# Remove images
docker-compose down --rmi all -v

# Rebuild and restart
docker-compose up --build
```

## Network Architecture

Both containers run in the `turtles-network` bridge network, allowing them to communicate using service names:

- Web app connects to PostgreSQL using hostname: `postgres`
- PostgreSQL is accessible from host via `localhost:5432`
- Web app is accessible from host via `localhost:8080` and `localhost:8081`

## Volumes

### postgres-data
- Persists PostgreSQL database files
- Located in Docker's default volume location
- Survives container restarts
- Removed with `docker-compose down -v`

## Development Workflow

### 1. Make Code Changes

Edit files locally in your IDE.

### 2. Rebuild and Restart

```bash
docker-compose up --build
```

### 3. View Logs

```bash
docker-compose logs -f turtlesorganizer.web
```

### 4. Test Changes

Navigate to http://localhost:8080

## Security Notes

⚠️ **Important for Production:**

1. **Change Default Passwords**: Update `TurtlesPass123!` in all configuration files
2. **Use Secrets Management**: Consider using Docker secrets or environment variables from a secure source
3. **Enable HTTPS**: Configure proper SSL certificates for production
4. **Network Isolation**: Use more restrictive network configurations
5. **Update Base Images**: Regularly update to latest security patches

## Performance Optimization

### Multi-stage Build
The Dockerfile uses multi-stage builds to minimize final image size.

### Health Checks
PostgreSQL has a health check ensuring it's ready before the web app starts.

### Connection Pooling
Entity Framework Core automatically handles connection pooling.

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
- [ASP.NET Core Docker Documentation](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
