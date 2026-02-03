**Note:** The assessment asked for SQL Server LocalDB, but that only works on Windows. Since I'm using a Mac, I went with SQLite instead and it works the same way for this project.

## SQL Query

Fetch the latest well for each platform:

```sql
SELECT 
    p.Id,
    p.PlatformName,
    w.Id AS WellId,
    w.UniqueName,
    w.UpdatedAt
FROM 
    Platforms p
INNER JOIN 
    Wells w ON p.Id = w.PlatformId
INNER JOIN (
    SELECT 
        PlatformId,
        MAX(UpdatedAt) AS MaxUpdatedAt
    FROM 
        Wells
    GROUP BY 
        PlatformId
) latest ON w.PlatformId = latest.PlatformId 
    AND w.UpdatedAt = latest.MaxUpdatedAt
ORDER BY 
    p.Id;

**To run it:**
sqlite3 platformwell.db < query.sql

## Testing
dotnet run
Will sync all platforms and wells successfully.

### View the Data
# Count records
sqlite3 platformwell.db "SELECT COUNT(*) FROM Platforms;"
sqlite3 platformwell.db "SELECT COUNT(*) FROM Wells;"

# View all data
sqlite3 platformwell.db "SELECT * FROM Platforms;"
sqlite3 platformwell.db "SELECT * FROM Wells;"

# Time Spent

**Total: 4 hours**

- Project setup & API understanding : 1 hr
- TModels, database context & API service : 1.5 hr
- Sync logic (insert/update) & error handling : 1.5 hr
- Testing, debugging & SQL query : 2 hr

Most of the time went into:
- Figuring out the actual API response structure (the field names were different than expected)
- Making sure the insert/update logic worked correctly
- Handling nullable fields properly
- Testing with different data scenarios

## API Info

**Base URL:** http://test-demo.aemenersol.com
**Username:** user@aemenersol.com
**Password:** Test@123
**Endpoints used:**
  - POST `/api/Account/Login` - To get bearer token
  - GET `/api/PlatformWell/GetPlatformWellActual` - To fetch real data
  - GET `/api/PlatformWell/GetPlatformWellDummy` - To test data with missing fields

Assessment completed: 4 February 2026
