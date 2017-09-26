# TinyDancerXamarin

C# port of the TinyDancer library.

The original TinyDancer library is real time frames per second measuring library for Android that also shows a color coded metric.

Original project and all credit to https://github.com/friendlyrobotnyc/TinyDancer

## Min SDK
TinyDancerXamarin min sdk is API 16

**Unfortunately this will not work on Android TV**

## Usage

#### Installation
Clone or download solution and use TinyDancerXamarin project in your solution.

#### Initialization
``` csharp
using TinyDancerXamarin.FpsLibrary;

namespace TinyDancerXamarin.Sample
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            TinyDancer.Create().Show(Context);

            //alternatively
            TinyDancer.Create()
               .RedFlagPercentage(.1f) // set red indicator for 10%....different from default
               .StartingXPosition(200)
               .StartingYPosition(600)
               .Show(Context);

            //you can add a callback to get frame times and the calculated
            //number of dropped frames within that window
            TinyDancer.Create().AddFrameDataCallback((previousFrameNS, currentFrameNS, droppedFrames) =>
            {
                //collect your stats here
            }).Show(Context);
        }
    }
}
```
