using Xamarin.Forms;

namespace Vulcanova.Core.Layout;

public interface ISheetPopper
{
    void PushSheet(ContentView content, bool hasCloseButton = true, bool useSafeArea = false);
    void PopSheet(ContentView content);
}