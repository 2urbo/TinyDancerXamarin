using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace TinyDancerXamarin.FpsLibrary
{
    // COPIED FROM: https://gist.github.com/steveliles/11116937

    /**
     * Usage:
     *
     * 1. Get the Foreground Singleton, passing a Context or Application object unless you
     * are sure that the Singleton has definitely already been initialised elsewhere.
     *
     * 2.a) Perform a direct, synchronous check: Foreground.isForeground() / .isBackground()
     *
     * or
     *
     * 2.b) Register to be notified (useful in Service or other non-UI components):
     *
     *   Foreground.Listener myListener = new Foreground.Listener(){
     *       public void onBecameForeground(){
     *           // ... whatever you want to do
     *       }
     *       public void onBecameBackground(){
     *           // ... whatever you want to do
     *       }
     *   }
     *
     *   public void onCreate(){
     *      super.onCreate();
     *      Foreground.get(this).addListener(listener);
     *   }
     *
     *   public void onDestroy(){
     *      super.onCreate();
     *      Foreground.get(this).removeListener(listener);
     *   }
     */
    public class Foreground : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        public const long CheckDelay = 600;
        public const string Tag = nameof(Foreground);

        public interface IListener
        {
            void OnBecameForeground();
            void OnBecameBackground();
        }

        private static Foreground instance;

        private bool foreground = true, paused = true;
        private Handler handler = new Handler();
        private SynchronizedCollection<IListener> listeners = new SynchronizedCollection<IListener>();
        private Java.Lang.Runnable check;

        /// <summary>
        /// Its not strictly necessary to use this method - _usually_ invoking
        /// get with a Context gives us a path to retrieve the Application and
        /// initialise, but sometimes(e.g. in test harness) the ApplicationContext
        /// is != the Application, and the docs make no guarantees.
        /// </summary>
        /// <param name="application">application</param>
        /// <returns>return an initialised Foreground instance</returns> 
        public static Foreground Init(Application application)
        {
            if (instance == null)
            {
                instance = new Foreground();
                application.RegisterActivityLifecycleCallbacks(instance);
            }

            return instance;
        }

        public static Foreground SharedInstance(Application application)
        {
            if (instance == null)
            {
                Init(application);
            }
            return instance;
        }

        public static Foreground SharedInstance(Context ctx)
        {
            if (instance == null)
            {
                if (ctx.ApplicationContext is Application appCtx)
                {
                    Init(appCtx);
                }

                throw new Java.Lang.IllegalStateException("Foreground is not initialised and " +
                                                          "cannot obtain the Application object");
            }
            return instance;
        }

        public static Foreground SharedInstance()
        {
            if (instance == null)
            {
                throw new Java.Lang.IllegalStateException("Foreground is not initialised - invoke " +
                                                          "at least once with parameterised init/get");
            }
            return instance;
        }

        public bool IsForeground => foreground;

        public bool IsBackground => !foreground;

        public void AddListener(IListener listener) => listeners.Add(listener);

        public void RemoveListener(IListener listener) => listeners.Remove(listener);

        public void OnActivityResumed(Activity activity)
        {
            paused = false;
            var wasBackground = !foreground;
            foreground = true;

            if (check != null)
            {
                handler.RemoveCallbacks(check);
            }

            if (wasBackground)
            {
                Log.Info(Tag, "went foreground");
                foreach (var l in listeners)
                {
                    try
                    {
                        l.OnBecameForeground();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Tag, "Listener threw exception!", ex);
                    }
                }
            }
            else
            {
                Log.Info(Tag, "still foreground");
            }
        }

        public void OnActivityPaused(Activity activity)
        {
            paused = true;

            if (check != null)
            {
                handler.RemoveCallbacks(check);
            }

            check = new Java.Lang.Runnable(() =>
            {
                if (foreground && paused)
                {
                    foreground = false;
                    Log.Info(Tag, "went background");
                    foreach (var l in listeners)
                    {
                        try
                        {
                            l.OnBecameBackground();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(Tag, "Listener threw exception!", ex);
                        }
                    }
                }
                else
                {
                    Log.Info(Tag, "still foreground");
                }
            });

            handler.PostDelayed(check, CheckDelay);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState) { }

        public void OnActivityStarted(Activity activity) { }

        public void OnActivityStopped(Activity activity) { }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }

        public void OnActivityDestroyed(Activity activity) { }
    }
}
