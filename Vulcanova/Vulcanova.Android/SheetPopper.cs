using System;
using Android.Content;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Platform = Xamarin.Forms.Platform.Android.Platform;

namespace Vulcanova.Android
{
    public class SheetPopper : ISheetPopper
    {
        public event EventHandler<SheetEventArgs> SheetWillDisappear;
        public event EventHandler<SheetEventArgs> SheetDisappeared;

        public Page DisplayedSheet { get; private set; }

        internal static Context Context { get; set; }

        private DismissNotifyingBottomSheetDialog _dialog;

        public void PushSheet(Page page)
        {
            page.NavigationProxy.Inner = Application.Current.NavigationProxy;

            var renderer = Platform.GetRenderer(page);

            if (renderer == null)
            {
                renderer = Platform.CreateRendererWithContext(page, Context);
                Platform.SetRenderer(page, renderer);
            }

            _dialog = new DismissNotifyingBottomSheetDialog(Context);
            _dialog.SetContentView(renderer.View);

            _dialog.WillDismiss += (s, e) => { SheetWillDisappear?.Invoke(this, new SheetEventArgs(DisplayedSheet)); };

            _dialog.DidDismiss += (s, e) =>
            {
                SheetDisappeared?.Invoke(this, new SheetEventArgs(DisplayedSheet));

                DisplayedSheet = null;
            };

            _dialog.Show();

            DisplayedSheet = page;
        }

        public void PopSheet()
        {
            if (DisplayedSheet == null)
            {
                throw new NullReferenceException("No sheet is being displayed");
            }

            _dialog.Dismiss();
        }
    }
}