using Android.Content;

namespace TinyDancerXamarin.FpsLibrary
{
    /**
    * Created by brianplummer on 8/29/15.
    */
    public static class TinyDancer
    {
        public static TinyDancerBuilder Create() => new TinyDancerBuilder();

        public static void Hide(Context context) => TinyDancerBuilder.Hide(context.ApplicationContext);
    }
}
