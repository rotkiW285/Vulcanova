using System.Reactive;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Xamarin.Essentials;

namespace Vulcanova.Features.Homework.HomeworkDetails;

public sealed class HomeworkDetailsViewModel : ViewModelBase, IInitialize
{
    [Reactive] public Homework Homework { get; private set; }

    public ReactiveCommand<string, Unit> OpenAttachment { get; }

    public HomeworkDetailsViewModel(INavigationService navigationService) : base(navigationService)
    {
        OpenAttachment = ReactiveCommand.CreateFromTask(async (string url) =>
        {
            await Browser.OpenAsync(url);

            return Unit.Default;
        });
    }

    public void Initialize(INavigationParameters parameters)
    {
        Homework = parameters.GetValue<Homework>(nameof(Homework));
    }
}