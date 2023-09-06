using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.LuckyNumber;

public class LuckyNumberViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, LuckyNumber> GetLuckyNumber { get; }

    [ObservableAsProperty]
    public LuckyNumber LuckyNumber { get; }

    [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

    private readonly ILuckyNumberService _luckyNumberService;

    public LuckyNumberViewModel(
        INavigationService navigationService,
        AccountContext accountContext,
        AccountAwarePageTitleViewModel accountViewModel,
        ILuckyNumberService luckyNumberService) : base(navigationService)
    {
        _luckyNumberService = luckyNumberService;

        AccountViewModel = accountViewModel;

        GetLuckyNumber = ReactiveCommand.CreateFromTask((Unit _) => GetLuckyNumberAsync(accountContext.Account.Id));
        GetLuckyNumber.ToPropertyEx(this, vm => vm.LuckyNumber);

        accountContext.WhenAnyValue(ctx => ctx.Account)
            .WhereNotNull()
            .Select(_ => Unit.Default)
            .InvokeCommand(GetLuckyNumber);
    }

    private async Task<LuckyNumber> GetLuckyNumberAsync(int accountId)
    {
        return await _luckyNumberService.GetLuckyNumberAsync(
            accountId,
            DateTime.Now);
    }
}