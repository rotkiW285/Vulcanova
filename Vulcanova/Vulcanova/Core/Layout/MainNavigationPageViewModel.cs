using System;
using System.Reactive;
using Prism.Services;
using ReactiveUI;
using Vulcanova.Resources;
using Xamarin.Essentials;

namespace Vulcanova.Core.Layout
{
    public class MainNavigationPageViewModel : ReactiveObject
    {
        public ReactiveCommand<Exception, Unit> ShowErrorDetails { get; }


        public MainNavigationPageViewModel(IPageDialogService dialogService)
        {
            ShowErrorDetails = ReactiveCommand.CreateFromTask(async (Exception ex) =>
            {
                var result = await dialogService.DisplayAlertAsync(Strings.ErrorAlertTitle, ex.ToString(),
                    Strings.ReportErrorButton, Strings.IgnoreErrorButton);

                if (result)
                {
                    await Browser.OpenAsync("https://github.com/VulcanovaApp/Vulcanova/issues",
                        BrowserLaunchMode.SystemPreferred);
                }

                return Unit.Default;
            });
        }
    }
}