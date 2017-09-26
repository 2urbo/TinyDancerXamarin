using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace TinyDancerXamarin.Sample.UI
{
    /**
     * Created by brianplummer on 8/30/15.
     */
    public class FpsSampleViewHolder : RecyclerView.ViewHolder
    {

        private int[] data;
        private ImageView colorImg;
        private TextView bindTime;

        public FpsSampleViewHolder(View itemView) : base(itemView)
        {
            colorImg = itemView.FindViewById<ImageView> (Resource.Id.colorImg);
            bindTime = itemView.FindViewById<TextView> (Resource.Id.bindTime);
            data = new int[1024 * 10];
        }

        public void OnBind(int value, float megaBytes)
        {
            ConfigureColor(value);

            var total = (int)(megaBytes * 100f);
            var start = SystemClock.ElapsedRealtime();
            var startInt = (int)start;
            for (int i = 0; i < total; i++)
            {
                for (int e = 0; e < data.Length; e++)
                {
                    // set dummy value (start time)
                    data[e] = startInt;
                }
            }
            var end = SystemClock.ElapsedRealtime();
            var bindTimeMs = end - start;

            bindTime.Text = $"{bindTimeMs} ms onBind()";
        }

        private void ConfigureColor(int value)
        {
            int multiplier = 22;
            int hundred = value / 100;
            int tens = (value - (hundred) * 100) / 10;
            int ones = value - (hundred * 100) - (tens * 10);
            int r = hundred * multiplier;
            int g = tens * multiplier;
            int b = ones * multiplier;
            var colorVal = Color.Rgb(r, g, b);
            colorImg.SetImageDrawable(new ColorDrawable(colorVal));
        }
    }
}
