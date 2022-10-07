using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Vulcanova.Core.Layout;

public interface ISheetPopper
{
    Dictionary<Page, Action> Sheets { get; }
    
    void PushSheet(Page page);
    void PopSheet(Page page);
}