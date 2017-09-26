using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using TinyDancerXamarin.Sample.UI;
using TinyDancerXamarin.FpsLibrary;

namespace TinyDancerXamarin.Sample
{
    [Activity(Label = "Tiny Dancer Xamarin Sample", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private FpsRecyclerView recyclerView;
        private SeekBar loadIndicator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            recyclerView = FindViewById<FpsRecyclerView>(Resource.Id.recyclerView);
            loadIndicator = FindViewById<SeekBar>(Resource.Id.loadIndicator);

            var startButton = FindViewById<Button>(Resource.Id.start);
            startButton.Click += (sender, e) =>
            {
                TinyDancer.Create().Show(ApplicationContext);
            };

            var stopButton = FindViewById<Button>(Resource.Id.stop);
            stopButton.Click += (sender, e) =>
            {
                TinyDancer.Hide((Application)ApplicationContext);
            };

            SetupRadioGroup();

            TinyDancer.Create().Show(ApplicationContext);
        }

        private void SetupRadioGroup()
        {
            recyclerView.SetMegaBytes(50f);
            recyclerView.NotifyDataSetChanged();

            loadIndicator.Progress = 50;
            loadIndicator.ProgressChanged += (sender, e) =>
            {
                recyclerView.SetMegaBytes(e.Progress);
                recyclerView.NotifyDataSetChanged();
            };
        }
    }
}

