using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Vulcanova.Features.Exams;
using Vulcanova.Features.Grades;
using Vulcanova.Features.Grades.Final;
using Vulcanova.Features.Homework;
using Vulcanova.Features.Messages;
using Vulcanova.Features.Notes;
using Vulcanova.Features.Timetable;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Core.Data.Migrations;

public class ChangeEntitiesIdType : LiteDbMigration
{
    protected override int ToVersion => 4;
    public override int RequiredVersion => 3;
    
    private static Type[] collectionTypesToAlter = new[]
    {
        typeof(Exam), typeof(Grade), typeof(FinalGradesEntry), typeof(AverageGrade), typeof(Homework),
        typeof(MessageBox), typeof(Note), typeof(TimetableEntry), typeof(TimetableChangeEntry)
    };

    protected override void Up(ILiteDatabase db)
    {
        db.BeginTrans();

        // Message entity doesn't have an AccountId property, but we can get it from MessageBox...
        // Message (MessageBoxId) -> MessageBox (GlobalKey)
        var messagesCollection = db.GetCollection("Message");
        var messageBoxAccountIdMap = db.GetCollection("MessageBox")
            .FindAll()
            .ToDictionary(d => d["GlobalKey"].AsGuid, d => d["AccountId"].AsInt32);

        var allMessages = messagesCollection.FindAll().ToArray();
        messagesCollection.DeleteAll();

        foreach (var message in allMessages)
        {
            if (!messageBoxAccountIdMap.TryGetValue(message["MessageBoxId"].AsGuid, out var accountId))
            {
                // orphaned message?
                accountId = 0;
            }
            
            message["_id"] = new BsonDocument(new Dictionary<string, BsonValue>()
            {
                { "VulcanId", message["_id"] },
                { "AccountId", new BsonValue(accountId) }
            });
        
            messagesCollection.Insert(message);
        }
        
        // the other entities that have an AccountId property can be easily migrated
        foreach (var collectionType in collectionTypesToAlter)
        {
            var collection = db.GetCollection(collectionType.Name);

            var items = collection.FindAll().ToArray();
            collection.DeleteAll();

            foreach (var item in items)
            {
                item["_id"] = new BsonDocument(new Dictionary<string, BsonValue>()
                {
                    { "VulcanId", item["_id"] },
                    { "AccountId", item["AccountId"] }
                });

                item.Remove("AccountId");
                collection.Insert(item);
            }
        }

        db.Commit();
    }

    protected override void Down(ILiteDatabase db)
    {
        db.BeginTrans();

        foreach (var collectionType in collectionTypesToAlter.Append(typeof(Message)))
        {
            var collection = db.GetCollection(collectionType.Name);

            var items = collection.FindAll().ToArray();
            collection.DeleteAll();

            foreach (var item in items)
            {
                if (collectionType != typeof(Message))
                {
                    item.Add("AccountId", item["_id.AccountId"]);
                }

                item["_id"] = item["_id.VulcanId"];

                collection.Insert(item);
            }
        }
        
        db.Commit();
    }
}