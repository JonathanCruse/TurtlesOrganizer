# TurtlesOrganizer

A .NET 9 web application built with Blazor for managing training sessions and attendees. The application follows Domain-Driven Design (DDD) principles and uses PostgreSQL for data persistence.

## Features

- **User Management**
  - User registration
  - Login authentication (internal)
  
- **Training Management**
  - Create trainings
  - Create training sessions for trainings
  - View all trainings and their sessions
  
- **Attendee Management**
  - Register as an attendee for training sessions
  - Track session capacity and availability

## Architecture

The application is structured following Domain-Driven Design principles:

- **Domain Layer**: Contains entities, value objects, aggregates, and repository interfaces
- **Application Layer**: Contains use cases, DTOs, and service implementations
- **Infrastructure Layer**: Contains database context, repository implementations, and data persistence logic
- **Web Layer**: Blazor Server UI components and pages

## Prerequisites

- .NET 9 SDK
- PostgreSQL database server
- Visual Studio 2022 or VS Code

## Getting Started

You have two options to run the application:

### Option 1: Docker (Recommended)

The easiest way to get started. See [DOCKER.md](DOCKER.md) for details.

```bash
docker-compose up --build
```

Then navigate to http://localhost:8080

### Option 2: Local Development

#### 1. Update Database Connection

Edit `src/TurtlesOrganizer.Web/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=turtlesorganizer;Username=your_username;Password=your_password"
  }
}
```

#### 2. Create the Database

```bash
createdb turtlesorganizer
```

#### 3. Run the Application

```bash
dotnet run --project src/TurtlesOrganizer.Web
```

**Note**: Database migrations are applied automatically on startup. You don't need to run `dotnet ef database update` manually unless you prefer to do so before starting the application

### 4. Run the Application

```bash
dotnet run --project src/TurtlesOrganizer.Web
```

Navigate to `https://localhost:5001` in your browser.

## Project Structure

```
TurtlesOrganizer/
├── src/
│   ├── TurtlesOrganizer.Domain/          # Domain entities and interfaces
│   │   ├── Aggregates/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Repositories/
│   │   └── Common/
│   ├── TurtlesOrganizer.Application/     # Application services and DTOs
│   │   ├── Services/
│   │   └── DTOs/
│   ├── TurtlesOrganizer.Infrastructure/  # Data access implementation
│   │   ├── Persistence/
│   │   └── Repositories/
│   └── TurtlesOrganizer.Web/            # Blazor UI
│       └── Components/
│           └── Pages/
└── TurtlesOrganizer.sln
```

## Use Cases

1. **Register**: Create a new user account
2. **Login**: Authenticate with email and password
3. **Create Training**: Create a new training topic
4. **Create Training Session**: Schedule sessions for a training
5. **Register as Attendee**: Sign up for a training session

## Technology Stack

- **.NET 9**: Latest version of .NET
- **Blazor Server**: Interactive web UI framework
- **Entity Framework Core**: ORM for database access
- **PostgreSQL**: Relational database
- **Bootstrap 5**: CSS framework for styling

## Future Enhancements

- SSO (Single Sign-On) authentication integration
- Advanced reporting and analytics
- Email notifications
- Calendar integration
- User roles and permissions

## License

See LICENSE file for details.
