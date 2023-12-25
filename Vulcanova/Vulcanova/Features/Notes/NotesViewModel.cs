using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Data;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Notes.NoteDetails;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Notes;

public class NotesViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IReadOnlyCollection<Note>> GetNotesEntries { get; }
    public ReactiveCommand<AccountEntityId, Unit> ShowNoteDetails { get; }

    [ObservableAsProperty] private IReadOnlyCollection<Note> Notes { get; }

    // ObservableCollection can't be used here: https://github.com/xamarin/Xamarin.Forms/issues/13268
    [Reactive] public IReadOnlyCollection<NotesGroup> CurrentPeriodEntries { get; private set; }
    [Reactive] public IEnumerable<Period> Periods { get; private set; }
    [Reactive] public Period SelectedPeriod { get; private set; }
    [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; set; }
    [Reactive] public Note SelectedNote { get; set; }

    private readonly INotesService _notesService;

    public NotesViewModel(
        INavigationService navigationService,
        AccountContext accountContext,
        AccountAwarePageTitleViewModel accountViewModel,
        INotesService notesService) : base(navigationService)
    {
        _notesService = notesService;

        AccountViewModel = accountViewModel;

        var accountSetObservable = accountContext.WhenAnyValue(ctx => ctx.Account)
            .WhereNotNull();
        
        accountSetObservable.Select(a => a.Periods)
            .BindTo(this, vm => vm.Periods);

        accountSetObservable.Select(a => a.Periods.Single(p => p.Current))
            .BindTo(this, vm => vm.SelectedPeriod);

        GetNotesEntries = ReactiveCommand.CreateFromObservable((bool forceSync) =>
            GetEntries(accountContext.Account.Id, forceSync));

        GetNotesEntries.ToPropertyEx(this, vm => vm.Notes);

        this.WhenAnyValue(vm => vm.Notes,
                vm => vm.SelectedPeriod)
            .Select(tuple =>
            {
                var notes = tuple.Item1;
                var period = tuple.Item2;
        
                return notes?
                    .Where(n => n.DateModified >= period.Start && n.DateModified <= period.End)
                    .GroupBy(n => n.DateModified)
                    .OrderBy(g => g.Key)
                    .Select(g => new NotesGroup(g.Key, g.ToList()))
                    .ToList()
                    .AsReadOnly();
            })
            .BindTo(this, vm => vm.CurrentPeriodEntries);

        ShowNoteDetails = ReactiveCommand.Create((AccountEntityId noteId) =>
        {
            SelectedNote = CurrentPeriodEntries.SelectMany(e => e).First(n => n.Id == noteId);
            navigationService.NavigateAsync(nameof(NoteDetailsView), (nameof(NoteDetailsView.Note), SelectedNote));

            return Unit.Default;
        });

        accountSetObservable
            .Select(_ => false)
            .InvokeCommand(GetNotesEntries);
    }

    private IObservable<IReadOnlyCollection<Note>> GetEntries(int accountId, bool forceSync = false)
    {
        return _notesService.GetNotes(accountId, forceSync)
            .Select(e => e.ToArray());
    }
}