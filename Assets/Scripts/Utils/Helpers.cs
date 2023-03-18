namespace Utils
{
    public static class Helpers
    {
        public static int WrapNumber(int value, int min, int max)
        {
            if (value < min)
            {
                return max - (min - value) % (max - min);
            }
            return min + (value - min) % (max - min);
        }
    }
}