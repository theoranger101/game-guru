using System;
using UnityEngine;

namespace Platforms.PlatformStates
{
    [Serializable]
    public abstract class PlatformState
    {
        public PlatformState(Platform platform)
        {
            this.Platform = platform;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected Platform Platform { get; private set; }
    }
}