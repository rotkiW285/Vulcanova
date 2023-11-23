using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
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

    [Reactive] public bool IsPickingRecipient { get; set; }
    [Reactive] public string RecipientFilter { get; set; } = string.Empty;

    [Reactive] public Guid? MessageBoxId { get; private set; }

    public ReactiveCommand<AddressBookEntry, Unit> SelectRecipient { get; }

    [Reactive] public AddressBookEntry Recipient { get; set; }

    [Reactive] public string Subject { get; set; } = string.Empty;

    [Reactive] public string Content { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> Send { get; }

    private Guid? _threadKey;

    public ComposeMessageViewModel(
        IAddressBookProvider addressBookProvider,
        IMessageSender messageSender,
        AccountContext accountContext,
        INavigationService navigationService) : base(navigationService)
    {
        var currentEntriesSource = new SourceList<AddressBookEntry>();

        var observableFilter = this.WhenAnyValue(vm => vm.RecipientFilter)
            .Select<string, Func<AddressBookEntry, bool>>(value => entry =>
                CultureInfo.InvariantCulture.CompareInfo.IndexOf(entry.Name, value, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != -1);

        currentEntriesSource
            .Connect()
            .Filter(observableFilter)
            .ObserveOn(RxApp.MainThreadScheduler)
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

        this.WhenAnyValue(vm => vm.RecipientFilter)
            .Subscribe(_ =>
            {
                if (IsPickingRecipient)
                {
                    Recipient = null;
                }
            });

        SelectRecipient = ReactiveCommand.Create<AddressBookEntry, Unit>(entry =>
        {
            IsPickingRecipient = false;
            Recipient = entry;
            RecipientFilter = entry.Name;

            return default;
        });

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
                await messageSender.SendMessageAsync(accountContext.Account.Id, Recipient, Subject, Content, 
                    _threadKey);

                await navigationService.GoBackAsync();
            },
            canExecute: canSend);
    }

    public void Initialize(INavigationParameters parameters)
    {
        MessageBoxId = (Guid) parameters[nameof(MessageBoxId)];

        if (parameters.TryGetValue<Message>("InReplyTo", out var message))
        {
            var cleanContent = message.Content.Replace("<br>", "\n");

            cleanContent = Regex.Replace(cleanContent, @"<p>(.*?)<\/p>", 
                m => m.Groups[1].Value + Environment.NewLine, RegexOptions.Singleline);

            Content = $"\n\nOd: {message.Sender.Name}\nData: {message.DateSent:G}\n\n{cleanContent}";
            Subject = $"RE: {message.Subject}";

            // TODO: Perhaps try to look-up the entry in the DB?
            Recipient = new AddressBookEntry
            {
                Id = message.Sender.GlobalKey,
                MessageBoxId = MessageBoxId.Value,
                Name = message.Sender.Name
            };

            RecipientFilter = message.Sender.Name;

            _threadKey = message.ThreadKey;
        }
    }
}