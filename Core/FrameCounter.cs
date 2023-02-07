using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Core
{
    public class FrameCounter
    {
        public const int MaximumSamples = 100;

        // ...

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        // ...

        private readonly Queue<float> sampleBuffer;

        // ...

        public FrameCounter()
        {
            sampleBuffer = new Queue<float>();
        }

        // ...

        public void Update()
        {
            var delta = Main.DeltaTime;

            CurrentFramesPerSecond = 1.0f / delta;

            sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (sampleBuffer.Count > MaximumSamples)
            {
                sampleBuffer.Dequeue();
                AverageFramesPerSecond = sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += delta;
        }
    }
}
