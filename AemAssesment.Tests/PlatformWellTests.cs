using System.Text.Json.Nodes;
using AemAssesment.DB;
using AemAssesment.Services;
using Microsoft.EntityFrameworkCore;

namespace AemAssesment.Tests;

public class PlatformWellImporterTests
{
    private static DBContext CreateDb() => new(
        new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static JsonNode? Json(string json) =>
        JsonNode.Parse(json);

    [Fact]
    public async Task Inserts_new_platform_and_well()
    {
        await using var db = CreateDb();
        var root = Json("""
            [{ "id": 1, "uniqueName": "Platform1",
               "well": [{ "id": 11, "platformId": 1, "uniqueName": "Well11" }] }]
            """);

        var (platforms, wells) = await PlatformWellServices.Import(db, root);

        Assert.Equal((1, 1), (platforms, wells));
        Assert.Equal("Platform1", (await db.Platforms.SingleAsync()).UniqueName);
        Assert.Equal("Well11", (await db.Wells.SingleAsync()).UniqueName);
    }

    [Fact]
    public async Task Updates_existing_rows_by_id()
    {
        await using var db = CreateDb();
        db.Platforms.Add(new Platform { Id = 1, UniqueName = "Old" });
        db.Wells.Add(new Well { Id = 11, PlatformId = 1, UniqueName = "OldWell" });
        await db.SaveChangesAsync();

        var root = Json("""
            [{ "id": 1, "uniqueName": "New", "well": [{ "id": 11, "uniqueName": "NewWell" }] }]
            """);

        await PlatformWellServices.Import(db, root);

        Assert.Equal("New", (await db.Platforms.SingleAsync()).UniqueName);
        Assert.Equal("NewWell", (await db.Wells.SingleAsync()).UniqueName);
    }
}