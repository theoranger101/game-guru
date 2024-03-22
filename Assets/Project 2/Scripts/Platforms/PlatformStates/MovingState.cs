using Core;
using UnityEngine;

namespace Platforms.PlatformStates
{
    public class MovingState : PlatformState
    {
        private Vector3 m_MoveDirection = Vector3.right;

        private Transform m_Transform;
        private GeneralSettings m_GeneralSettings;

        private float m_MoveSpeed => m_GeneralSettings.PlatformMoveSpeed;

        private float m_MoveRange => m_GeneralSettings.PlatformMoveRange;

        private Vector3 m_InitialPosition;
        private Vector3 m_TargetPosition;

        public MovingState(Platform platform) : base(platform)
        {
            m_Transform = platform.transform;
            m_GeneralSettings = GeneralSettings.Get();

            platform.gameObject.SetActive(true);
        }

        public override void EnterState()
        {
            m_InitialPosition = m_Transform.position;
            UpdateTargetPosition();
        }

        public override void UpdateState()
        {
            m_Transform.position =
                Vector3.MoveTowards(m_Transform.position, m_TargetPosition, m_MoveSpeed * Time.deltaTime);

            if (!(Vector3.Distance(m_Transform.position, m_TargetPosition) < 0.01f)) return;

            m_MoveDirection *= -1f;
            UpdateTargetPosition();
        }

        private void UpdateTargetPosition()
        {
            m_TargetPosition = m_InitialPosition + (m_MoveDirection * m_MoveRange);
        }

        public override void ExitState()
        {
        }
    }
}