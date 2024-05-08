using System;

namespace Update.Core
{
    internal class SizeCalculation
    {
        public static string GetSizeAsString(long bytes)
        {
            bytes = Math.Abs(bytes);

            long unit = 1024;
            if (bytes < unit)
                return $"{bytes}B";

            int exp = (int)Math.Log(bytes, unit);
            return $"{bytes / Math.Pow(unit, exp):F2} {"KMG"[exp - 1]}B";
        }
    }
}
