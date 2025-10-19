# IsTrainer Feature Implementation

## Overview

Added an `IsTrainer` boolean flag to the `Person` entity to identify which persons can act as trainers for training sessions. Only persons marked as trainers will be available for selection when creating training sessions.

## Problem Solved

**Before:** When creating a training session, the trainer dropdown was empty because there was no way to distinguish trainers from regular persons/attendees.

**After:** Persons can be marked as trainers, and only those marked will appear in the trainer selection dropdown.

## Changes Made

### 1. Domain Layer

#### **File: `Domain/Entities/Person.cs`**

Added `IsTrainer` property and methods:

```csharp
public class Person : Entity
{
    public string FullName { get; private set; }
    public Email Email { get; private set; }
    public Guid? MembershipId { get; private set; }
    public bool IsTrainer { get; private set; }  // ✅ NEW
    public bool IsMember => MembershipId.HasValue;
    public bool IsGuest => !MembershipId.HasValue;

    public Person(string fullName, Email email, Guid? membershipId = null, bool isTrainer = false)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        MembershipId = membershipId;
        IsTrainer = isTrainer;  // ✅ NEW
    }

    public void SetTrainerStatus(bool isTrainer)  // ✅ NEW
    {
        IsTrainer = isTrainer;
    }
}
```

**Benefits:**
- Encapsulation - Can only set trainer status through the method
- Validation ready - Can add business rules if needed
- Audit trail ready - Can add domain events

### 2. Application Layer

#### **File: `Application/DTOs/PersonDtos.cs`**

Updated DTOs to include `IsTrainer`:

```csharp
public record CreatePersonDto(string FullName, string Email, Guid? MembershipId, bool IsTrainer = false);

public record PersonDto(Guid Id, string FullName, string Email, Guid? MembershipId, bool IsMember, bool IsTrainer);
```

#### **File: `Application/Services/IPersonService.cs`**

Added `GetTrainersAsync()` method:

```csharp
public interface IPersonService
{
    Task<PersonDto> CreatePersonAsync(CreatePersonDto dto);
    Task<PersonDto?> GetPersonByIdAsync(Guid id);
    Task<IEnumerable<PersonDto>> GetAllPersonsAsync();
    Task<IEnumerable<PersonDto>> GetTrainersAsync();  // ✅ NEW
}
```

#### **File: `Application/Services/PersonService.cs`**

Implemented the new method:

```csharp
public async Task<IEnumerable<PersonDto>> GetTrainersAsync()
{
    var persons = await _personRepository.GetAllAsync();
    return persons.Where(p => p.IsTrainer).Select(MapToDto);
}

private PersonDto MapToDto(Person person) => new PersonDto(
    person.Id,
    person.FullName,
    person.Email.Value,
    person.MembershipId,
    person.IsMember,
    person.IsTrainer  // ✅ NEW
);
```

### 3. Infrastructure Layer

#### **File: `Infrastructure/Persistence/Configurations/PersonConfiguration.cs`**

Added EF Core mapping:

```csharp
builder.Property(p => p.IsTrainer)
    .IsRequired()
    .HasDefaultValue(false);
```

**Benefits:**
- Default value of `false` for existing records
- Required field (no nulls)
- Database constraint enforced

#### **Migration: `AddIsTrainerToPerson`**

Generated migration:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<bool>(
        name: "IsTrainer",
        table: "Persons",
        type: "boolean",
        nullable: false,
        defaultValue: false);
}
```

**What Happens:**
- Adds `IsTrainer` column to `Persons` table
- Sets default value to `false` for all existing persons
- Existing data is preserved

### 4. Presentation Layer

#### **File: `Web/Components/Pages/CreateSession.razor`**

Changed to load only trainers:

```csharp
// ❌ BEFORE:
protected override async Task OnInitializedAsync()
{
    persons = await PersonService.GetAllPersonsAsync();
    model.DateTime = DateTime.Now.AddDays(1);
}

// ✅ AFTER:
protected override async Task OnInitializedAsync()
{
    persons = await PersonService.GetTrainersAsync();  // Only trainers!
    model.DateTime = DateTime.Now.AddDays(1);
}
```

**User Experience:**
- Dropdown only shows persons marked as trainers
- No confusion with regular members
- Clear and focused selection

#### **File: `Web/Components/Pages/ManageTrainers.razor`** ✅ NEW

Created admin page to manage trainer status:

**Features:**
- View all persons with their trainer status
- Toggle trainer status with a button
- Shows member/guest status
- Real-time updates
- Success/error messages
- Statistics (X trainers out of Y persons)

**UI:**
```
+--------------------------------------------------+
| Manage Trainers                                  |
+--------------------------------------------------+
| Name          | Email       | Status  | Trainer  | Action         |
|---------------|-------------|---------|----------|----------------|
| John Doe      | john@...    | Member  | ✓        | Remove Trainer |
| Jane Smith    | jane@...    | Guest   | Not      | Make Trainer   |
+--------------------------------------------------+
| Trainers: 1 of 2 persons                         |
+--------------------------------------------------+
```

#### **File: `Web/Components/Layout/NavMenu.razor`**

Added navigation link:

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="admin/manage-trainers">
        <span class="bi bi-people-nav-menu" aria-hidden="true"></span> Manage Trainers
    </NavLink>
</div>
```

## Usage Workflow

### Step 1: Mark Persons as Trainers

