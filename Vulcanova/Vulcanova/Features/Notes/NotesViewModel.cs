using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Notes.NoteDetails;
using Vulcanova.Features.Shared;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Vulcanova.Features.Notes;

public class NotesViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IReadOnlyCollection<Note>> GetNotesEntries { get; }
    public ReactiveCommand<int, Unit> ShowNoteDetails { get; }
    public ReactiveCommand<Unit, Unit> NextSemester { get; }
    public ReactiveCommand<Unit, Unit> PreviousSemester { get; }

    public ReadOnlyObservableCollection<NotesGroup> CurrentPeriodEntries => _currentPeriodEntries;
    private readonly ReadOnlyObservableCollection<NotesGroup> _currentPeriodEntries;

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
        var currentEntriesSource = new SourceList<NotesGroup>();

        var observableFilter = this.WhenAnyValue(vm => vm.SelectedPeriod)
            .Select<Period, Func<NotesGroup, bool>>(period => notesGroup => notesGroup.Date >= period.Start &&
                                                                            notesGroup.Date <= period.End);

        currentEntriesSource
            .Connect()
            .Filter(observableFilter)
            .Bind(out _currentPeriodEntries)
            .Subscribe();

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

        GetNotesEntries.Subscribe(notes =>
        {
            currentEntriesSource.Edit(items =>
            {
                items.Clear();

                items.AddRange(notes.GroupBy(x => x.DateModified)
                    .OrderBy(g => g.Key)
                    .Select(g => new NotesGroup(g.Key, g.ToList())));
            });
        });

        ShowNoteDetails = ReactiveCommand.Create((int noteId) =>
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