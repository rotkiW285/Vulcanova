using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcanova.Features.Notes;

public interface INotesRepository
{
    Task<IEnumerable<Note>> GetNotesByPupilAsync(int accountId, int pupilId);
    Task UpdateNoteEntriesAsync(IEnumerable<Note> entries, int accountId, int pupilId);
}