using System;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout;

public class NameOfTypeExtension : IMarkupExtension<string>
{
    public Type Type { get; set; }

    public string ProvideValue(IServiceProvider serviceProvider)
    {
        return Type.Name;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}