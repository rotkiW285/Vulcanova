using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Messages.Compose;

public class ComposeMessageViewModel : ViewModelBase, IInitialize
{
    [ObservableAsProperty] public IEnumerable<AddressBookEntry> AddressBookEntries { get; }

    public ReadOnlyObservableCollection<AddressBookEntry> FilteredAddressBookEntries => _filteredAddressBookEntries;
    private readonly ReadOnlyObservableCollection<AddressBookEntry> _filteredAddressBookEntries;

    private ReactiveCommand<Unit, IEnumerable<AddressBookEntry>> GetAddressBookEntries { get; }

    [Reactive] public string RecipientFilter { get; set; } = string.Empty;

    [Reactive] public Guid? MessageBoxId { get; private set; }

    [Reactive] public AddressBookEntry Recipient { get; set; }

    [Reactive] public string Subject { get; set; } = string.Empty;

    [Reactive] public string Content { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> Send { get; }

    public ComposeMessageViewModel(
        IAddressBookProvider addressBookProvider,
        IMessageSender messageSender,
        AccountContext accountContext,
        INavigationService navigationService) : base(navigationService)
    {
        var currentEntriesSource = new SourceList<AddressBookEntry>();

        var observableFilter = this.WhenAnyValue(vm => vm.RecipientFilter)
            .Select<string, Func<AddressBookEntry, bool>>(value => entry => entry.Name.Contains(value));

        currentEntriesSource
            .Connect()
            .Filter(observableFilter)
            .Bind(out _filteredAddressBookEntries)
            .Subscribe();

        GetAddressBookEntries = ReactiveCommand.CreateFromObservable((Unit _) =>
            addressBookProvider.GetAddressBookEntriesForBox(accountContext.Account.Id, MessageBoxId!.Value));

        GetAddressBookEntries.ToPropertyEx(this, vm => vm.AddressBookEntries);

        this.WhenAnyValue(vm => vm.AddressBookEntries)
            .WhereNotNull()
            .Subscribe(x =>
            {
                currentEntriesSource.Edit(update =>
                {
                    update.Clear();
                    update.AddRange(x);
                });
            });

        this.WhenAnyValue(vm => vm.MessageBoxId)
            .WhereNotNull()
            .Select(_ => Unit.Default)
            .InvokeCommand(GetAddressBookEntries);

        this.WhenAnyValue(vm => vm.Recipient)
            .WhereNotNull()
            .Subscribe(v => RecipientFilter = v.Name);

        var canSend = this.WhenAnyValue(
                vm => vm.Recipient,
                vm => vm.Content,
                vm => vm.Subject)
            .Select(values => values.Item1 != null
                              && !string.IsNullOrEmpty(values.Item2)
                              && !string.IsNullOrEmpty(values.Item3));

        Send = ReactiveCommand.CreateFromTask(async _
                =>
            {
                await messageSender.SendMessageAsync(accountContext.Account.Id, Recipient, Subject, Content);
                await navigationService.GoBackAsync();
            },
            canExecute: canSend);
    }

    public void Initialize(INavigationParameters parameters)
    {
        MessageBoxId = (Guid) parameters[nameof(MessageBoxId)];
    }
}