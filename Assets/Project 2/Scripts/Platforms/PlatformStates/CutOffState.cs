using UnityEngine;

namespace Platforms.PlatformStates
{
    public class CutOffState : PlatformState
    {
        private float m_FallDownTime = 5f;
        private float m_FallDownTimer = 0f;

        private Rigidbody m_Rigidbody;
        
        public CutOffState(Platform platform) : base(platform)
        {
            m_Rigidbody = Platform.gameObject.AddComponent<Rigidbody>();
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
            Object.Destroy(m_Rigidbody);
            Platform.CurrentStateType = Platform.PlatformStateType.Inactive;
        }

        public override void ExitState()
        {
        }
    }
}