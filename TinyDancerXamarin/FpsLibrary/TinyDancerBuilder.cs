using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Java.Interop;
using TinyDancerXamarin.FpsLibrary.UI;

namespace TinyDancerXamarin.FpsLibrary
{
    /**
    * Created by brianplummer on 8/29/15.
    */
    public class TinyDancerBuilder
    {
        private class ForegroundListener : Foreground.IListener
        {
            public void OnBecameBackground() => tinyCoach.Show();
            public void OnBecameForeground() => tinyCoach.Hide(false);
        }

        private static FpsConfig fpsConfig;
        private static FpsFrameCallback fpsFrameCallback;
        private static TinyCoach tinyCoach;
        private static Foreground.IListener foregroundListener = new ForegroundListener();

        internal TinyDancerBuilder()
        {
            fpsConfig = new FpsConfig();
        }

        /// <summary>
        /// configures the fpsConfig to the device's hardware
        /// refreshRate ex. 60fps and deviceRefreshRateInMs ex. 16.6ms
        /// </summary>
        /// <param name="context">Context.</param>
        private void SetFrameRate(Context context)
        {
            var windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var display = windowManager.DefaultDisplay;
            fpsConfig.DeviceRefreshRateInMs = 1000f / display.RefreshRate;
            fpsConfig.RefreshRate = display.RefreshRate;
        }

        /// <summary>
        /// stops the frame callback and foreground listener
        /// nulls out static variables
        /// called from FPSLibrary in a static context
        /// </summary>
        /// <param name="context">Context.</param>
        internal static void Hide(Context context)
        {
            // tell callback to stop registering itself
            fpsFrameCallback.Enabled = false;

            Foreground.SharedInstance(context).RemoveListener(foregroundListener);
            // remove the view from the window
            tinyCoach.Destroy();

            // null it all out
            tinyCoach = null;
            fpsFrameCallback = null;
            fpsConfig = null;
        }

        // PUBLIC BUILDER METHODS

        /// <summary>
        /// show fps meter, this regisers the frame callback that
        /// collects the fps info and pushes it to the ui
        /// </summary>
        /// <param name="context">Context.</param>
        public void Show(Context context)
        {
            if (OverlayPermRequest(context))
            {
                //once permission is granted then you must call show() again
                return;
            }

            //are we running?  if so, call tinyCoach.show() and return
            if (tinyCoach != null)
            {
                tinyCoach.Show();
                return;
            }

            // set device's frame rate info into the config
            SetFrameRate(context);

            // create the presenter that updates the view
            tinyCoach = new TinyCoach((Application)context.ApplicationContext, fpsConfig);

            // create our choreographer callback and register it
            fpsFrameCallback = new FpsFrameCallback(fpsConfig, tinyCoach);
            Choreographer.Instance.PostFrameCallback(fpsFrameCallback);

            //set activity background/foreground listener
            Foreground.Init((Application)context.ApplicationContext).AddListener(foregroundListener);
        }

        /// <summary>
        /// this adds a frame callback that the library will invoke on the
        /// each time the choreographer calls us, we will send you the frame times
        /// and number of dropped frames.
        /// </summary>
        public TinyDancerBuilder AddFrameDataCallback(FrameDataCallback callback)
        {
            fpsConfig.FrameDataCallback = callback;
            return this;
        }

        /// <summary>
        /// set red flag percent, default is 20%.
        /// </summary>
        /// <returns>The flag percentage.</returns>
        /// <param name="percentage">Percentage.</param>
        public TinyDancerBuilder RedFlagPercentage(float percentage)
        {
            fpsConfig.RedFlagPercentage = percentage;
            return this;
        }

        /// <summary>
        /// set red flag percent, default is 5%.
        /// </summary>
        /// <param name="percentage">Percentage.</param>
        public TinyDancerBuilder YellowFlagPercentage(float percentage)
        {
            fpsConfig.YellowFlagPercentage = percentage;
            return this;
        }

        /// <summary>
        /// starting x position of fps meter default is 200px.
        /// </summary>
        /// <param name="xPosition">X position.</param>
        public TinyDancerBuilder StartingXPosition(int xPosition)
        {
            fpsConfig.StartingXPosition = xPosition;
            fpsConfig.XOrYSpecified = true;
            return this;
        }

        /// <summary>
        /// starting y positon of fps meter default is 600px.
        /// </summary>
        /// <param name="yPosition">Y position.</param>
        public TinyDancerBuilder StartingYPosition(int yPosition)
        {
            fpsConfig.StartingYPosition = yPosition;
            fpsConfig.XOrYSpecified = true;
            return this;
        }

        /// <summary>
        /// starting gravity of fps meter default is Gravity.TOP | Gravity.START.
        /// </summary>
        /// <param name="gravity">Gravity.</param>
        public TinyDancerBuilder StartingGravity(GravityFlags gravity)
        {
            fpsConfig.StartingGravity = gravity;
            fpsConfig.GravitySpecified = true;
            return this;
        }

        /// <summary>
        /// request overlay permission when api >= 23.
        /// </summary>
        /// <param name="context">Context.</param>
        private bool OverlayPermRequest(Context context)
        {
            var permNeeded = false;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (!Android.Provider.Settings.CanDrawOverlays(context))
                {
                    var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission,
                                            Android.Net.Uri.Parse("package:" + context.PackageName));
                    intent.SetFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                    permNeeded = true;
                }
            }
            return permNeeded;
        }
    }
}
