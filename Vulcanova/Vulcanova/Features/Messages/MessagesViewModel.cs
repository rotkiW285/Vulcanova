using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Messages.Compose;
using Vulcanova.Features.Settings;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public class MessagesViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IEnumerable<MessageBox>> LoadBoxes { get; }
    
    public ReactiveCommand<bool, IEnumerable<Message>> LoadMessages { get; }
    
    public ReactiveCommand<Unit, INavigationResult> ShowMessageComposer { get; }

    public ReactiveCommand<Guid, Unit> ShowMessage { get; }

    [ObservableAsProperty]
    public IEnumerable<MessageBox> MessageBoxes { get; }
    
    [Reactive]
    public MessageBox CurrentBox { get; private set; }
    
    [ObservableAsProperty]
    public IEnumerable<Message> Messages { get; private set; }

    [Reactive]
    public MessageBoxFolder SelectedFolder { get; set; } = MessageBoxFolder.Received;

    public MessagesViewModel(
        INavigationService navigationService,
        IMessageBoxesService messageBoxesService,
        IMessagesService messagesService,
        AccountContext accountContext,
        AppSettings appSettings) : base(navigationService)
    {
        LoadBoxes = ReactiveCommand.CreateFromObservable((bool forceSync) => 
            messageBoxesService.GetMessageBoxesByAccountId(accountContext.Account.Id, forceSync));

        LoadBoxes.ToPropertyEx(this, vm => vm.MessageBoxes);

        LoadMessages = ReactiveCommand.CreateFromObservable((bool forceSync) =>
            messagesService.GetMessagesByBox(accountContext.Account.Id, CurrentBox.GlobalKey, SelectedFolder,
                forceSync));

        LoadMessages.ToPropertyEx(this, vm => vm.Messages);

        ShowMessage = ReactiveCommand.CreateFromTask(async (Guid messageId) =>
        {
            var message = Messages.Single(x => x.Id == messageId);

            await navigationService.NavigateAsync(nameof(MessageView), (nameof(MessageView.Message), message));

            if (!appSettings.DisableReadReceipts)
            {
                await messagesService.MarkMessageAsReadAsync(accountContext.Account.Id, message.MessageBoxId,
                    messageId);
            }
        });

        ShowMessageComposer = ReactiveCommand.CreateFromTask(async _ =>
            await navigationService.NavigateAsync(nameof(ComposeMessageView), useModalNavigation: true, parameters: new NavigationParameters
            {
                {nameof(ComposeMessageViewModel.MessageBoxId), CurrentBox.GlobalKey}
            }));

        this.WhenAnyValue(vm => vm.MessageBoxes)
            .WhereNotNull()
            .Subscribe(boxes =>
            {
                var messageBoxes = boxes as MessageBox[] ?? boxes.ToArray();

                CurrentBox = messageBoxes.SingleOrDefault(x => x.IsSelected)
                             ?? messageBoxes.FirstOrDefault();
            });

        var whenAnyFolder = this.WhenAnyValue(vm => vm.SelectedFolder);

        this.WhenAnyValue(vm => vm.CurrentBox)
            .WhereNotNull()
            .CombineLatest(whenAnyFolder)
            .Select(_ => false) // don't force refresh
            .InvokeCommand(LoadMessages);

        accountContext
            .WhenAnyValue(ctx => ctx.Account)
            .Select(_ => false) // don't force refresh
            .InvokeCommand(LoadBoxes);
    }
}