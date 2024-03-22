using Platforms.PlatformStates;
using UnityEngine;

namespace Platforms
{
    public class Platform : MonoBehaviour
    {
        // implementing state logic for this simple structure where states rarely change and each state does not have much
        // special functionality is not very viable, i implemented it as an example.
        public enum PlatformStateType
        {
            Moving = 0,
            Stationary = 1,
            CutOff = 2,
            Finish = 3, 
            Inactive = 4,
        };

        private PlatformState m_CurrentState;

        public PlatformStateType CurrentStateType
        {
            get => m_CurrentStateType;
            set
            {
                m_CurrentStateType = value;
                SetState(m_CurrentStateType);
            }
        }

        private PlatformStateType m_CurrentStateType;
        
        public Vector3 Position => transform.position;

        public BoxCollider Collider;
        public Renderer Renderer;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
            Renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            m_CurrentState.UpdateState();
        }

        private void SetState(PlatformStateType stateType)
        {
            m_CurrentState = stateType switch
            {
                PlatformStateType.Moving => new MovingState(this),
                PlatformStateType.Stationary => new StationaryState(this),
                PlatformStateType.CutOff => new CutOffState(this),
                PlatformStateType.Finish => new FinishState(this),
                PlatformStateType.Inactive => new InactiveState(this),
                _ => m_CurrentState
            };

            m_CurrentState.EnterState();
        }
    }
}