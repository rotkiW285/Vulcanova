using System.Reactive;
using Prism.Navigation;
using ReactiveUI;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Settings.Grades.iOS;

namespace Vulcanova.Features.Settings;

public class SettingsViewModel : ViewModelBase
{
    public decimal ValueOfPlus => _appSettings.Modifiers.PlusSettings.SelectedValue;
    public decimal ValueOfMinus => _appSettings.Modifiers.MinusSettings.SelectedValue;

    private readonly AppSettings _appSettings;

    public ReactiveCommand<Unit, INavigationResult> OpenValueOfPlusPicker { get; }
    public ReactiveCommand<Unit, INavigationResult> OpenValueOfMinusPicker { get; }

    public bool DisableReadReceipts
    {
        get => _appSettings.DisableReadReceipts;
        set => _appSettings.DisableReadReceipts = value;
    }
    
    public bool ForceAverageCalculationByApp
    {
        get => _appSettings.ForceAverageCalculationByApp;
        set => _appSettings.ForceAverageCalculationByApp = value;
    }

    public SettingsViewModel(INavigationService navigationService, AppSettings appSettings) : base(navigationService)
    {
        _appSettings = appSettings;

        OpenValueOfPlusPicker =
            ReactiveCommand.CreateFromTask(async () => await NavigationService.NavigateAsync(nameof(ValueOfPlusPickeriOS)));
            
        OpenValueOfMinusPicker =
            ReactiveCommand.CreateFromTask(async () => await NavigationService.NavigateAsync(nameof(ValueOfMinusPickeriOS)));
    }
}