using System;
using System.Collections.Generic;

namespace Vulcanova.Features.Notes;

public class NotesGroup : List<Note>
{
    public NotesGroup(DateTime date, IEnumerable<Note> notes) : base(notes)
    {
        Date = date;
    }

    public DateTime Date { get; }
}