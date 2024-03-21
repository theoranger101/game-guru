using System.Collections.Generic;
using Events;
using UnityEngine;

namespace Platforms
{
    public class PlatformManager : MonoBehaviour
    {
        private Queue<Platform> m_PlatformPool = new();

        private Platform m_MovingPlatform;
        private Platform m_StationaryPlatform;

        private List<Platform> m_LevelPlatforms = new();
        private int m_CurrentPlatformIndex;

        private GeneralSettings m_GeneralSettings;

        private int m_PoolSize => m_GeneralSettings.PlatformPoolSize;
        private Vector3 m_InitialPlatformScale => m_GeneralSettings.InitialPlatformScale;
        
        private void Awake()
        {
            m_GeneralSettings = GeneralSettings.Get();

            CreatePlatformPool();
            CreateLevel(5);
        }

        private void OnEnable()
        {
            GEM.AddListener<PlatformEvent>(OnGetPlatformFromPool, channel: (int)PlatformEventType.GetPooledPlatform);
            GEM.AddListener<PlatformEvent>(OnUpdatePlatforms, channel: (int)PlatformEventType.UpdatePlatforms);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<PlatformEvent>(OnGetPlatformFromPool, channel: (int)PlatformEventType.GetPooledPlatform);
            GEM.RemoveListener<PlatformEvent>(OnUpdatePlatforms, channel: (int)PlatformEventType.UpdatePlatforms);
        }

        private void OnUpdatePlatforms(PlatformEvent evt)
        {
            UpdateMovingPlatform();
        }

        private void OnGetPlatformFromPool(PlatformEvent evt)
        {
            evt.Platform1 = GetPlatformFromPool();
        }

        private void CreatePlatformPool()
        {
            for (var i = 0; i < m_PoolSize; i++)
            {
                var newPlatformObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newPlatformObj.transform.localScale = m_InitialPlatformScale;

                var newPlatform = newPlatformObj.AddComponent<Platform>();
                newPlatform.CurrentStateType = Platform.PlatformStateType.Inactive;
                
                m_PlatformPool.Enqueue(newPlatform);
            }
        }

        private Platform GetPlatformFromPool()
        {
            var platform = m_PlatformPool.Dequeue();
            return platform;
        }

        private void CreateLevel(int platformCount)
        {
            for (var i = 0; i < platformCount; i++)
            {
                var platform = GetPlatformFromPool();
                platform.transform.position = new Vector3(0f, 0f, i * 3f);
                m_LevelPlatforms.Add(platform);
            }

            m_CurrentPlatformIndex = 0;
            UpdateMovingPlatform();
        }

        private void UpdateMovingPlatform()
        {
            if (m_StationaryPlatform != null)
            {
            }

            m_StationaryPlatform = m_LevelPlatforms[m_CurrentPlatformIndex++];
            m_MovingPlatform = m_LevelPlatforms[m_CurrentPlatformIndex];

            m_StationaryPlatform.CurrentStateType = Platform.PlatformStateType.Stationary;
            m_MovingPlatform.CurrentStateType = Platform.PlatformStateType.Moving;
            
            m_MovingPlatform.transform.localScale = m_StationaryPlatform.transform.localScale;
            
            // m_MovingPlatform.AddComponent<MovingPlatform>();
            // m_MovingPlatform.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SplitPlatform();
            }
        }

        private void SplitPlatform()
        {
            using var evt = PlatformEvent.Get(m_StationaryPlatform, m_MovingPlatform)
                .SendGlobal((int)PlatformEventType.Split);
        }
    }
}