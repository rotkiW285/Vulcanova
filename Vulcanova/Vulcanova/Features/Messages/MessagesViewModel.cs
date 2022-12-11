using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public class MessagesViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IEnumerable<MessageBox>> LoadBoxes { get; }
    
    public ReactiveCommand<bool, IEnumerable<Message>> LoadMessages { get; }

    [ObservableAsProperty]
    public IEnumerable<MessageBox> MessageBoxes { get; }
    
    [Reactive]
    public MessageBox CurrentBox { get; private set; }
    
    [ObservableAsProperty]
    public IEnumerable<Message> Messages { get; private set; }
    
    [Reactive]
    public int SelectedFolderIndex { get; set; }

    public MessagesViewModel(
        INavigationService navigationService,
        IMessageBoxesService messageBoxesService,
        IMessagesService messagesService,
        AccountContext accountContext) : base(navigationService)
    {
        LoadBoxes = ReactiveCommand.CreateFromObservable((bool forceSync) => 
            messageBoxesService.GetMessageBoxesByAccountId(accountContext.Account.Id, forceSync));

        LoadBoxes.ToPropertyEx(this, vm => vm.MessageBoxes);

        LoadMessages = ReactiveCommand.CreateFromObservable((bool forceSync) =>
            messagesService.GetMessagesByBox(accountContext.Account.Id, CurrentBox.GlobalKey, MessageBoxFolder.Received,
                forceSync));

        LoadMessages.ToPropertyEx(this, vm => vm.Messages);

        this.WhenAnyValue(vm => vm.MessageBoxes)
            .WhereNotNull()
            .Subscribe(boxes =>
            {
                var messageBoxes = boxes as MessageBox[] ?? boxes.ToArray();

                CurrentBox = messageBoxes.SingleOrDefault(x => x.IsSelected)
                             ?? messageBoxes.FirstOrDefault();
            });

        this.WhenAnyValue(vm => vm.CurrentBox)
            .WhereNotNull()
            .Select(_ => false) // don't force refresh
            .InvokeCommand(LoadMessages);

        accountContext
            .WhenAnyValue(ctx => ctx.Account)
            .Select(_ => false) // don't force refresh
            .InvokeCommand(LoadBoxes);
    }
}