namespace TinyDancerXamarin.FpsLibrary
{
    /// <summary>
    /// this is called for every DoFrame() on the choreographer callback
    /// use this very judiciously.  Logging synchronously from here is a bad
    /// idea as FrameDataCallback will be called every 16-32ms.
    /// </summary>
    /// <param name="previousFrameNS">previous vsync frame time in NS</param>
    /// <param name="currentFrameNS">current vsync frame time in NS</param>
    /// <param name="droppedFrames">number of dropped frames between current and previous times</param>
    public delegate void FrameDataCallback(long previousFrameNS, long currentFrameNS, int droppedFrames);
}
