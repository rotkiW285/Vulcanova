using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api;
using Vulcanova.Uonet.Api.Grades;

namespace Vulcanova.Features.Grades;

public class AverageGradesService : UonetResourceProvider, IAverageGradesService
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IAccountRepository _accountRepository;
    private readonly IAverageGradesRepository _averageGradesRepository;
    private readonly IMapper _mapper;

    public AverageGradesService(
        IApiClientFactory apiClientFactory,
        IAccountRepository accountRepository,
        IAverageGradesRepository averageGradesRepository,
        IMapper mapper)
    {
        _apiClientFactory = apiClientFactory;
        _accountRepository = accountRepository;
        _averageGradesRepository = averageGradesRepository;
        _mapper = mapper;
    }

    public IObservable<IEnumerable<AverageGrade>> GetAverageGrades(int accountId, int periodId, bool forceSync = false)
    {
        return Observable.Create<IEnumerable<AverageGrade>>(async observer =>
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            var normalGradesResourceKey = GetAverageGradesResourceKey(account, periodId);

            var items = await _averageGradesRepository.GetAverageGradesForPupilAsync(account.Id, account.Pupil.Id,
                periodId);

            observer.OnNext(items);

            if (ShouldSync(normalGradesResourceKey) || forceSync)
            {
                var onlineGrades = await FetchAverageGradesAsync(account, periodId);

                await _averageGradesRepository.UpdatePupilAverageGradesAsync(onlineGrades);

                SetJustSynced(normalGradesResourceKey);

                items = await _averageGradesRepository.GetAverageGradesForPupilAsync(account.Id, account.Pupil.Id,
                    periodId);

                observer.OnNext(items);
            }

            observer.OnCompleted();
        });
    }

    private async Task<AverageGrade[]> FetchAverageGradesAsync(Account account, int periodId)
    {
        var client = await _apiClientFactory.GetAuthenticatedAsync(account);

        var averageGradesQuery =
            new GetAverageGradesByPupilQuery(account.Unit.Id, account.Pupil.Id, periodId, 500);

        var averageGrades = client.GetAllAsync(GetAverageGradesByPupilQuery.ApiEndpoint,
            averageGradesQuery);

        var domainGrades = await averageGrades
            .Select(_mapper.Map<AverageGrade>)
            .ToArrayAsync();

        foreach (var grade in domainGrades)
        {
            grade.Id.AccountId = account.Id;
        }

        return domainGrades;
    }

    private static string GetAverageGradesResourceKey(Account account, int periodId)
        => $"AverageGrades_{account.Id}_{account.Pupil.Id}_{periodId}";

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromMinutes(15);
}