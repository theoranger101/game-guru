namespace Platforms.PlatformStates
{
    public class FinishState : PlatformState
    {
        public FinishState(Platform platform) : base(platform)
        {
            Platform.gameObject.SetActive(true);
            Platform.gameObject.layer = 6;
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