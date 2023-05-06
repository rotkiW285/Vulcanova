using System;
using System.Linq;
using System.Reflection;
using DryIoc;

namespace Vulcanova.Core.Data;

public static class AssemblyMigrationsProvider
{
    public static LiteDbMigration[] GetAllMigrations()
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(LiteDbMigration)) && !t.IsAbstract)
            .Select(t => (LiteDbMigration) Activator.CreateInstance(t))
            .ToArray();
    }
}