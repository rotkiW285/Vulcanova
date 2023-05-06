using LiteDB;
using Vulcanova.Features.Attendance.Report;

namespace Vulcanova.Core.Data.Migrations;

public class UpdateAttendanceReportsPk : LiteDbMigration
{
    protected override int ToVersion { get; } = 1;
    public override int RequiredVersion { get; } = 0;

    protected override void Up(ILiteDatabase db)
    {
        var collection = db.GetCollection(nameof(AttendanceReport));
        
        var documents = collection.FindAll();
        
        foreach (var doc in documents)
        {
            collection.Delete(doc["_id"]);

            var subject = doc["Subject"].AsDocument;
            var subjectId = subject["_id"].AsInt32;
            var newId = new BsonValue($"{doc["AccountId"].AsInt32}/{subjectId}");

            if (collection.Exists(d => d["_id"] == newId))
            {
                continue;
            }

            doc["_id"] = newId;

            collection.Insert(doc);
        }
    }

    protected override void Down(ILiteDatabase db)
    {
        throw new System.NotImplementedException();
    }
}