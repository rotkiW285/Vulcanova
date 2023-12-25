using System;
using Vulcanova.Core.Data;

namespace Vulcanova.Features.Grades;

public class Grade
{
    public AccountEntityId Id { get; set; }
    public string CreatorName { get; set; }
    public int PupilId { get; set; }
    public string ContentRaw { get; set; }
    public string Content { get; set; }
    public string Comment { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime DateModify { get; set; }
    public bool IsModified => DateCreated != DateModify;
    public decimal? Value { get; set; }
    public Column Column { get; set; }
}