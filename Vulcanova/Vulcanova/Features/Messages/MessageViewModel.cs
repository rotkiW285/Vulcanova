using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Features.Messages.Compose;
using Vulcanova.Resources;
using Vulcanova.Uonet.Api.MessageBox;
using Xamarin.Essentials;

namespace Vulcanova.Features.Messages;

public class MessageViewModel : ReactiveObject, IInitialize
{
    public ReactiveCommand<string, Unit> OpenAttachment { get; }
    
    public ReactiveCommand<Unit, Unit> ShowSeenByDialog { get; }
    
    public ReactiveCommand<Unit, INavigationResult> Reply { get; }
    
    [Reactive]
    public Message Message { get; private set; }

    [ObservableAsProperty]
    public bool ShowReadByControls { get; set; }

    [ObservableAsProperty]
    public int ReadBy { get; set; }
    
    [ObservableAsProperty]
    public bool CanReply { get; set; }

    public MessageViewModel(IPageDialogService dialogService, INavigationService navigationService)
    {
        OpenAttachment = ReactiveCommand.CreateFromTask(async (string url) =>
        {
            await Browser.OpenAsync(url);

            return Unit.Default;
        });

        ShowSeenByDialog = ReactiveCommand.CreateFromTask(async (Unit _) =>
        {
            var seenByOrderedAlphabetically = Message.Receiver
                .Where(x => x.HasRead == 1)
                .OrderBy(x => x.Name)
                .Select(x => x.Name)
                .ToArray();

            var message = seenByOrderedAlphabetically.Length == 0
                ? Strings.WhoSeenMessageDialogNobodyContent
                : string.Join(Environment.NewLine, seenByOrderedAlphabetically);

            await dialogService.DisplayAlertAsync(Strings.WhoSeenMessageDialogTitle, message, "OK");
        });

        Reply = ReactiveCommand.CreateFromTask(async (Unit _) => await navigationService.NavigateAsync(nameof(ComposeMessageView), useModalNavigation: true,
            parameters: new NavigationParameters
            {
                {nameof(ComposeMessageViewModel.MessageBoxId), Message.MessageBoxId},
                {"InReplyTo", Message}
            }), this.WhenAnyValue(vm => vm.Message)
            .WhereNotNull()
            .Select(m => m.Folder == MessageBoxFolder.Received));

        Reply.CanExecute.ToPropertyEx(this, vm => vm.CanReply);

        this.WhenAnyValue(vm => vm.Message)
            .WhereNotNull()
            .Select(m => m.Folder == MessageBoxFolder.Sent)
            .ToPropertyEx(this, vm => vm.ShowReadByControls);

        this.WhenAnyValue(vm => vm.Message)
            .WhereNotNull()
            .Select(m => m.Receiver.Count(c => c.HasRead == 1))
            .ToPropertyEx(this, vm => vm.ReadBy);
    }


    public void Initialize(INavigationParameters parameters)
    {
        Message = (Message) parameters[nameof(Message)];
    }
}