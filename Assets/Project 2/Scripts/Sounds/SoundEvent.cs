using Events;

namespace Sounds
{
    public class SoundEvent : Event<SoundEvent>
    {
        public Sound Sound;
        public float Pitch;
        
        public static SoundEvent Get(Sound sound)
        {
            var evt = GetPooledInternal();
            evt.Sound = sound;
            
            return evt;
        }
        
        public static SoundEvent Get(Sound sound, float pitch)
        {
            var evt = GetPooledInternal();
            evt.Sound = sound;
            evt.Pitch = pitch;
            
            return evt;
        }
    }
}
