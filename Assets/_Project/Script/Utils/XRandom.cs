using System;

namespace Util
{
    public static class XRandom
    {
        private static Random _random;

        static XRandom()
        {
            _random = new Random((int)DateTime.UtcNow.Ticks);
        }

        public static bool NextBool()
        {
            return _random.Next(0, 2) == 1;
        }

        public static int NextInt()
        {
            return _random.Next();
        }

        public static float NextFloat()
        {
            return (float)_random.NextDouble();
        }

        public static int Range(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float)_random.NextDouble() * (max - min) + min;
        }
    }
}
