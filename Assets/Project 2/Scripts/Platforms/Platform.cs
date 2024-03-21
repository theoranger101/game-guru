using System;
using Platforms.PlatformStates;
using UnityEngine;

namespace Platforms
{
    public class Platform : MonoBehaviour
    {
        // implementing state logic for this simple structure where states rarely change and each state does not have much
        // special functionality is not very viable, i implemented it to show an example of my knowledge. 
        public enum PlatformStateType
        {
            Moving = 0,
            Stationary = 1,
            CutOff = 2,
            Finish = 3, 
            Inactive = 4,
        };

        public PlatformState m_CurrentState;

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
        
        // void Start()
        // {
        //     // Set the initial state (change as needed)
        //     CurrentStateType = PlatformStateType.Stationary;
        //     SetState(CurrentStateType);
        // }

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

        // You can add methods to trigger state transitions based on your game logic
        public void TriggerMoveToFinish()
        {
            SetState(PlatformStateType.Moving);
        }
    }
}