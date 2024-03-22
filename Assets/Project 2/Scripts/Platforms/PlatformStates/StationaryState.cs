namespace Platforms.PlatformStates
{
    public class StationaryState : PlatformState
    {
        public StationaryState(Platform platform) : base(platform)
        {
            platform.gameObject.SetActive(true);
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