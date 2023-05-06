using LiteDB;

namespace Vulcanova.Core.Data;

public abstract class LiteDbMigration
{
    protected abstract int ToVersion { get; }
    public abstract int RequiredVersion { get; }

    protected abstract void Up(ILiteDatabase db);
    protected abstract void Down(ILiteDatabase db);

    public void MigrateUp(ILiteDatabase db)
    {
        if (db.UserVersion != RequiredVersion)
        {
            throw new MigrationException("Can't migrate from this version");
        }

        Up(db);

        db.UserVersion = ToVersion;
    }

    public void MigrateDown(ILiteDatabase db)
    {
        if (db.UserVersion != ToVersion)
        {
            throw new MigrationException("Can't migrate from this version");
        }
        
        Down(db);

        db.UserVersion = RequiredVersion;
    }
}