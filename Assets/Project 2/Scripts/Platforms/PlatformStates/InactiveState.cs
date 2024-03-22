using UnityEngine;

namespace Platforms.PlatformStates
{
    public class InactiveState : PlatformState
    {
        public InactiveState(Platform platform) : base(platform)
        {
            Platform.transform.position = Vector3.zero;
            Platform.gameObject.SetActive(false);
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }
    }
}