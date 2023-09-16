using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vulcanova.Features.Timetable;

public class TimetableChangeTextDecorationsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var decorations = (value as TimetableListEntry.ChangeDetails).GetDisplayTextDecorations();

        return decorations switch
        {
            ChangeDisplayTextDecorations.None => TextDecorations.None,
            ChangeDisplayTextDecorations.Strikethrough => TextDecorations.Strikethrough,
            _ => TextDecorations.None
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}