using Xamarin.Forms;

namespace Vulcanova.Features.Grades.Final;

public sealed class FinalGradesTemplateSelector : DataTemplateSelector
{
    public DataTemplate Average { get; set; }
    public DataTemplate Entry { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item is decimal ? Average : Entry;
    }
}