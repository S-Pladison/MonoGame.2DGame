using System;

namespace Pladi.Utilities
{
    public static class RandUtils
    {
        public static float NextFloat(this Random random, float minValue, float maxValue)
            => (float)random.NextDouble() * (maxValue - minValue) + minValue;
    }
}