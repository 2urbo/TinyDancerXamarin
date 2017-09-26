using System.Runtime.CompilerServices;
using Android.Views;
using Java.Util.Concurrent;

[assembly: InternalsVisibleTo("TinyDancerXamarin.Tests")]
namespace TinyDancerXamarin.FpsLibrary
{
    public class FpsConfig
    {
        public static GravityFlags DefaultGravity { get; set; } = GravityFlags.Top | GravityFlags.Start;

        public float RedFlagPercentage { get; set; } = 0.2f; //
        public float YellowFlagPercentage { get; set; } = 0.05f; //
        public float RefreshRate { get; set; } = 60; //60fps
        public float DeviceRefreshRateInMs { get; set; } = 16.6f; //value from device ex 16.6 ms

        // starting coordinates
        public int StartingXPosition { get; set; } = 200;
        public int StartingYPosition { get; set; } = 600;
        public GravityFlags StartingGravity { get; set; } = DefaultGravity;
        public bool XOrYSpecified { get; set; } = false;
        public bool GravitySpecified { get; set; } = false;

        // client facing callback that provides frame info
        public FrameDataCallback FrameDataCallback { get; set; }

        // making final for now.....want to be solid on the math before we allow an
        // arbitrary value
        public readonly long SampleTimeInMs = 736;//928;//736; // default sample time

        internal FpsConfig()
        { }

        public long SampleTimeInNs => TimeUnit.Nanoseconds.Convert(SampleTimeInMs, TimeUnit.Milliseconds);

        public long DeviceRefreshRateInNs => (long)(DeviceRefreshRateInMs * 1000000f);
    }
}
