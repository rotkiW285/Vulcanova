using System;
using System.Windows.Input;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout.Controls.ErrorPopup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPopup
    {
        public Exception Error { get; }

        public ICommand ShowDetailsCommand { get; }

        public ErrorPopup(Exception error, ICommand showDetailsCommand)
        {
            Error = error;
            ShowDetailsCommand = showDetailsCommand;

            InitializeComponent();
        }
    }
}