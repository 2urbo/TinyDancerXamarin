using Android.Support.V7.Widget;
using Android.Views;

namespace TinyDancerXamarin.Sample.UI
{
    /**
    * Created by brianplummer on 8/30/15.
    */
    public class FpsSampleAdpater : RecyclerView.Adapter
    {
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context)
                                         .Inflate(Resource.Layout.Sample_item, parent, false);
            return new FpsSampleViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var fpsSampleViewHolder = holder as FpsSampleViewHolder;
            fpsSampleViewHolder.OnBind(position, MegaBytes);
        }

        public override int ItemCount => 255;

        public float MegaBytes { get; set; } = 1;
    }
}
