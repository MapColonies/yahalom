namespace com.mapcolonies.core.Utilities
{
    public static class TimeUtilities
    {
        private const int SECONDS_TO_MILLISECONDS = 1000;

        public static double SecondsToMilliseconds(double seconds)
        {
            return seconds * SECONDS_TO_MILLISECONDS;
        }

        public static double MillisecondsToSeconds(double seconds)
        {
            return seconds / SECONDS_TO_MILLISECONDS;
        }
    }
}
