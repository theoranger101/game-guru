using Events;
using UnityEngine;

namespace Project_2.Scripts.Platforms
{
    public enum PlatformEventType
    {
        Split = 0,
        UpdatePlatforms = 1,
        GetPooledPlatform = 2,
    }
    
    public class PlatformEvent : Event<PlatformEvent>
    {
        public GameObject Platform1;
        public GameObject Platform2;
        
        public static PlatformEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }

        public static PlatformEvent Get(GameObject stationaryPlatform, GameObject movingPlatform)
        {
            var evt = GetPooledInternal();
            evt.Platform1 = stationaryPlatform;
            evt.Platform2 = movingPlatform;

            return evt;
        }

        public static PlatformEvent Get(GameObject pooledPlatform)
        {
            var evt = GetPooledInternal();
            evt.Platform1 = pooledPlatform;

            return evt;
        }

        public override void Dispose()
        {
            base.Dispose();

            Platform1 = null;
            Platform2 = null;
        }
    }
}