using System;
using LiteDB;
using Vulcanova.Core.Data;

namespace Vulcanova.Features.Notes;

public class Note
{
    public AccountEntityId Id { get; set; }
    public Guid Key { get; set; }
    public int PupilId { get; set; }
    public string CreatorName { get; set; }
    public string Content { get; set; }
    public int? Points { get; set; }
    public string CategoryName { get; set; }
    public string CategoryType { get; set; }
    public bool Positive { get; set; }
    public DateTime DateModified { get; set; }

    [BsonIgnore]
    public bool? NoteOrCategoryPositive
    {
        get
        {
            if (Positive || CategoryType == "pozytywna") return true;
            if (CategoryType == "negatywna") return false;
            return null;
        }
    }
}