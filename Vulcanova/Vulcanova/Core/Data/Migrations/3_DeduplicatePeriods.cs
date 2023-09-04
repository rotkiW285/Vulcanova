using System.Linq;
using LiteDB;
using Vulcanova.Features.Auth.Accounts;

namespace Vulcanova.Core.Data.Migrations;

public class DeduplicatePeriods : LiteDbMigration
{
    protected override int ToVersion => 3;
    public override int RequiredVersion => 2;

    protected override void Up(ILiteDatabase db)
    {
        var collection = db.GetCollection<Account>();
        var accounts = collection.FindAll();
        foreach (var account in accounts)
        {
            account.Periods = account.Periods.GroupBy(p => p.Id).Select(g => g.First()).ToList();
            collection.Update(account);
        }
    }

    protected override void Down(ILiteDatabase db)
    {
    }
}