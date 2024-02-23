using System;
using System.Collections.Generic;
using System.Linq;
using Vulcanova.Features.Shared;

namespace Vulcanova.Core.Uonet;

public static class PeriodsExtensions
{
    public static Period CurrentOrLast(this IList<Period> periods)
    {
        var current = periods.SingleOrDefault(x => x.Current) ?? periods
            .SingleOrDefault(p => p.Start <= DateTime.Now && p.End >= DateTime.Now);
        
        return current ?? periods.OrderByDescending(x => x.Start).First();
    }
}