using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Vulcanova.Core.Layout.Converters;

public class PriorityBindingConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.FirstOrDefault(x => x is not null);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}