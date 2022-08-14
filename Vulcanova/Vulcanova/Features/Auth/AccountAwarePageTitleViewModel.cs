using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Auth;

public class AccountAwarePageTitleViewModel
{
    [Reactive]
    public string Initials { get; private set; }

    public AccountAwarePageTitleViewModel(
        AccountContext accountContext,
        IAccountRepository accountRepository)
    {
        accountContext.WhenAnyValue(ctx => ctx.AccountId)
            .WhereNotNull()
            .Select(id =>
            {
                return Observable.FromAsync(async () =>
                {
                    var account = await accountRepository.GetByIdAsync(id);

                    Initials = $"{account.Pupil.FirstName[0]}{account.Pupil.Surname[0]}";
                });
            })
            .Concat()
            .Subscribe();
    }
}