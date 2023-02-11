using System;

namespace Vulcanova.Features.Messages.Compose;

public class AddressBookEntry
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Group { get; set; }
    public Guid MessageBoxId { get; set; }
}