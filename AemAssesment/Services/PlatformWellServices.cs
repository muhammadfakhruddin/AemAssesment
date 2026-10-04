using System.Globalization;
using System.Text.Json.Nodes;
using AemAssesment.DB;
using Microsoft.EntityFrameworkCore;

namespace AemAssesment.Services
{
    public static class PlatformWellServices
    {
        // Records without an id are skipped, missing keys keep the existing value, extra keys are ignored.
        public static async Task<(int Platforms, int Wells)> Import(DBContext db, JsonNode? root, CancellationToken ct = default)
        {
            var platforms = await db.Platforms.ToDictionaryAsync(p => p.Id, ct);
            var wells = await db.Wells.ToDictionaryAsync(w => w.Id, ct);

            int platformCount = 0;
            int wellCount = 0;

            var platformNodes = (root as JsonArray)?.OfType<JsonObject>() ?? Enumerable.Empty<JsonObject>();

            foreach(var plat in platformNodes)
            {
                // Skip records without an id
                if(GetInt(plat, "id") is not int platformId) continue;

                if(!platforms.TryGetValue(platformId, out var platform))
                {
                    platform = new Platform { Id = platformId };
                    db.Platforms.Add(platform);
                    platforms[platformId] = platform;
                }

                platform.UniqueName = GetString(plat, "uniqueName") ?? platform.UniqueName;
                platform.Latitude = GetDouble(plat, "latitude") ?? platform.Latitude;
                platform.Longitude = GetDouble(plat, "longitude") ?? platform.Longitude;
                platform.CreatedAt = GetDate(plat, "createdAt") ?? platform.CreatedAt;
                platform.UpdatedAt = GetDate(plat, "updatedAt") ?? platform.UpdatedAt;
                platformCount++;

                foreach(var wel in (plat["well"] as JsonArray)?.OfType<JsonObject>() ?? Enumerable.Empty<JsonObject>())
                {
                    if(GetInt(wel, "id") is not int wellId) continue;

                    if(!wells.TryGetValue(wellId, out var well))
                    {
                        well = new Well { Id = wellId };
                        db.Wells.Add(well);
                        wells[wellId] = well;
                    }

                    well.PlatformId = GetInt(wel, "platformId") ?? platformId;
                    well.UniqueName = GetString(wel, "uniqueName") ?? well.UniqueName;
                    well.Latitude = GetDouble(wel, "latitude") ?? well.Latitude;
                    well.Longitude = GetDouble(wel, "longitude") ?? well.Longitude;
                    well.CreatedAt = GetDate(wel, "createdAt") ?? well.CreatedAt;
                    well.UpdatedAt = GetDate(wel, "updatedAt") ?? well.UpdatedAt;
                    wellCount++;
                }
            }

            await db.SaveChangesAsync(ct);
            return (platformCount, wellCount);
        }

        private static string? GetString(JsonObject o, string key) =>
            o[key] is JsonValue v ? v.ToString() : null;

        private static int? GetInt(JsonObject o, string key) =>
            int.TryParse(GetString(o, key), out var r) ? r : null;

        private static double? GetDouble(JsonObject o, string key) =>
            double.TryParse(GetString(o, key), NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : null;

        private static DateTime? GetDate(JsonObject o, string key) =>
            DateTime.TryParse(GetString(o, key), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var r) ? r : null;
    }
}