namespace Extensions
{
    public static class IntExtension
    {
        public static int SetBitToOne(this int value, int position)
        {
            return value |= (1 << position);
        }
    
        public static int SetBitToZero(this int value, int position)
        {
            return value &= ~(1 << position);
        }
    
        public static bool IsBitOne(this int value, int position)
        {
            return (value & (1 << position)) != 0;
        }
    }
}