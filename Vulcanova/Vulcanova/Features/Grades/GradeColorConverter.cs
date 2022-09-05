using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vulcanova.Features.Grades;

public class GradeColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var numericValue = (uint) value;

        if (numericValue == 0)
        {
            return parameter;
        }

        var bytes = BitConverter.GetBytes(numericValue);
        return Color.FromRgb(bytes[2], bytes[1], bytes[0]);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}