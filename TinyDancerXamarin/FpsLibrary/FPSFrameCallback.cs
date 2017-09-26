using System.Collections.Generic;
using System.Linq;
using Android.Views;
using TinyDancerXamarin.FpsLibrary.UI;

namespace TinyDancerXamarin.FpsLibrary
{
    /**
    * Created by brianplummer on 8/29/15.
    */
    public class FpsFrameCallback : Java.Lang.Object, Choreographer.IFrameCallback
    {
        private FpsConfig fpsConfig;
        private TinyCoach tinyCoach;
        private readonly List<long> dataSet; //holds the frame times of the sample set
        private bool enabled = true;
        private long startSampleTimeInNs;

        public FpsFrameCallback(FpsConfig fpsConfig, TinyCoach tinyCoach)
        {
            this.fpsConfig = fpsConfig;
            this.tinyCoach = tinyCoach;
            dataSet = new List<long>();
        }

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public void DoFrame(long frameTimeNanos)
        {
            //if not enabled then we bail out now and don't register the callback
            if (!enabled)
            {
                Destroy();
                return;
            }

            //initial case
            if (startSampleTimeInNs == 0)
            {
                startSampleTimeInNs = frameTimeNanos;
            }
            // only invoked for callbacks....
            else if (fpsConfig.FrameDataCallback != null)
            {
                long start = dataSet.Last();
                int droppedCount = Calculation.DroppedCount(start, frameTimeNanos, fpsConfig.DeviceRefreshRateInMs);
                fpsConfig.FrameDataCallback(start, frameTimeNanos, droppedCount);
            }

            //we have exceeded the sample length ~700ms worth of data...we should push results and save current
            //frame time in new list
            if (IsFinishedWithSample(frameTimeNanos))
            {
                CollectSampleAndSend(frameTimeNanos);
            }

            // add current frame time to our list
            dataSet.Add(frameTimeNanos);

            //we need to register for the next frame callback
            Choreographer.Instance.PostFrameCallback(this);
        }

        private void CollectSampleAndSend(long frameTimeNanos)
        {
            //this occurs only when we have gathered over the sample time ~700ms
            var dataSetCopy = new List<long>();
            dataSetCopy.AddRange(dataSet);

            //push data to the presenter
            tinyCoach.ShowData(fpsConfig, dataSetCopy);

            // clear data
            dataSet.Clear();

            //reset sample timer to last frame
            startSampleTimeInNs = frameTimeNanos;
        }

        /// <summary>
        /// returns true when sample length is exceed
        /// </summary>
        /// <returns><c>true</c>, when sample length is exceed, <c>false</c> otherwise.</returns>
        /// <param name="frameTimeNanos">frameTimeNanos current frame time in NS</param>
        private bool IsFinishedWithSample(long frameTimeNanos)
        {
            return frameTimeNanos - startSampleTimeInNs > fpsConfig.SampleTimeInNs;
        }

        private void Destroy()
        {
            dataSet.Clear();
            fpsConfig = null;
            tinyCoach = null;
        }
    }
}
