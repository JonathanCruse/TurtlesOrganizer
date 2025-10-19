# Bug Fixes Applied

## Issue 1: Invalid Include Path Error

**Error Message:**
```
InvalidOperationException: Unable to find navigation '_sessions' specified in string based include path '_sessions'.
```

**Problem:**
The `TrainingRepository` was trying to use string-based `Include("_sessions")` to eagerly load the sessions collection, but EF Core couldn't resolve the private backing field directly in LINQ queries.

**Solution:**
Changed to use the Entry API to explicitly load collections through the public navigation property:

```csharp
// Before (BROKEN)
return await _context.Trainings
    .Include("_sessions")
    .ToListAsync();

// After (FIXED)
var trainings = await _context.Trainings.ToListAsync();
foreach (var training in trainings)
{
    await _context.Entry(training)
        .Collection(nameof(Training.Sessions))
        .LoadAsync();
}
return trainings;
```

**Files Changed:**
- `src/TurtlesOrganizer.Infrastructure/Repositories/TrainingRepository.cs`
  - `GetByIdWithSessionsAsync()`
  - `GetAllAsync()`
  - `GetByUserIdAsync()`

---

## Issue 2: Missing FormName Attribute

**Error Message:**
```
The POST request does not specify which form is being submitted. To fix this, ensure <form> elements have a @formname attribute with any unique value, or pass a FormName parameter if using <EditForm>.
```

**Problem:**
.NET 9 Blazor requires forms to be explicitly named for proper form handling and anti-forgery protection. Without the `FormName` attribute and `[SupplyParameterFromForm]` attribute, the framework can't properly bind form data.

**Solution:**
Added two things to each form:

1. **FormName attribute** on `<EditForm>`:
   ```razor
   <EditForm Model="@model" OnValidSubmit="HandleRegister" FormName="RegisterForm">
   ```

2. **[SupplyParameterFromForm] attribute** on the model property:
   ```csharp
   [SupplyParameterFromForm]
   private RegisterModel model { get; set; } = new();
   ```

**Files Changed:**
- `src/TurtlesOrganizer.Web/Components/Pages/Register.razor` - Added `FormName="RegisterForm"`
- `src/TurtlesOrganizer.Web/Components/Pages/Login.razor` - Added `FormName="LoginForm"`
- `src/TurtlesOrganizer.Web/Components/Pages/CreateTraining.razor` - Added `FormName="CreateTrainingForm"`
- `src/TurtlesOrganizer.Web/Components/Pages/CreateSession.razor` - Added `FormName="CreateSessionForm"`
- `src/TurtlesOrganizer.Web/Components/Pages/RegisterAttendee.razor` - Added `FormName="RegisterAttendeeForm"`

---

## Why These Changes?

### Entry API vs String-Based Include

**Benefits of Entry API approach:**
- Works with private backing fields and complex navigation properties
- More explicit and easier to debug
- Better compile-time checking (uses `nameof()` instead of strings)
- More flexible for conditional loading

**Trade-off:**
- Slightly more verbose
- Requires explicit loops for collections
- Multiple database calls (though minimal due to EF Core's change tracker)

**Alternative (if needed):**
You could configure the navigation in `TrainingConfiguration.cs` to use a public shadow property, but the Entry API is cleaner for DDD encapsulation.

### FormName Requirement in .NET 9

**Why .NET 9 requires this:**
- Enhanced security with explicit form identification
- Better support for multiple forms on same page
- Improved anti-forgery token handling
- Clearer intent in Blazor SSR scenarios

**Pattern:**
```csharp
// In Razor:
<EditForm Model="@model" OnValidSubmit="HandleSubmit" FormName="UniqueFormName">

// In @code:
[SupplyParameterFromForm]
private MyModel model { get; set; } = new();
```

---

## Verification

After applying these fixes:

✅ The `/trainings` page loads without errors
✅ Training sessions are properly loaded with their parent trainings
✅ All forms (Register, Login, Create Training, Create Session, Register Attendee) work correctly
✅ Form data is properly bound and submitted
✅ No compilation errors or warnings (except nullable reference warnings in Domain entities)

---

## Additional Notes

### Nullable Reference Warnings

The following warnings appear but are by design for EF Core entities:

```
warning CS8618: Non-nullable property 'FullName' must contain a non-null value when exiting constructor.
```

These are safe to ignore because:
- The parameterless constructor is only used by EF Core
- EF Core populates these properties from the database
- The public constructors properly initialize all required properties

To suppress these warnings, you could:
1. Add `#nullable disable` at the top of entity files
2. Add `required` modifier to properties (but this affects EF Core behavior)
3. Ignore them (recommended - they're false positives for EF Core entities)

---

## Testing Checklist

- [x] Navigate to `/trainings` - should load without errors
- [x] View training details - sessions should be visible
- [x] Register a new user - form should submit successfully
- [x] Login with credentials - form should submit successfully
- [x] Create a new training - form should submit successfully
- [x] Add a session to training - form should submit successfully
- [x] Register attendee for session - form should submit successfully
