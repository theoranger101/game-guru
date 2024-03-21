using Events;
using UnityEngine;

namespace Platforms
{
    public enum PlatformEventType
    {
        Split = 0,
        UpdatePlatforms = 1,
        GetPooledPlatform = 2,
        PerfectHit = 3,
        Fail = 4
    }
    
    public class PlatformEvent : Event<PlatformEvent>
    {
        public Platform Platform1;
        public Platform Platform2;
        
        public static PlatformEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }

        public static PlatformEvent Get(Platform stationaryPlatform, Platform movingPlatform)
        {
            var evt = GetPooledInternal();
            evt.Platform1 = stationaryPlatform;
            evt.Platform2 = movingPlatform;

            return evt;
        }

        public static PlatformEvent Get(Platform pooledPlatform)
        {
            var evt = GetPooledInternal();
            evt.Platform1 = pooledPlatform;

            return evt;
        }
    }
}