using LiteDB;
using Vulcanova.Features.Auth;
using Xamarin.Essentials;

namespace Vulcanova.Core.Data.Migrations;

public sealed class ClearAccountsSyncDate2 : LiteDbMigration
{
    protected override int ToVersion => 6;
    public override int RequiredVersion => 5;

    protected override void Up(ILiteDatabase db)
    {
        Preferences.Remove("LastSync_AccountsSync");
    }

    protected override void Down(ILiteDatabase db)
    {
    }
}