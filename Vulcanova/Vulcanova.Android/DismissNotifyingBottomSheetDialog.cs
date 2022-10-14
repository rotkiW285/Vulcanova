using System;
using Android.Content;
using Android.Runtime;
using Google.Android.Material.BottomSheet;

namespace Vulcanova.Android
{
    public class DismissNotifyingBottomSheetDialog : BottomSheetDialog
    {
        public event EventHandler WillDismiss;
        public event EventHandler DidDismiss;

        protected DismissNotifyingBottomSheetDialog(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            
        }

        public DismissNotifyingBottomSheetDialog(Context context) : base(context)
        {
        }

        protected DismissNotifyingBottomSheetDialog(Context context, bool cancelable, IDialogInterfaceOnCancelListener cancelListener) : base(context, cancelable, cancelListener)
        {
        }

        public DismissNotifyingBottomSheetDialog(Context context, int theme) : base(context, theme)
        {
        }

        public override void Dismiss()
        {
            WillDismiss?.Invoke(this, EventArgs.Empty);

            base.Dismiss();

            DidDismiss?.Invoke(this, EventArgs.Empty);
        }
    }
}