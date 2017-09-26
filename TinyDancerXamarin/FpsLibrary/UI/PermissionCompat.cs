using Android.OS;
using Android.Views;

namespace TinyDancerXamarin.FpsLibrary.UI
{
    public static class PermissionCompat
    {
        public static WindowManagerTypes PermissionFlag
        {
            get
            {
                //TODO: Uncomment when O will be available 
                //if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
                //{
                //    return WindowManagerTypes.ApplicationOverlay;
                //}

                return WindowManagerTypes.Phone;
            }
        }
    }
}
