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

        //change name
        private void OnTap(InputEvent evt)
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

            OnLevelEnd(evt.PathDuration);
        }

        private void OnFail(GameEvent evt)
        {
            OnLevelEnd(evt.PathDuration);
        }

        private void OnLevelEnd(float delay)
        {
            m_ResetDelay = new WaitForSeconds(delay);
            StartCoroutine(ToggleInputWithDelay(true));
        }

        private IEnumerator ToggleInputWithDelay(bool listen)
        {
            yield return m_ResetDelay;
            ToggleListenToTapInput(listen);
        }

        private void ToggleListenToTapInput(bool listen)
        {
            if (listen)
            {
                GEM.AddListener<InputEvent>(OnTap, channel: (int)InputEventType.Tap);
            }
            else
            {
                GEM.RemoveListener<InputEvent>(OnTap, channel: (int)InputEventType.Tap);
            }
        }
    }
}