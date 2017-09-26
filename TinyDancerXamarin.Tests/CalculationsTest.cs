using System.Collections.Generic;
using Java.Util.Concurrent;
using NUnit.Framework;
using TinyDancerXamarin.FpsLibrary;

namespace TinyDancerXamarin.Tests
{
    [TestFixture]
    public class CalculationsTest
    {
        // 16.6ms
        private readonly long oneFrameNS = TimeUnit.Nanoseconds.Convert(16600, TimeUnit.Microseconds);

        [SetUp]
        public void Setup() { }

        [TearDown]
        public void Tear() { }

        [Test]
        public void TestBaseCase()
        {
            var fpsConfig = new FpsConfig();
            var dataSet = new List<long>();
            dataSet.Add(0L);
            dataSet.Add(TimeUnit.Nanoseconds.Convert(50, TimeUnit.Milliseconds));
            var droppedSet = Calculation.GetDroppedSet(fpsConfig, dataSet);
            Assert.AreEqual(droppedSet.Count, 1);
            Assert.AreEqual(droppedSet[0], 2);
        }

        [Test]
        public void TestBaseGetAmountOfFramesInSet()
        {
            var fpsConfig = new FpsConfig();
            Assert.AreEqual(Calculation.GetNumberOfFramesInSet(oneFrameNS, fpsConfig), 1);
            Assert.AreEqual(Calculation.GetNumberOfFramesInSet(oneFrameNS * 5, fpsConfig), 5);
            Assert.AreEqual(Calculation.GetNumberOfFramesInSet(oneFrameNS * 58, fpsConfig), 58);
        }

        [Test]
        public void TestCalculateMetric()
        {
            var fpsConfig = new FpsConfig();
            long start = 0;
            long end = oneFrameNS * 100;
            Assert.AreEqual(Calculation.GetNumberOfFramesInSet(end, fpsConfig), 100);

            var dataSet = new List<long> { start, end };
            var droppedSet = new List<int>();

            droppedSet.Add(4);
            Assert.AreEqual(Calculation.CalculateMetric(fpsConfig, dataSet, droppedSet).Key,
                            Calculation.Metric.Good);

            droppedSet.Add(6);
            Assert.AreEqual(Calculation.CalculateMetric(fpsConfig, dataSet, droppedSet).Key,
                            Calculation.Metric.Medium);

            droppedSet.Add(10);
            Assert.AreEqual(Calculation.CalculateMetric(fpsConfig, dataSet, droppedSet).Key,
                            Calculation.Metric.Bad);
        }
    }
}
