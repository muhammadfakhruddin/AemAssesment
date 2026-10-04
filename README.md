# AemAssesment

ASP.NET Core Web API (.NET 10) that syncs platform and well data from the AEM Enersol test API into SQL Server LocalDB using EF Core Code First.

## Run

Requires the .NET 10 SDK and SQL Server LocalDB.

```bash
dotnet run --project AemAssesment
```

The database is created automatically on startup. Swagger: `http://localhost:5014/swagger`

## Endpoints

- `POST /api/sync?source=Actual|Dummy` - logs in, fetches the data and upserts Platform/Well by id (update if the id exists, insert if not). Missing keys keep the existing value, extra keys are ignored.
- `GET /api/last-updated-well` - last updated well for each platform.

## Tests

```bash
dotnet test
```
