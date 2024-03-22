using System.Collections;
using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;
using Random = System.Random;

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

        private float m_CharPathDelay;
        private WaitForSeconds m_ResetDelay;

        private void OnEnable()
        {
            GEM.AddListener<GameEvent>(OnLoadLevel, channel: (int)GameEventType.Load);

            GEM.AddListener<PlatformEvent>(UpdateMovingPlatform, channel: (int)PlatformEventType.UpdatePlatforms);
            GEM.AddListener<PlatformEvent>(OnFail, channel: (int)PlatformEventType.Fail);
        }

        private void OnDisable()
        {
            GEM.AddListener<GameEvent>(OnLoadLevel, channel: (int)GameEventType.Load);

            GEM.RemoveListener<PlatformEvent>(UpdateMovingPlatform, channel: (int)PlatformEventType.UpdatePlatforms);
            GEM.RemoveListener<PlatformEvent>(OnFail, channel: (int)PlatformEventType.Fail);
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

            GEM.AddListener<InputEvent>(HandleInput, channel: (int)InputEventType.Tap);
        }

        private void CreateLevel(int platformCount)
        {
            var levelOffset = m_FinishPlatform == null ? 0f : m_FinishPlatform.Position.z;

            for (var i = 0; i < platformCount; i++)
            {
                var platform = PlatformExtensions.GetPlatformFromPool();
                platform.transform.position = new Vector3(0f, 0f, levelOffset + i * 3f);
                platform.Renderer.material = m_GeneralSettings.PlatformColors.GetRandomMaterial();
                m_LevelPlatforms.Add(platform);
            }

            m_FinishPlatform = PlatformExtensions.GetPlatformFromPool();
            m_FinishPlatform.transform.position = new Vector3(0f, 0f, levelOffset + platformCount * 3f);
            m_FinishPlatform.CurrentStateType = Platform.PlatformStateType.Finish;
            m_FinishPlatform.Renderer.material = m_GeneralSettings.FinishPlatformMaterial;

            m_CurrentPlatformIndex = 0;
            UpdateMovingPlatform();
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
            m_FinishPlatform = null;
            using var failEvt = GameEvent.Get(CalculateCharacterPath()).SendGlobal((int)GameEventType.Fail);
            
            OnLevelEnd(false);
        }

        private void OnSuccess()
        {
            m_LevelPlatforms.Add(m_FinishPlatform);
            using var successEvt = GameEvent.Get(CalculateCharacterPath()).SendGlobal((int)GameEventType.Success);
            
            OnLevelEnd(true);
        }

        private void OnLevelEnd(bool success)
        {
            GEM.RemoveListener<InputEvent>(HandleInput, channel: (int)InputEventType.Tap);

            m_ResetDelay = new WaitForSeconds(m_CharPathDelay);
            StartCoroutine(ResetPlatformsWithDelay(success));
        }

        private IEnumerator ResetPlatformsWithDelay(bool success)
        {
            yield return m_ResetDelay;
            
            ResetLevelPlatforms(!success);
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

        private void HandleInput(InputEvent inputEvent)
        {
            using var evt = PlatformEvent.Get(m_StationaryPlatform, m_MovingPlatform)
                .SendGlobal((int)PlatformEventType.CheckSplit);
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

            m_CharPathDelay = path.Count * m_GeneralSettings.CharacterSpeedPerPlatform;
            return (path.ToArray(), m_CharPathDelay);
        }
    }
}