1. **Navigate to:** `/admin/manage-trainers`
2. **View all persons** in the table
3. **Click "Make Trainer"** button for persons who should be trainers
4. **See success message:** "John Doe is now a trainer!"
5. **Status updates** immediately in the UI

### Step 2: Create Training Session

1. **Navigate to:** `/trainings/{id}/sessions/create`
2. **Trainer dropdown** now shows only persons marked as trainers
3. **Select a trainer** from the dropdown
4. **Fill other fields** and submit
5. **Session created** with selected trainer

### Step 3: Remove Trainer Status (if needed)

1. **Go back to:** `/admin/manage-trainers`
2. **Click "Remove Trainer"** for persons who should no longer train
3. **They disappear** from the trainer dropdown in CreateSession

## Database Schema

### Persons Table (After Migration)

```sql
CREATE TABLE "Persons" (
    "Id" UUID PRIMARY KEY,
    "FullName" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "MembershipId" UUID NULL,
    "IsTrainer" BOOLEAN NOT NULL DEFAULT FALSE  -- ✅ NEW COLUMN
);
```

### Applying the Migration

**Automatic (on startup):**
```csharp
// Program.cs already has auto-migration
db.Database.Migrate();
```

**Manual (via command line):**
```bash
dotnet ef database update --project src/TurtlesOrganizer.Infrastructure --startup-project src/TurtlesOrganizer.Web
```

## Data States

### Existing Data After Migration

All existing persons will have `IsTrainer = false` by default:

```
| Name       | Email           | IsTrainer |
|------------|-----------------|-----------|
| John Doe   | john@email.com  | false     |
| Jane Smith | jane@email.com  | false     |
```

### After Marking Trainers

Use the Manage Trainers page to update:

```
| Name       | Email           | IsTrainer |
|------------|-----------------|-----------|
| John Doe   | john@email.com  | true      |  ← Can train!
| Jane Smith | jane@email.com  | false     |  ← Regular person
```

## API Summary

### New Methods

#### `IPersonService.GetTrainersAsync()`
```csharp
Task<IEnumerable<PersonDto>> GetTrainersAsync();
```

**Returns:** Only persons where `IsTrainer = true`

**Used by:** CreateSession page to populate trainer dropdown

#### `Person.SetTrainerStatus(bool isTrainer)`
```csharp
void SetTrainerStatus(bool isTrainer);
```

**Sets:** The `IsTrainer` flag on a person

**Used by:** ManageTrainers page to toggle status

### Updated Methods

#### `CreatePersonDto` Constructor
```csharp
CreatePersonDto(string FullName, string Email, Guid? MembershipId, bool IsTrainer = false)
```

**New parameter:** `IsTrainer` with default value `false`

**Backward compatible:** Existing code still works

## Testing Checklist

### ✅ Test Migration
1. Start application
2. Check logs: "Database migrations applied successfully"
3. Verify `IsTrainer` column exists in database

### ✅ Test Manage Trainers Page
1. Navigate to `/admin/manage-trainers`
2. See all existing persons
3. Click "Make Trainer" on a person
4. See success message
5. Badge changes to "✓ Trainer"
6. Button changes to "Remove Trainer"

### ✅ Test Trainer Selection
1. Mark at least one person as trainer
2. Go to `/trainings/{id}/sessions/create`
3. Trainer dropdown shows marked trainers
4. Can select and create session
5. Unmark the trainer
6. Refresh CreateSession page
7. Trainer no longer in dropdown

### ✅ Test Validation
1. Go to CreateSession without any trainers marked
2. Dropdown only shows "Select Trainer"
3. Try to submit
4. See validation error: "Trainer is required"

## Future Enhancements

### Short Term
1. **Add authorization** - Only admins can access `/admin/manage-trainers`
2. **Bulk operations** - Mark multiple persons as trainers at once
3. **Search/filter** - Find specific persons in large lists

### Medium Term
1. **Trainer qualifications** - Track what subjects they can train
2. **Availability calendar** - Manage when trainers are available
3. **Trainer ratings** - Collect feedback from attendees

### Long Term
1. **Trainer dashboard** - Show their upcoming sessions
2. **Training materials** - Upload/manage session resources
3. **Certification tracking** - Manage trainer certifications
4. **Auto-assignment** - Suggest trainers based on availability/skills

## Benefits Summary

✅ **Clear separation** - Trainers vs regular persons/attendees  
✅ **Better UX** - Only relevant options in dropdown  
✅ **Flexible** - Can change trainer status anytime  
✅ **Scalable** - Ready for future trainer features  
✅ **Database integrity** - Default value protects existing data  
✅ **Easy management** - Simple UI to toggle status  
✅ **Backward compatible** - Existing code still works  

## Troubleshooting

### Issue: No trainers in dropdown
**Solution:** Go to `/admin/manage-trainers` and mark at least one person as trainer

### Issue: Migration not applied
**Solution:** Restart the application - auto-migration runs on startup

### Issue: Can't access ManageTrainers page
**Solution:** The route is `/admin/manage-trainers` (not `/manage-trainers`)

### Issue: Existing persons not showing
**Solution:** Create persons first, then mark them as trainers

## Summary

The `IsTrainer` feature provides a clean way to distinguish trainers from regular persons in the system. Combined with the Manage Trainers admin page, you can easily control who appears in the trainer selection dropdown when creating training sessions.

**Next Step:** Mark your first trainers at `/admin/manage-trainers` and try creating a training session!
