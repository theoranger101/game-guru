namespace Platforms.PlatformStates
{
    public class InactiveState : PlatformState
    {
        public InactiveState(Platform platform) : base(platform)
        {
            platform.gameObject.SetActive(false);
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