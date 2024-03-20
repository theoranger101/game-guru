using System;
using System.Collections.Generic;
using Events;
using Project_2.Scripts;
using Project_2.Scripts.Platforms;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Info")]
    public int PlatformPoolSize = 32;

    public Vector3 PlatformInitialScale = new Vector3(3f, 1f, 3f);

    public GameObject EndPlatformPrefab;

    private Queue<GameObject> PlatformPool = new Queue<GameObject>();

    private GameObject movingPlatform;
    private GameObject stationaryPlatform;

    private List<GameObject> levelPlatforms = new List<GameObject>();
    private int currentPlatformIndex;

    private void Awake()
    {
        CreatePlatformPool();
        CreateLevel(5);
    }

    private void OnEnable()
    {
        GEM.AddListener<PlatformEvent>(OnGetPlatformFromPool, channel:(int)PlatformEventType.GetPooledPlatform);
        GEM.AddListener<PlatformEvent>(OnUpdatePlatforms, channel:(int)PlatformEventType.UpdatePlatforms);
    }

    private void OnDisable()
    {
        GEM.RemoveListener<PlatformEvent>(OnGetPlatformFromPool, channel:(int)PlatformEventType.GetPooledPlatform);
        GEM.RemoveListener<PlatformEvent>(OnUpdatePlatforms, channel:(int)PlatformEventType.UpdatePlatforms);
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
        for (var i = 0; i < PlatformPoolSize; i++)
        {
            var newPlatformObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newPlatformObj.transform.localScale = PlatformInitialScale;
            newPlatformObj.AddComponent<Rigidbody>().useGravity = false;
            newPlatformObj.SetActive(false);
            PlatformPool.Enqueue(newPlatformObj);
        }
    }

    private GameObject GetPlatformFromPool()
    {
        var platform = PlatformPool.Dequeue();
        platform.SetActive(true);
        return platform;
    }

    private void CreateLevel(int platformCount)
    {
        for (var i = 0; i < platformCount; i++)
        {
            var platform = GetPlatformFromPool();
            platform.transform.position = new Vector3(0f, 0f, i * 3f);
            levelPlatforms.Add(platform);
            platform.SetActive(i == 0);
        }

        currentPlatformIndex = 0;
        UpdateMovingPlatform();
    }

    private void UpdateMovingPlatform()
    {
        if (stationaryPlatform != null)
        {
        }

        stationaryPlatform = levelPlatforms[currentPlatformIndex++];
        movingPlatform = levelPlatforms[currentPlatformIndex];

        movingPlatform.AddComponent<MovingPlatform>();
        movingPlatform.SetActive(true);
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
        using var evt = PlatformEvent.Get(stationaryPlatform, movingPlatform).SendGlobal((int)PlatformEventType.Split);
    }
}