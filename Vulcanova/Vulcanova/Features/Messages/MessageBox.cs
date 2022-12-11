using System;

namespace Vulcanova.Features.Messages;

public class MessageBox
{
    public int Id { get; set; }
    public Guid GlobalKey { get; set; }
    public string Name { get; set; }
    public int AccountId { get; set; }
    public bool IsSelected { get; set; }
}