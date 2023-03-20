using System;
using System.Globalization;
using Vulcanova.Core.Layout;
using Vulcanova.Uonet.Api.Schedule;
using Xamarin.Forms;

namespace Vulcanova.Features.Timetable;

public class TimetableChangeColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values[0];

        if (value is TimetableListEntry.ChangeDetails change)
        {
            var baseColor = change switch
            {
                {ChangeType: ChangeType.Exemption or ChangeType.ClassAbsence} => "ErrorColor",
                {ChangeType: ChangeType.Substitution} => "WarningColor",
                {
                    ChangeType: ChangeType.Rescheduled, RescheduleKind: TimetableListEntry.RescheduleKind.Added
                } => "WarningColor",
                {
                    ChangeType: ChangeType.Rescheduled, RescheduleKind: TimetableListEntry.RescheduleKind.Removed
                } => "ErrorColor",
                _ => "WarningColor"
            };

            return ThemeUtility.GetThemedColorByResourceKey(baseColor);
        }

        return ThemeUtility.GetDefaultTextColor();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}