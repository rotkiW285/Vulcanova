using LiteDB;
using Vulcanova.Features.Notes;

namespace Vulcanova.Core.Data.Migrations;

public class ClearNotesPointsAndCategoryType : LiteDbMigration
{
    protected override int ToVersion => 2;
    public override int RequiredVersion => 1;

    protected override void Up(ILiteDatabase db)
    {
        var collection = db.GetCollection(nameof(Note));
        
        var documents = collection.FindAll();
        
        foreach (var doc in documents)
        {
            doc["Points"] = BsonValue.Null;
            doc["CategoryType"] = BsonValue.Null;
            collection.Update(doc);
        }
    }

    protected override void Down(ILiteDatabase db)
    {
    }
}