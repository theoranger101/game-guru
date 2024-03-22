using System.Collections;
using Events;
using Serialization;
using UnityEngine;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        private SerializedInt LevelIndex = new();

        private GeneralSettings m_GeneralSettings;

        private float m_CharPathDelay;
        private WaitForSeconds m_ResetDelay;

        private void OnEnable()
        {
            m_GeneralSettings = GeneralSettings.Get();

            GEM.AddListener<GameEvent>(OnSuccess, channel: (int)GameEventType.Success);
            GEM.AddListener<GameEvent>(OnFail, channel: (int)GameEventType.Fail);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<GameEvent>(OnSuccess, channel: (int)GameEventType.Success);
            GEM.RemoveListener<GameEvent>(OnFail, channel: (int)GameEventType.Fail);
        }

        private void Start()
        {
            LevelIndex.Key = "LevelIndex";
            ToggleListenToTapInput(true);
        }

        private void OnTapToPlay(InputEvent evt)
        {
            using var startLevelEvt = GameEvent.Get(m_GeneralSettings.LevelPlatformCounts[LevelIndex.Value])
                .SendGlobal((int)GameEventType.Load);
            ToggleListenToTapInput(false);
        }

        private void OnSuccess(GameEvent evt)
        {
            LevelIndex.Value++;

            if (LevelIndex.Value >= m_GeneralSettings.LevelPlatformCounts.Count)
            {
                LevelIndex.Value = 0;
            }

            OnLevelEnd(evt.PathDuration, true);
        }

        private void OnFail(GameEvent evt)
        {
            OnLevelEnd(evt.PathDuration, false);
        }

        private void OnLevelEnd(float delay, bool success)
        {
            m_ResetDelay = new WaitForSeconds(delay);
            
            StartCoroutine(OnLevelEndWithDelay(true, success));
        }

        private IEnumerator OnLevelEndWithDelay(bool listen, bool success)
        {
            yield return m_ResetDelay;
            
            using var evt = GameEvent.Get(success).SendGlobal((int)GameEventType.End);
            ToggleListenToTapInput(listen);
        }

        private void ToggleListenToTapInput(bool listen)
        {
            if (listen)
            {
                GEM.AddListener<InputEvent>(OnTapToPlay, channel: (int)InputEventType.Tap);
            }
            else
            {
                GEM.RemoveListener<InputEvent>(OnTapToPlay, channel: (int)InputEventType.Tap);
            }
        }
    }
}