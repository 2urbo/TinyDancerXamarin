using System.Collections.Generic;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace TinyDancerXamarin.FpsLibrary.UI
{
    public class TinyCoach
    {
        private class SimpleOnGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly TinyCoach _self;

#pragma warning disable RECS0154 // Parameter is never used
            internal SimpleOnGestureListener(TinyCoach self) => _self = self;
#pragma warning restore RECS0154 // Parameter is never used

            public override bool OnDoubleTap(MotionEvent e)
            {
                //hide but don't remove view
                _self.Hide(false);
                return base.OnDoubleTap(e);
            }
        }

        private class HideAnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
        {
            private readonly TinyCoach _self;
            private bool _needRemove;

            internal HideAnimatorListener(TinyCoach self, bool needRemove)
            {
                _self = self;
                _needRemove = needRemove;
            }

            public void OnAnimationStart(Animator animation) { }

            public void OnAnimationEnd(Animator animation)
            {
                _self.meterView.Visibility = ViewStates.Gone;
                if (_needRemove)
                {
                    _self.windowManager.RemoveView(_self.meterView);
                }
            }

            public void OnAnimationCancel(Animator animation) { }

            public void OnAnimationRepeat(Animator animation) { }
        }

        private FpsConfig fpsConfig;
        private readonly TextView meterView;
        private readonly IWindowManager windowManager;
        private int shortAnimationDuration = 200, longAnimationDuration = 700;

        // detect double tap so we can hide tinyDancer
        private GestureDetector.SimpleOnGestureListener simpleOnGestureListener;

        public TinyCoach(Application context, FpsConfig config)
        {
            fpsConfig = config;

            simpleOnGestureListener = new SimpleOnGestureListener(this);

            //create meter view
            meterView = LayoutInflater.From(context).Inflate(Resource.Layout.meter_view, null) as TextView;

            //set initial fps value....might change...
            meterView.Text = ((int)fpsConfig.RefreshRate).ToString();

            // grab window manager and add view to the window
            windowManager = meterView.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            AddViewToWindow(meterView);
        }

        private void AddViewToWindow(TextView view)
        {
            var permissionFlag = PermissionCompat.PermissionFlag;

            var paramsF = new WindowManagerLayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent,
                    permissionFlag,
                    WindowManagerFlags.NotFocusable,
                    Format.Translucent);

            // configure starting coordinates
            if (fpsConfig.XOrYSpecified)
            {
                paramsF.X = fpsConfig.StartingXPosition;
                paramsF.Y = fpsConfig.StartingYPosition;
                paramsF.Gravity = FpsConfig.DefaultGravity;
            }
            else if (fpsConfig.GravitySpecified)
            {
                paramsF.X = 0;
                paramsF.Y = 0;
                paramsF.Gravity = fpsConfig.StartingGravity;
            }
            else
            {
                paramsF.Gravity = FpsConfig.DefaultGravity;
                paramsF.X = fpsConfig.StartingXPosition;
                paramsF.Y = fpsConfig.StartingYPosition;
            }

            // add view to the window
            windowManager.AddView(view, paramsF);

            // create gesture detector to listen for double taps
            var gestureDetector = new GestureDetector(view.Context, simpleOnGestureListener);

            // attach touch listener
            view.SetOnTouchListener(new DancerTouchListener(paramsF, windowManager, gestureDetector));

            // disable haptic feedback
            view.HapticFeedbackEnabled = false;

            // show the meter
            Show();
        }

        public void ShowData(FpsConfig fpsConfig, List<long> dataSet)
        {
            var droppedSet = Calculation.GetDroppedSet(fpsConfig, dataSet);
            var answer = Calculation.CalculateMetric(fpsConfig, dataSet, droppedSet);

            switch (answer.Key)
            {
                case Calculation.Metric.Bad:
                    meterView.SetBackgroundResource(Resource.Drawable.fpsmeterring_bad);
                    break;
                case Calculation.Metric.Medium:
                    meterView.SetBackgroundResource(Resource.Drawable.fpsmeterring_medium);
                    break;
                default:
                    meterView.SetBackgroundResource(Resource.Drawable.fpsmeterring_good);
                    break;
            } 

            meterView.Text = answer.Value.ToString();
        }

        public void Destroy()
        {
            meterView.SetOnTouchListener(null);
            Hide(true);
        }

        public void Show()
        {
            meterView.Alpha = 0f;
            meterView.Visibility = ViewStates.Visible;
            meterView.Animate()
                     .Alpha(1f)
                     .SetDuration(longAnimationDuration)
                     .SetListener(null);
        }

        public void Hide(bool needRemove)
        {
            meterView.Animate()
                     .Alpha(0f)
                     .SetDuration(shortAnimationDuration)
                     .SetListener(new HideAnimatorListener(this, needRemove));
        }
    }
}
