using System;
using System.Globalization;
using Vulcanova.Uonet.Api.Schedule;
using Xamarin.Forms;

namespace Vulcanova.Features.Timetable;

public class TimetableChangeTextDecorationsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimetableListEntry.ChangeDetails change)
        {
            return change switch
            {
                {ChangeType: ChangeType.Exemption or ChangeType.ClassAbsence} => TextDecorations.Strikethrough,
                {
                    ChangeType: ChangeType.Rescheduled, RescheduleKind: TimetableListEntry.RescheduleKind.Removed
                } => TextDecorations.Strikethrough,
                _ => TextDecorations.None
            };
        }

        return TextDecorations.None;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}