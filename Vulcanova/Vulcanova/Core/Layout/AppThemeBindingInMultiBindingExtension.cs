using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout;

// https://github.com/xamarin/Xamarin.Forms/issues/15037
public static class XamlParseExceptionLineInfoHelper
{
    public static XamlParseException CreateException(string message, IServiceProvider serviceProvider,
        Exception innerException = null)
    {
        return new XamlParseException(message, GetLineInfo(serviceProvider), innerException);
    }

    private static IXmlLineInfo GetLineInfo(IServiceProvider serviceProvider)
    {
        IXmlLineInfoProvider xmlLineInfoProvider =
            serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider;
        if (xmlLineInfoProvider == null)
        {
            return new XmlLineInfo();
        }

        return xmlLineInfoProvider.XmlLineInfo;
    }
}

public class AppThemeBindingReflection
{
    private static readonly Type AppThemeBindingType;
    private static readonly PropertyInfo LightPropertyInfo;
    private static readonly PropertyInfo DarkPropertyInfo;
    private static readonly PropertyInfo DefaultPropertyInfo;

    static AppThemeBindingReflection()
    {
        var types = typeof(OnPlatform<>).Assembly.GetTypes();
        AppThemeBindingType = types.First(t => t.Name == "AppThemeBinding");
        LightPropertyInfo = AppThemeBindingType.GetProperty("Light");
        DarkPropertyInfo = AppThemeBindingType.GetProperty("Dark");
        DefaultPropertyInfo = AppThemeBindingType.GetProperty("Default");
    }

    public BindingBase AppThemeBinding { get; }

    public AppThemeBindingReflection()
    {
        AppThemeBinding = (BindingBase) Activator.CreateInstance(AppThemeBindingType);
    }

    public void SetLight(object value)
    {
        LightPropertyInfo.SetValue(AppThemeBinding, value);
    }

    public void SetDark(object value)
    {
        DarkPropertyInfo.SetValue(AppThemeBinding, value);
    }

    public void SetDefault(object value)
    {
        DefaultPropertyInfo.SetValue(AppThemeBinding, value);
    }
}

[ContentProperty("Default")]
public class AppThemeBindingInMultiBindingExtension : IMarkupExtension<BindingBase>
{
    #region reflection

    private static readonly Type ValueConverterProviderType;
    private static readonly MethodInfo ConvertWithValueConverterMethodInfo;
    private static readonly MethodInfo ConvertToMethodInfo;

    #endregion

    static AppThemeBindingInMultiBindingExtension()
    {
        var types = typeof(OnPlatform<>).Assembly.GetTypes();
        var typeConverterExtensionsType = types.First(t => t.Name == "TypeConversionExtensions");
        ConvertToMethodInfo = typeConverterExtensionsType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m => m.GetParameters()[2].ParameterType == typeof(Func<MemberInfo>));
        ValueConverterProviderType = types.First(t => t.Name == "IValueConverterProvider");
        ConvertWithValueConverterMethodInfo = ValueConverterProviderType.GetMethod("Convert");
    }

    #region light, dark, default

    private object _default;

    private bool _hasdefault;

    private object _light;

    private bool _haslight;

    private object _dark;

    private bool _hasdark;

    public object Default
    {
        get => _default;
        set
        {
            _default = value;
            _hasdefault = true;
        }
    }

    public object Light
    {
        get => _light;
        set
        {
            _light = value;
            _haslight = true;
        }
    }

    public object Dark
    {
        get => _dark;
        set
        {
            _dark = value;
            _hasdark = true;
        }
    }

    #endregion

    public BindableProperty BindableProperty { get; set; }

    private BindingBase GetConfiguredAppThemeBinding(Func<object, object> converter)
    {
        var appThemeBindingReflection = new AppThemeBindingReflection();
        if (_haslight)
        {
            appThemeBindingReflection.SetLight(converter(Light));
        }

        if (_hasdark)
        {
            appThemeBindingReflection.SetDark(converter(Dark));
        }

        if (_hasdefault)
        {
            appThemeBindingReflection.SetDefault(converter(Default));
        }

        return appThemeBindingReflection.AppThemeBinding;
    }

    private BindingBase WithoutConversion()
    {
        return GetConfiguredAppThemeBinding(themeValue => themeValue);
    }

    private BindingBase WithConversion(IServiceProvider serviceProvider)
    {
        var toType = BindableProperty.ReturnType;
        object valueConverterProvider = serviceProvider.GetService(ValueConverterProviderType);
        Func<object, object> convertWithValueConverterProvider = themeValue =>
            ConvertWithValueConverterMethodInfo.Invoke(valueConverterProvider,
                new object[] {themeValue, toType, (Func<MemberInfo>) Minforetriever, serviceProvider});
        Func<object, object> converter = valueConverterProvider == null
            ? convertWithValueConverterProvider
            : themeValue =>
            {
                var args = new object[] {themeValue, toType, (Func<MemberInfo>) Minforetriever, serviceProvider, null};
                var converted = ConvertToMethodInfo.Invoke(null, args);
                var exception = (Exception) args[4];
                if (exception != null)
                {
                    throw exception;
                }

                return converted;
            };

        return GetConfiguredAppThemeBinding(converter);

        MemberInfo Minforetriever()
        {
            MemberInfo memberInfo = null;
            try
            {
                memberInfo = BindableProperty.DeclaringType.GetRuntimeProperty(BindableProperty.PropertyName);
            }
            catch (AmbiguousMatchException innerException)
            {
                throw XamlParseExceptionLineInfoHelper.CreateException(
                    $"Multiple properties with name '{BindableProperty.DeclaringType}.{BindableProperty.PropertyName}' found.",
                    serviceProvider, innerException);
            }

            if (memberInfo != null)
            {
                return memberInfo;
            }

            try
            {
                return BindableProperty.DeclaringType.GetRuntimeMethod("Get" + BindableProperty.PropertyName,
                    new Type[1]
                    {
                        typeof(BindableObject)
                    });
            }
            catch (AmbiguousMatchException innerException2)
            {
                throw XamlParseExceptionLineInfoHelper.CreateException(
                    $"Multiple methods with name '{BindableProperty.DeclaringType}.Get{BindableProperty.PropertyName}' found.",
                    serviceProvider, innerException2);
            }
        }
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return ((IMarkupExtension<BindingBase>) this).ProvideValue(serviceProvider);
    }

    BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
    {
        if (Default == null && Light == null && Dark == null)
        {
            throw XamlParseExceptionLineInfoHelper.CreateException(
                "AppThemeBindingExtension requires a non-null value to be specified for at least one theme or Default.",
                serviceProvider);
        }

        if (BindableProperty == null)
        {
            return WithoutConversion();
        }

        return WithConversion(serviceProvider);
    }
}