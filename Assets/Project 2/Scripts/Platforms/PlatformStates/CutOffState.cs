using UnityEngine;

namespace Platforms.PlatformStates
{
    public class CutOffState : PlatformState
    {
        private float m_FallDownTime = 5f;
        private float m_FallDownTimer = 0f;
        
        public CutOffState(Platform platform) : base(platform)
        {
            Platform.gameObject.AddComponent<Rigidbody>();
            Platform.gameObject.SetActive(true);
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            m_FallDownTimer += Time.deltaTime;

            if (!(m_FallDownTimer >= m_FallDownTime)) return;
            
            Platform.gameObject.SetActive(false);
            Platform.CurrentStateType = Platform.PlatformStateType.Inactive;
            
            // add back to queue
        }

        public override void ExitState()
        {
        }
    }
}