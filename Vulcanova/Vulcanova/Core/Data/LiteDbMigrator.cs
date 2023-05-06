using System.Linq;
using LiteDB;

namespace Vulcanova.Core.Data;

public class LiteDbMigrator
{
    public static void Migrate(ILiteDatabase db)
    {
        var migrations = AssemblyMigrationsProvider.GetAllMigrations();

        if (migrations.GroupBy(m => m.RequiredVersion).Any(g => g.Count() > 1))
        {
            throw new MigrationException("Every migration must require unique database version");
        }

        while (migrations.SingleOrDefault(x => x.RequiredVersion == db.UserVersion) is { } next)
        {
            next.MigrateUp(db);
        }
    }
}