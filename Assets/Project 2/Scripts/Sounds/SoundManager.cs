using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;

namespace Sounds
{
    // i re-utilized a sound manager i used in other projects. 
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        private List<AudioSource> m_AudioSources => GetComponents<AudioSource>().ToList();

        private int m_SourceIndex = 0;
        
        private void Awake()
        {
            m_SourceIndex = 0;

            Reset();

            GEM.AddListener<SoundEvent>(OnSoundPlayEvent);
        }

        private void OnSoundPlayEvent(SoundEvent evt)
        {
            PlaySound(evt.Sound, evt.Pitch);
        }

        private void PlaySound(Sound sound)
        {
            PlayOneShot(sound);
        }
        
        private void PlaySound(Sound sound, float pitch)
        {
            PlayOneShot(sound, pitch:pitch);
        }
        
        private AudioSource GetSource()
        {
            var src = m_AudioSources[m_SourceIndex++];
            m_SourceIndex %= m_AudioSources.Count;
            return src;
        }

        public void Reset()
        {
        }

        private void PlayOneShot(Sound sound, float volume = 1f, float pitch = 1f)
        {
            if (!sound || !sound.Clip || sound.Volume < 1e-2f)
            {
                Debug.Log($"Ignoring sound {sound.name}");
                return;
            }

            var src = GetSource();
            src.PlayOneShot(sound, volume, pitch);
        }
    }

    public static class SoundExtensions
    {
        public static void PlayOneShot(this AudioSource src, Sound sound, float volume = 1f, float pitch = 1f)
        {
            src.pitch = sound.Pitch * pitch;
            src.loop = sound.Loop;
            src.PlayOneShot(sound.Clip, sound.Volume * volume);
        }
    }
}