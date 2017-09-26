using System;
using System.Collections.Generic;
using System.Linq;
using Java.Util.Concurrent;

namespace TinyDancerXamarin.FpsLibrary
{
    public static class Calculation
    {
        public enum Metric 
        { 
            Good, 
            Bad, 
            Medium 
        };

        public static List<int> GetDroppedSet(FpsConfig fpsConfig, List<long> dataSet)
        {
            var droppedSet = new List<int>();
            long start = -1;

            foreach (var item in dataSet)
            {
                if (start == -1)
                {
                    start = item;
                    continue;
                }

                var droppedCount = DroppedCount(start, item, fpsConfig.DeviceRefreshRateInMs);
                if (droppedCount > 0)
                {
                    droppedSet.Add(droppedCount);
                }
                start = item;
            }

            return droppedSet;
        }

        public static int DroppedCount(long start, long end, float devRefreshRate)
        {
            int count = 0;
            var diffNs = end - start;

            var diffMs = TimeUnit.Milliseconds.Convert(diffNs, TimeUnit.Nanoseconds);
            var dev = (long)Math.Round(devRefreshRate);
            if (diffMs > dev)
            {
                long droppedCount = (diffMs / dev);
                count = (int)droppedCount;
            }

            return count;
        }

        public static KeyValuePair<Metric,long> CalculateMetric(FpsConfig fpsConfig,
                                                                List<long> dataSet,
                                                                List<int> droppedSet)
        {
            var timeInNS = dataSet.Last() - dataSet.First();
            var size = GetNumberOfFramesInSet(timeInNS, fpsConfig);

            //metric
            int runningOver = 0;
            // total dropped
            int dropped = 0;

            foreach (var k in droppedSet)
            {
                dropped += k;
                if (k >= 2)
                {
                    runningOver += k;
                }
            }

            var multiplier = fpsConfig.RefreshRate / size;
            var answer = multiplier * (size - dropped);
            var realAnswer = (long)Math.Round(answer);

            // calculate metric
            var percentOver = runningOver / (float)size;
            var metric = Metric.Good;
            if (percentOver >= fpsConfig.RedFlagPercentage)
            {
                metric = Metric.Bad;
            }
            else if (percentOver >= fpsConfig.YellowFlagPercentage)
            {
                metric = Metric.Medium;
            }

            return new KeyValuePair<Metric, long>(metric, realAnswer);
        }

        internal static long GetNumberOfFramesInSet(long realSampleLengthNs, FpsConfig fpsConfig)
        {
            float realSampleLengthMs = TimeUnit.Milliseconds.Convert(realSampleLengthNs, TimeUnit.Nanoseconds);
            float size = realSampleLengthMs / fpsConfig.DeviceRefreshRateInMs;
            return (long)Math.Round(size);
        }
    }
}
