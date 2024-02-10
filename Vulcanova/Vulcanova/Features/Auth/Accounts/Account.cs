using System.Collections.Generic;
using LiteDB;
using Vulcanova.Uonet.Api.Auth;
using Period = Vulcanova.Features.Shared.Period;

namespace Vulcanova.Features.Auth.Accounts;

public class Account
{
    [BsonId]
    public int Id { get; set; }
    public int? PupilNumber { get; set; }
    public Pupil Pupil { get; set; }
    public Unit Unit { get; set; }
    public ConstituentUnit ConstituentUnit { get; set; }
    public bool IsActive { get; set; }
    public List<Period> Periods { get; set; }
    public string IdentityThumbprint { get; set; }
    public Login Login { get; set; }
    public string[] Capabilities { get; set; }
    public SenderEntry SenderEntry { get; set; }
    public string ClassDisplay { get; set; }
    public string Context { get; set; }
    public string Partition { get; set; }
}