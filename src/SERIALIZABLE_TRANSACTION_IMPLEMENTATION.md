# Serializable Transaction Implementation for Race Condition Prevention

## Overview

Implemented **database transaction with Serializable isolation level** to prevent race conditions where two users could potentially book the same seat simultaneously.

## Changes Made

### 1. **IUnitOfWork Interface** (`Application.Contracts/Repositories/IUnitOfWork.cs`)

- Created a Unit of Work pattern interface to manage database transactions
- Supports different isolation levels (ReadCommitted, RepeatableRead, Serializable, etc.)
- Methods: `BeginTransactionAsync()`, `CommitTransactionAsync()`, `RollbackTransactionAsync()`

### 2. **UnitOfWork Implementation** (`Infrastructure/Repositories/UnitOfWork.cs`)

- Implements transaction management using Entity Framework Core
- Sets transaction isolation level using SQL command
- Handles transaction lifecycle (begin, commit, rollback)
- Proper cleanup and disposal of transaction resources

### 3. **BookingService Updates** (`Application/Services/BookingService.cs`)

- Injected `IUnitOfWork` dependency
- Wrapped seat booking logic in **Serializable transaction**
- Added proper transaction rollback on validation failures
- Transaction commits only after all operations succeed

### 4. **Dependency Injection** (`WebApi/Program.cs`)

- Registered `IUnitOfWork` and `UnitOfWork` in DI container

## How It Prevents Race Conditions

### Before (VULNERABLE):

```
User A: Read seat (Status = Available) ✓
User B: Read seat (Status = Available) ✓  ← BOTH SEE AVAILABLE!
User A: Book seat (Status = Booked) ✓
User B: Book seat (Status = Sold) ✓      ← DOUBLE BOOKING!
```

### After (PROTECTED):

```
User A: Begin Serializable Transaction → LOCK SEAT ROW
User A: Read seat (Status = Available) ✓
User B: Begin Serializable Transaction → WAIT (blocked)
User A: Book seat (Status = Sold) ✓
User A: Commit Transaction → RELEASE LOCK
User B: Read seat (Status = Sold)
User B: Validation fails - seat not available ✓
User B: Rollback Transaction
```

## Key Features

### Serializable Isolation Level

- **Highest isolation level** in database transactions
- Prevents phantom reads, non-repeatable reads, and dirty reads
- Locks rows being read/modified until transaction completes
- Ensures complete transaction isolation

### Transaction Flow

1. **Begin Transaction** with Serializable isolation
2. **Lock seat row** when reading with `GetWithDetailsAsync()`
3. **Validate seat availability** (no other transaction can modify it)
4. **Perform booking operations** (create ticket, update seat)
5. **Commit transaction** (save changes and release locks)
6. **Rollback on any error** (maintain data consistency)

### Benefits

✅ **Prevents double booking** - Guaranteed single seat assignment  
✅ **Data consistency** - All-or-nothing transaction semantics  
✅ **Automatic rollback** - On any validation or system error  
✅ **Thread-safe** - Works with multiple concurrent requests  
✅ **Database-level protection** - Independent of application instances

### Trade-offs

⚠️ **Performance impact** - Serializable transactions can reduce throughput  
⚠️ **Lock contention** - Concurrent requests for same seat will wait  
⚠️ **Deadlock potential** - Multiple transactions can create deadlocks (rare)

## Testing Recommendations

### Test Concurrent Bookings

```bash
# Simulate 2 users booking same seat simultaneously
# Both requests should be sent at exact same time
# Only one should succeed, other should fail with "not available"
```

### Load Testing

- Test with multiple concurrent users
- Monitor transaction wait times
- Check for deadlock scenarios
- Verify seat availability accuracy

## Alternative Approaches (Not Implemented)

### Option 1: Optimistic Concurrency Control

- Add `RowVersion` property to `Seat` entity
- Detect concurrent modifications via version mismatch
- Lighter weight but requires retry logic

### Option 2: Pessimistic Locking

- Use `WITH (UPDLOCK, ROWLOCK)` in SQL queries
- More granular control over locks
- Requires raw SQL queries

## Monitoring

Watch for these in production:

- Transaction timeout errors
- Deadlock exceptions
- Long-running transactions blocking others
- Database lock wait statistics

## Database Compatibility

This implementation works with:

- ✅ **PostgreSQL** (current project database)
- ✅ **SQL Server**
- ✅ **MySQL** (syntax may differ slightly)
- ✅ **SQLite** (limited isolation support)
