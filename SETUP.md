# Quick Setup Guide - Turtles Organizer

This guide will help you get the Turtles Organizer application up and running quickly.

## Prerequisites

1. **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **PostgreSQL** - [Download here](https://www.postgresql.org/download/)
   - Default connection assumes: Host=localhost, Database=turtlesorganizer, User=postgres, Password=postgres

## Setup Steps

### 1. Configure Database Connection

Edit `src/TurtlesOrganizer.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=turtlesorganizer;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  }
}
```

### 2. Create the Database

Option A: Using PostgreSQL command line:
```bash
createdb turtlesorganizer
```

Option B: Using pgAdmin or another GUI tool to create a database named `turtlesorganizer`.

### 3. Apply Database Migrations

From the root directory of the solution:

```bash
dotnet ef database update --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web
```

This will create all necessary tables in your PostgreSQL database.

### 4. Run the Application

```bash
dotnet run --project src/TurtlesOrganizer.Web
```

Or open the solution in Visual Studio and press F5.

### 5. Access the Application

Open your browser and navigate to:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

## First Steps

1. **Register a User**: Go to `/register` or click "Register" in the navigation
2. **Login**: Use your registered credentials at `/login`
3. **Create Persons**: You'll need to create persons (trainers/attendees) before creating sessions
4. **Create a Training**: Click "Create Training" to add a new training topic
5. **Add Sessions**: From a training details page, click "Add Session" to schedule training sessions
6. **Register Attendees**: From any open session, click "Register" to sign up attendees

## Use Case Flows

### Register & Login
1. Navigate to `/register`
2. Fill in full name, email, and password
3. Click "Register"
4. Navigate to `/login`
5. Enter your credentials

### Create a Training
1. Navigate to `/trainings`
2. Click "Create Training"
3. Enter topic and description
4. Click "Create Training"

### Create a Training Session
1. From the training details page
2. Click "Add Session"
3. Fill in title, date/time, select trainer, and set max attendees
4. Click "Create Session"

### Register as Attendee
1. View any training with open sessions
2. Click "Register" on an available session
3. Select the person to register
4. Click "Register"

## Troubleshooting

### Database Connection Issues
- Verify PostgreSQL is running
- Check connection string credentials
- Ensure the database exists

### Migration Issues
If migrations fail, you can remove and recreate:
```bash
# Remove migration (if needed)
dotnet ef migrations remove --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web

# Create fresh migration
dotnet ef migrations add InitialCreate --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web

# Apply to database
dotnet ef database update --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web
```

### Port Already in Use
If ports 5000/5001 are in use, modify `src/TurtlesOrganizer.Web/Properties/launchSettings.json` to use different ports.

## Project Structure Overview

```
src/
├── TurtlesOrganizer.Domain/          # Business entities and rules
│   ├── Aggregates/                   # Aggregate roots (Training)
│   ├── Entities/                     # Domain entities (User, Person)
│   ├── ValueObjects/                 # Value objects (Email)
│   └── Repositories/                 # Repository interfaces
├── TurtlesOrganizer.Application/     # Use cases and DTOs
│   ├── Services/                     # Application services
│   └── DTOs/                         # Data transfer objects
├── TurtlesOrganizer.Infrastructure/  # Data access
│   ├── Persistence/                  # DbContext and configurations
│   └── Repositories/                 # Repository implementations
└── TurtlesOrganizer.Web/            # Blazor UI
    └── Components/Pages/             # Razor pages
```

## Next Steps

- Add proper authentication/authorization (currently simplified)
- Integrate SSO for enterprise authentication
- Add reporting and analytics
- Implement email notifications
- Add calendar integration
- Enhance UI/UX with custom styling

## Support

For issues or questions, please refer to the README.md or create an issue in the repository.
