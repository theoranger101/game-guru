using Core;
using Events;
using Platforms;
using Sounds;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField]
    private bool m_ComboStarted = false;

    [SerializeField]
    private int m_CurrentCombo = 0;

    private Sound ComboSound => GeneralSettings.Get().PerfectHitSound;

    private void OnEnable()
    {
        GEM.AddListener<PlatformEvent>(OnPerfectHit, channel: (int)PlatformEventType.PerfectHit);
    }

    private void OnDisable()
    {
        GEM.RemoveListener<PlatformEvent>(OnPerfectHit, channel: (int)PlatformEventType.PerfectHit);
    }

    private void OnPerfectHit(PlatformEvent evt)
    {
        m_CurrentCombo++;

        using var playSoundEvt = SoundEvent.Get(ComboSound, m_CurrentCombo).SendGlobal();

        if (m_ComboStarted) return;

        m_ComboStarted = true;
        GEM.AddListener<PlatformEvent>(OnComboBroken, channel: (int)PlatformEventType.OnSplit);
    }

    private void OnComboBroken(PlatformEvent evt)
    {
        m_CurrentCombo = 0;
        m_ComboStarted = false;

        GEM.RemoveListener<PlatformEvent>(OnComboBroken, channel: (int)PlatformEventType.OnSplit);
    }
}