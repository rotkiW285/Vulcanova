using System;
using Xamarin.Forms;

namespace Vulcanova.Core.Layout.Controls;

public class KeyboardNotifyingEditor : Editor
{
    public event EventHandler KeyboardWillShow;
    public event EventHandler KeyboardDidShow;

    public virtual void OnKeyboardWillShow()
    {
        KeyboardWillShow?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnKeyboardDidShow()
    {
        KeyboardDidShow?.Invoke(this, EventArgs.Empty);
    }
}