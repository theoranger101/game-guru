using System.Collections.Generic;
using Sounds;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "General Settings")]
    public class GeneralSettings : ScriptableObject
    {
        private static GeneralSettings _GeneralSettings;

        private static GeneralSettings generalSettings
        {
            get
            {
                if (!_GeneralSettings)
                {
                    _GeneralSettings = Resources.Load<GeneralSettings>($"GeneralSettings");
                }

                return _GeneralSettings;
            }
        }

        public static GeneralSettings Get()
        {
            return generalSettings;
        }

        [Header("Level Settings")]
        public List<int> LevelPlatformCounts = new List<int>();

        [Header("Platform Settings")]
        public int PlatformPoolSize = 32;

        public Vector3 InitialPlatformScale = new(3f, 1f, 3f);
        public float PlatformMoveSpeed = 1f;
        public float GlobalY => InitialPlatformScale.y / 2f;

        public float PlatformMoveRange = 5f;
        
        public List<Material> PlatformColors = new List<Material>();
        public Material FinishPlatformMaterial;

        [Header("Tolerance Settings")]
        public float PerfectHitTolerance = 0.05f;

        public float FailTolerance = 0.38f;

        [Header("Other Settings")]
        public float CharacterSpeedPerPlatform = 1f;

        public Sound PerfectHitSound;
    }
}