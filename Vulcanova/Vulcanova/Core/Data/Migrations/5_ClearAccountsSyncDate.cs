using LiteDB;
using Vulcanova.Features.Auth;
using Xamarin.Essentials;

namespace Vulcanova.Core.Data.Migrations;

public sealed class ClearAccountsSyncDate : LiteDbMigration
{
    protected override int ToVersion => 5;
    public override int RequiredVersion => 4;

    protected override void Up(ILiteDatabase db)
    {
        Preferences.Remove("LastSync_AccountsSync");
    }

    protected override void Down(ILiteDatabase db)
    {
    }
}