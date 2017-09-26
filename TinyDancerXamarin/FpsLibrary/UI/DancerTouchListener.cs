using Android.Views;

namespace TinyDancerXamarin.FpsLibrary.UI
{
    /**
    * Created by brianplummer on 9/12/15.
    */
    public class DancerTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private int initialX;
        private int initialY;
        private float initialTouchX;
        private float initialTouchY;

        private readonly WindowManagerLayoutParams paramsF;
        private readonly IWindowManager windowManager;
        private readonly GestureDetector gestureDetector;

        public DancerTouchListener(WindowManagerLayoutParams paramsF,
                                   IWindowManager windowManager,
                                   GestureDetector gestureDetector)
        {
            this.windowManager = windowManager;
            this.paramsF = paramsF;
            this.gestureDetector = gestureDetector;
        }

        public bool OnTouch(View v, MotionEvent ev)
        {
            gestureDetector.OnTouchEvent(ev);
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    initialX = paramsF.X;
                    initialY = paramsF.Y;
                    initialTouchX = ev.RawX;
                    initialTouchY = ev.RawY;
                    break;
                case MotionEventActions.Up:
                    break;
                case MotionEventActions.Move:
                    paramsF.X = initialX + (int)(ev.RawX - initialTouchX);
                    paramsF.Y = initialY + (int)(ev.RawY - initialTouchY);
                    windowManager.UpdateViewLayout(v, paramsF);
                    break;
            }
            return false;
        }
    }
}
