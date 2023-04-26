using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Core
{
    public class FrameCounterComponent : BasicComponent
    {
        // [public static properties and fields]

        public static readonly int MaximumSamples;

        // [static constructors]

        static FrameCounterComponent()
        {
            MaximumSamples = 100;
        }

        // [public properties and fields]

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        // [private properties and fields]

        private readonly Queue<float> sampleBuffer;

        // [constructors]

        public FrameCounterComponent()
        {
            sampleBuffer = new Queue<float>();
        }

        // [public methods]

        public override void Initialize()
        {
            UpdateOrder = int.MaxValue;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
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