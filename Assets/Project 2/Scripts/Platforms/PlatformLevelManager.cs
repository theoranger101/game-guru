using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Platforms
{
    public class PlatformLevelManager : MonoBehaviour
    {
        [SerializeField]
        private Platform m_MovingPlatform;

        [SerializeField]
        private Platform m_StationaryPlatform;

        [SerializeField]
        private Platform m_FinishPlatform;

        [SerializeField]
        private int m_CurrentPlatformIndex;

        private List<Platform> m_LevelPlatforms = new();

        private GeneralSettings m_GeneralSettings;

        private Vector3 m_InitialPlatformScale => m_GeneralSettings.InitialPlatformScale;

        private void OnEnable()
        {
            GEM.AddListener<GameEvent>(OnLoadLevel, channel: (int)GameEventType.Load);
            GEM.AddListener<GameEvent>(OnLevelEnd, channel: (int)GameEventType.End);

            GEM.AddListener<PlatformEvent>(UpdateMovingPlatform, channel: (int)PlatformEventType.UpdatePlatforms);
            GEM.AddListener<PlatformEvent>(OnFail, channel: (int)PlatformEventType.Fall);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<GameEvent>(OnLoadLevel, channel: (int)GameEventType.Load);
            GEM.RemoveListener<GameEvent>(OnLevelEnd, channel: (int)GameEventType.End);

            GEM.RemoveListener<PlatformEvent>(UpdateMovingPlatform, channel: (int)PlatformEventType.UpdatePlatforms);
            GEM.RemoveListener<PlatformEvent>(OnFail, channel: (int)PlatformEventType.Fall);
        }

        private void Start()
        {
            m_GeneralSettings = GeneralSettings.Get();
        }

        private void OnLoadLevel(GameEvent evt)
        {
            CreateLevel(evt.PlatformCount);

            using var startEvt = GameEvent.Get(m_StationaryPlatform.Position.WithY(m_GeneralSettings.GlobalY))
                .SendGlobal((int)GameEventType.Start);

            ToggleListenToInput(true);
        }

        private void CreateLevel(int platformCount)
        {
            var levelOffset = m_FinishPlatform == null ? 0f : m_FinishPlatform.Position.z;

            for (var i = 0; i < platformCount; i++)
            {
                var platform = SetupPlatform(levelOffset + i * 3f);
                m_LevelPlatforms.Add(platform);
            }

            SetupFinishPlatform(levelOffset + platformCount * 3f);

            m_CurrentPlatformIndex = 0;
            UpdateMovingPlatform();
        }

        private Platform SetupPlatform(float pos)
        {
            var platform = PlatformExtensions.GetPlatformFromPool();
            platform.CurrentStateType = Platform.PlatformStateType.Inactive;
            
            var platformT = platform.transform;

            platformT.position = new Vector3(0f, 0f, pos);
            platformT.localScale = m_InitialPlatformScale;

            platform.Renderer.material = m_GeneralSettings.PlatformColors.GetRandomMaterial();
            
            return platform;
        }
        
        private void SetupFinishPlatform(float pos)
        {
            m_FinishPlatform = SetupPlatform(pos);
            
            m_FinishPlatform.CurrentStateType = Platform.PlatformStateType.Finish;
            m_FinishPlatform.Renderer.material = m_GeneralSettings.FinishPlatformMaterial;
        }

        private void ResetLevelPlatforms(bool setInactive)
        {
            for (var i = 0; i < m_LevelPlatforms.Count; i++)
            {
                var platform = m_LevelPlatforms[i];
                platform.AddPlatformToPool();

                if (setInactive)
                {
                    platform.CurrentStateType = Platform.PlatformStateType.Inactive;
                }
            }

            m_LevelPlatforms.Clear();
        }

        private void OnFail(PlatformEvent evt)
        {
            ToggleListenToInput(false);

            m_FinishPlatform = null;
            using var failEvt = GameEvent.Get(CalculateCharacterPath()).SendGlobal((int)GameEventType.Fail);
        }

        private void OnSuccess()
        {
            ToggleListenToInput(false);

            m_LevelPlatforms.Add(m_FinishPlatform);
            using var successEvt = GameEvent.Get(CalculateCharacterPath()).SendGlobal((int)GameEventType.Success);
        }

        private void OnLevelEnd(GameEvent evt)
        {
            ResetLevelPlatforms(!evt.Success);
        }

        private void UpdateMovingPlatform(PlatformEvent evt = null)
        {
            if (m_CurrentPlatformIndex + 1 == m_LevelPlatforms.Count)
            {
                OnSuccess();
                return;
            }

            m_StationaryPlatform = m_LevelPlatforms[m_CurrentPlatformIndex++];
            m_MovingPlatform = m_LevelPlatforms[m_CurrentPlatformIndex];

            m_StationaryPlatform.CurrentStateType = Platform.PlatformStateType.Stationary;
            m_MovingPlatform.CurrentStateType = Platform.PlatformStateType.Moving;

            m_MovingPlatform.transform.localScale = m_StationaryPlatform.transform.localScale;
        }

        private (Vector3[], float) CalculateCharacterPath()
        {
            var path = new List<Vector3>();

            for (var i = 0; i < m_LevelPlatforms.Count; i++)
            {
                if (m_LevelPlatforms[i].CurrentStateType == Platform.PlatformStateType.CutOff)
                {
                    path.Add(path[i - 1].WithZ(m_LevelPlatforms[i].Collider.bounds.center.z));
                    break;
                }

                path.Add(m_LevelPlatforms[i].Collider.bounds.center.WithY(m_GeneralSettings.GlobalY));
            }

            var duration = path.Count * m_GeneralSettings.CharacterSpeedPerPlatform;
            return (path.ToArray(), duration);
        }

        private void HandleInput(InputEvent inputEvent)
        {
            using var evt = PlatformEvent.Get(m_StationaryPlatform, m_MovingPlatform)
                .SendGlobal((int)PlatformEventType.CheckSplit);
        }

        private void ToggleListenToInput(bool listen)
        {
            if (listen)
            {
                GEM.AddListener<InputEvent>(HandleInput, channel: (int)InputEventType.Tap);
            }
            else
            {
                GEM.RemoveListener<InputEvent>(HandleInput, channel: (int)InputEventType.Tap);
            }
        }
    }
}