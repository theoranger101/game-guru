using Events;

namespace Platforms
{
    public enum PlatformEventType
    {
        CheckSplit = 0,
        OnSplit = 1,
        UpdatePlatforms = 2,
        PerfectHit = 3,
        Fall = 4,
        GetPooledPlatform = 5,
        AddPlatformToPool = 6,
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