using Vulcanova.Uonet.Api.Schedule;

namespace Vulcanova.Features.Timetable;

public static class ChangeDisplayStyleExtensions
{
    public static ChangeDisplayTextDecorations GetDisplayTextDecorations(this TimetableListEntry.ChangeDetails changeDetails)
    {
        if (changeDetails is not null)
        {
            return changeDetails switch
            {
                {ChangeType: ChangeType.Exemption or ChangeType.ClassAbsence} => ChangeDisplayTextDecorations.Strikethrough,
                {
                    ChangeType: ChangeType.Rescheduled, RescheduleKind: TimetableListEntry.RescheduleKind.Removed
                } => ChangeDisplayTextDecorations.Strikethrough,
                _ => ChangeDisplayTextDecorations.None
            };
        }

        return ChangeDisplayTextDecorations.None;
    }

    public static ChangeDisplayColor GetDisplayColor(this TimetableListEntry.ChangeDetails changeDetails)
    {
        if (changeDetails is not null)
        {
            return changeDetails switch
            {
                {ChangeType: ChangeType.Exemption or ChangeType.ClassAbsence} => ChangeDisplayColor.Red,
                {ChangeType: ChangeType.Substitution} => ChangeDisplayColor.Yellow,
                {
                    ChangeType: ChangeType.Rescheduled, RescheduleKind: TimetableListEntry.RescheduleKind.Added
                } => ChangeDisplayColor.Yellow,
                {
                    ChangeType: ChangeType.Rescheduled, RescheduleKind: TimetableListEntry.RescheduleKind.Removed
                } => ChangeDisplayColor.Red,
                _ => ChangeDisplayColor.Yellow
            };
        }

        return ChangeDisplayColor.Normal;
    }
}

public enum ChangeDisplayColor
{
    Normal,
    Yellow,
    Red,
}

public enum ChangeDisplayTextDecorations
{
    None,
    Strikethrough
}