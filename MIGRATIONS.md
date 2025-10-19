# Database Migration Strategy

## Approach: Application Startup Migrations

This application uses **automatic database migrations on application startup** rather than running migrations in the Dockerfile or as a separate init container.

## Implementation

Located in `src/TurtlesOrganizer.Web/Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Applying database migrations...");
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations");
        throw; // Fail fast if migrations fail
    }
}
```

## Why This Approach?

### ✅ Advantages

1. **Idiomatic .NET**
   - Standard practice in ASP.NET Core applications
   - Recommended by Microsoft documentation
   - Widely used in the .NET community

2. **Cleaner Code**
   - Migration logic is in C# where it belongs
   - Easy to read and maintain
   - Integrated with application logging and DI

3. **Better Error Handling**
   - Exceptions are properly caught and logged
   - Application fails fast if migrations fail
   - Clear error messages in application logs

4. **Simplified Deployment**
   - No need for separate migration containers
   - No shell scripts in Dockerfile
   - Single application container handles everything

5. **Development Experience**
   - Works identically in development and production
   - Easy to debug with standard debugging tools
   - No switching between tools/contexts

6. **Scale-Out Safe**
   - EF Core handles concurrent migration attempts
   - Built-in locking prevents duplicate executions
   - Same behavior as Dockerfile approach (no difference in safety)

### ❌ Dockerfile Migration Disadvantages

1. **Mixed Concerns**
   - Shell scripting in Dockerfile
   - Harder to test and debug
   - Requires `dotnet-ef` tool in production image

2. **Reduced Visibility**
   - Migration logs separate from application logs
   - Harder to monitor and troubleshoot
   - Errors may not be obvious

3. **Image Bloat**
   - Need to include SDK or `dotnet-ef` tool
   - Increases image size
   - Security surface increases

4. **Complexity**
   - Custom entrypoint scripts
   - Sleep/wait logic for database readiness
   - More moving parts to maintain

## Scale-Out Considerations

Both approaches handle scale-out identically:

- **Multiple instances starting simultaneously**: EF Core's `Database.Migrate()` uses database-level locking
- **First instance wins**: Other instances wait or skip if already applied
- **No duplicate migrations**: EF Core tracks applied migrations in `__EFMigrationsHistory` table

## Alternative: Manual Migrations

For production environments where you want explicit control:

### Option 1: Pre-deployment Script

```bash
# In CI/CD pipeline before deployment
dotnet ef database update --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web
```

Then disable automatic migrations in `Program.cs`.

### Option 2: Init Container (Kubernetes)

```yaml
initContainers:
  - name: migrate
    image: your-app:latest
    command: 
      - dotnet
      - ef
      - database
      - update
```

### Option 3: Separate Migration Job

Run a Kubernetes Job or similar that executes migrations before rolling out new app versions.

## When to Use Manual Migrations

Consider manual migrations if:

- You have strict change control processes
- You want to review migrations in production before applying
- You need to coordinate with other systems during migration
- You're working with very large databases where migrations are risky
- You have complex rollback requirements

## Best Practices

Regardless of approach:

1. **Test migrations** in non-production environments first
2. **Backup databases** before major migrations
3. **Monitor migration logs** during deployment
4. **Have rollback procedures** documented
5. **Use migration scripts** for complex data transformations
6. **Version your migrations** properly

## Conclusion

**For this application, automatic startup migrations are the right choice** because:

- It's a standard .NET application pattern
- Code is cleaner and more maintainable
- Development and production behave identically
- No operational complexity added
- Scale-out is handled properly by EF Core

If your organization has different requirements (strict change control, manual approval, etc.), you can easily disable automatic migrations and use one of the manual approaches described above.
