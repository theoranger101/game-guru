using Cinemachine;
using Events;
using Platforms;
using UnityEngine;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        public CinemachineVirtualCamera Camera;

        private void OnEnable()
        {
            GEM.AddListener<PlatformEvent>(OnCheckSplit, channel:(int)PlatformEventType.CheckSplit);
            
            GEM.AddListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Success);
            GEM.AddListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Fail);        
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<PlatformEvent>(OnCheckSplit, channel:(int)PlatformEventType.CheckSplit);
            GEM.RemoveListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Success);
            GEM.RemoveListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Fail);
        }
        
        private void OnCheckSplit(PlatformEvent evt)
        {
            Camera.Follow = evt.Platform2.transform;
        }
        
        private void OnStartMovement(GameEvent evt)
        {
            // i added this part last minute so i am not proud of it...
            Camera.Follow = CharacterBehaviour.CharacterTransform;
        }
    }
}