using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Vulcanova.Core.Layout;

public interface ISheetPopper
{
    event EventHandler<SheetEventArgs> SheetWillDisappear;
    event EventHandler<SheetEventArgs> SheetDisappeared;

    Page DisplayedSheet { get; }

    void PushSheet(Page page);
    void PopSheet();
}

public class SheetEventArgs
{
    public Page Sheet { get; }

    public SheetEventArgs(Page sheet) => Sheet = sheet;
}