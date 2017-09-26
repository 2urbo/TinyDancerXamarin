using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;

namespace TinyDancerXamarin.Sample.UI
{
    /**
    * Created by 206847 on 9/12/15.
    */
    [Register("com.codemonkeylabs.fpslibrary.sample.UI.FpsRecyclerView")]
    public class FpsRecyclerView : RecyclerView
    {
        private readonly FpsSampleAdpater adapter;

        public FpsRecyclerView(Context context) : this(context, null)
        {}

        public FpsRecyclerView(Context context, IAttributeSet attrs) : this (context, attrs, 0)
        {}

        public FpsRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            adapter = new FpsSampleAdpater();
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            var layoutManager = new LinearLayoutManager(Context)
            {
                Orientation = LinearLayoutManager.Vertical
            };
            SetLayoutManager(layoutManager);
            AddItemDecoration(new DividerItemDecoration(Context, DividerItemDecoration.VerticalList));
            SetAdapter(adapter);
        }

        public void SetMegaBytes(float megaBytes) => adapter.MegaBytes = megaBytes;

        public void NotifyDataSetChanged() => adapter.NotifyDataSetChanged();
    }
}
