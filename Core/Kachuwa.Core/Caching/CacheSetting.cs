using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Caching
{
    public static class CacheSetting
    {
        public static int ShortDuration { get; } = 60; //seconds 
        public static int MediumDuration { get; } = 1200;//seconds
        public static int LongDuration { get; } = 3600;//seconds
    }
}
