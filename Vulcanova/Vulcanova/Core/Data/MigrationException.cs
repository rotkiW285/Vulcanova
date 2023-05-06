using System;

namespace Vulcanova.Core.Data;

public class MigrationException : Exception
{
    public MigrationException(string message) : base(message)
    {
    }
}