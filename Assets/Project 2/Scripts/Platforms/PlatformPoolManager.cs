using System.Collections.Generic;
using Core;
using Events;
using UnityEngine;

namespace Platforms
{
    public class PlatformPoolManager : MonoBehaviour
    {
        private Queue<Platform> m_PlatformPool = new();
        
        private GeneralSettings m_GeneralSettings;

        private int m_PoolSize => m_GeneralSettings.PlatformPoolSize;
        private Vector3 m_InitialPlatformScale => m_GeneralSettings.InitialPlatformScale;

        private void Awake()
        {
            m_GeneralSettings = GeneralSettings.Get();

            CreatePool();
        }

        private void OnEnable()
        {
            GEM.AddListener<PlatformEvent>(OnGetFromPool, channel: (int)PlatformEventType.GetPooledPlatform);
            GEM.AddListener<PlatformEvent>(OnAddToPool, channel: (int)PlatformEventType.AddPlatformToPool);
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<PlatformEvent>(OnGetFromPool, channel: (int)PlatformEventType.GetPooledPlatform);
            GEM.RemoveListener<PlatformEvent>(OnAddToPool, channel: (int)PlatformEventType.AddPlatformToPool);
        }
        
        private void CreatePool()
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
        
        private Platform GetFromPool()
        {
            var platform = m_PlatformPool.Dequeue();
            return platform;
        }
        
        private void OnGetFromPool(PlatformEvent evt)
        {
            evt.Platform1 = GetFromPool();
        }

        private void AddToPool(Platform platform)
        {
            m_PlatformPool.Enqueue(platform);
        }

        private void OnAddToPool(PlatformEvent evt)
        {
            AddToPool(evt.Platform1);
        }
    }
    
    public static class PlatformExtensions
    {
        public static Platform GetPlatformFromPool()
        {
            using var evt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.GetPooledPlatform);
            return evt.Platform1;
        }
        
        public static void AddPlatformToPool(this Platform platform)
        {
            using var evt = PlatformEvent.Get(platform).SendGlobal((int)PlatformEventType.AddPlatformToPool);
        }
    }
}