using Events;
using UnityEngine;

namespace Core
{
    public enum GameEventType
    {
        Load = 0,
        Start = 1,
        Success = 2,
        Fail = 3,
    }

    public class GameEvent : Event<GameEvent>
    {
        public int PlatformCount;
        
        public Vector3 CharacterStartPosition;
        
        public Vector3[] CharacterPath;
        public float PathDuration;
        
        public static GameEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
        
        public static GameEvent Get(int platformCount)
        {
            var evt = GetPooledInternal();
            evt.PlatformCount = platformCount;

            return evt;
        }
        
        public static GameEvent Get(Vector3 charStartPos)
        {
            var evt = GetPooledInternal();
            evt.CharacterStartPosition = charStartPos;
            return evt;
        }

        public static GameEvent Get((Vector3[] path, float duration) valueTuple)
        {
            var evt = GetPooledInternal();
            evt.CharacterPath = valueTuple.path;
            evt.PathDuration = valueTuple.duration;

            return evt;
        }
    }
}