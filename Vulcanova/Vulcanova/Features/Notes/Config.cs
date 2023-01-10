using Prism.Ioc;
using Vulcanova.Features.Notes.NoteDetails;

namespace Vulcanova.Features.Notes;

public static class Config
{
    public static void RegisterNotes(this IContainerRegistry container)
    {
        container.RegisterForNavigation<NoteDetailsView>();

        container.RegisterScoped<INotesRepository, NotesRepository>();
        container.RegisterScoped<INotesService, NotesService>();
    }
}