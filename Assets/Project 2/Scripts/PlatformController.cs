using System;
using System.Collections.Generic;
using Project_2.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlatformController : MonoBehaviour
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

    [Header("Tolerance Info")]
    public float PerfectHitTolerance = 0.5f;

    public float FailTolerance = 1f;

    private float lmp_Stationary;
    private float rmp_Stationary;

    private float lmp_Moving;
    private float rmp_Moving;

    private void Awake()
    {
        CreatePlatformPool();
        CreateLevel(5);
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
        OnSplitPlatform();
    }

    [Button]
    private void GetStationaryPlatformInfo()
    {
        var collider1 = stationaryPlatform.GetComponent<BoxCollider>();

        var leftMostPoint_Stationary = collider1.bounds.min.x;
        var rightMostPoint_Stationary = collider1.bounds.max.x;

        Debug.Log(leftMostPoint_Stationary + " " + rightMostPoint_Stationary);

        var collider2 = movingPlatform.GetComponent<BoxCollider>();

        var leftMostPoint_Moving = collider2.bounds.min.x;
        var rightMostPoint_Moving = collider2.bounds.max.x;

        Debug.Log(leftMostPoint_Moving + " " + rightMostPoint_Moving);

        var cutOffRight =
            rightMostPoint_Moving.IsWithin(leftMostPoint_Stationary, rightMostPoint_Stationary, PerfectHitTolerance);
        var cutOffLeft =
            leftMostPoint_Moving.IsWithin(leftMostPoint_Stationary, rightMostPoint_Stationary, PerfectHitTolerance);

        Debug.Log(cutOffRight + " " + cutOffLeft);

        var cutOff = 0f;

        if (!cutOffRight && !cutOffLeft)
        {
            Debug.Log("Fail");
        }
        else if (!cutOffRight)
        {
            cutOff = Mathf.Abs(Mathf.Abs(rightMostPoint_Stationary) - Mathf.Abs(rightMostPoint_Moving));

            Debug.Log(cutOff);

            var center_CutOff = rightMostPoint_Stationary + (cutOff / 2f);

            var cutOffObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cutOffObj.transform.position = new Vector3(center_CutOff, movingPlatform.transform.position.y,
                movingPlatform.transform.position.z);
            cutOffObj.transform.localScale = new Vector3(cutOff, 1f, 1f);

            var remaining = Mathf.Abs(rightMostPoint_Stationary - leftMostPoint_Moving);
            var center_Remaining = rightMostPoint_Stationary - remaining / 2f;

            Debug.Log(remaining);

            var remainingObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            remainingObj.transform.position = new Vector3(center_Remaining, movingPlatform.transform.position.y,
                movingPlatform.transform.position.z);
            remainingObj.transform.localScale = new Vector3(remaining, 1f, 1f);
        }
        else if (!cutOffLeft)
        {
            cutOff = Mathf.Abs(Mathf.Abs(leftMostPoint_Stationary) - Mathf.Abs(leftMostPoint_Moving));

            Debug.Log(cutOff);

            var center_CutOff = leftMostPoint_Stationary - (cutOff / 2f);

            var cutOffObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cutOffObj.transform.position = new Vector3(center_CutOff, movingPlatform.transform.position.y,
                movingPlatform.transform.position.z);
            cutOffObj.transform.localScale = new Vector3(cutOff, 1f, 1f);

            // yes it can be calculated more easily if we use the object scale to calculate the remaining part
            // but i wanted to keep the theme of using the right-most/left-most points :)
            var remaining = Mathf.Abs(rightMostPoint_Moving - leftMostPoint_Stationary);
            var center_Remaining = leftMostPoint_Stationary + remaining / 2f;

            Debug.Log(remaining);

            var remainingObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            remainingObj.transform.position = new Vector3(center_Remaining, movingPlatform.transform.position.y,
                movingPlatform.transform.position.z);
            remainingObj.transform.localScale = new Vector3(remaining, 1f, 1f);
        }
        else
        {
            Debug.Log("Perfect hit");
        }
    }

    // LMP -> Left-most point, RMP -> Right-most point
    private void OnSplitPlatform()
    {
        var bounds_Stationary = stationaryPlatform.GetComponent<BoxCollider>().bounds;

        lmp_Stationary = bounds_Stationary.min.x;
        rmp_Stationary = bounds_Stationary.max.x;

        Debug.Log(lmp_Stationary + " " + rmp_Stationary);

        var bounds_Moving = movingPlatform.GetComponent<BoxCollider>().bounds;

        lmp_Moving = bounds_Moving.min.x;
        rmp_Moving = bounds_Moving.max.x;

        Debug.Log(lmp_Moving + " " + rmp_Moving);

        var outOfBoundsWithPerfectHitTolerance_Right =
            !rmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, PerfectHitTolerance);
        var outOfBoundsWithPerfectHitTolerance_Left =
            !lmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, PerfectHitTolerance);

        var outOfBoundsWithFailTolerance_Right = !rmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, FailTolerance);
        var outOfBoundsWithFailTolerance_Left = !lmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, FailTolerance);

        if ((outOfBoundsWithPerfectHitTolerance_Right && outOfBoundsWithFailTolerance_Left) ||
            (outOfBoundsWithPerfectHitTolerance_Left && outOfBoundsWithFailTolerance_Right))
        {
            Debug.Log("Fail");
        }
        else if (outOfBoundsWithPerfectHitTolerance_Right)
        {
            Debug.Log("Cutoff from right");
            OnCutOffRight();
        }
        else if (outOfBoundsWithPerfectHitTolerance_Left)
        {
            Debug.Log("Cutoff from left");
            OnCutOffLeft();
        }
        else
        {
            Debug.Log("Perfect hit");
        }
    }

    private void OnCutOffRight()
    {
        Destroy(movingPlatform.GetComponent<MovingPlatform>());
        Destroy(movingPlatform.GetComponent<Rigidbody>());

        var movingTransform = movingPlatform.transform;
        var movingPosition = movingTransform.position;
        var movingScale = movingTransform.localScale;

        var cutOff = Mathf.Abs(Mathf.Abs(rmp_Stationary) - Mathf.Abs(rmp_Moving));

        Debug.Log(cutOff);

        var center_CutOff = rmp_Stationary + (cutOff / 2f);

        var cutOffObj = GetPlatformFromPool();
        var cutOffTransform = cutOffObj.transform;

        cutOffTransform.position = new Vector3(center_CutOff, movingPosition.y, movingPosition.z);
        cutOffTransform.localScale = new Vector3(cutOff, movingScale.y, movingScale.z);

        var cutOffRB = cutOffObj.GetComponent<Rigidbody>();
        cutOffRB.useGravity = true;

        var remaining = Mathf.Abs(rmp_Stationary - lmp_Moving);
        var center_Remaining = rmp_Stationary - remaining / 2f;

        Debug.Log(remaining);

        // var remainingObj = GetPlatformFromPool();
        // var remainingTransform = remainingObj.transform;

        movingTransform.position = new Vector3(center_Remaining, movingPosition.y, movingPosition.z);
        movingTransform.localScale = new Vector3(remaining, movingScale.y, movingScale.z);

        UpdateMovingPlatform();
    }

    private void OnCutOffLeft()
    {
        Destroy(movingPlatform.GetComponent<MovingPlatform>());

        var movingTransform = movingPlatform.transform;
        var movingPosition = movingTransform.position;
        var movingScale = movingTransform.localScale;

        var cutOff = Mathf.Abs(Mathf.Abs(lmp_Stationary) - Mathf.Abs(lmp_Moving));

        Debug.Log(cutOff);

        var center_CutOff = lmp_Stationary - (cutOff / 2f);

        var cutOffObj = GetPlatformFromPool();
        var cutOffTransform = cutOffObj.transform;

        cutOffTransform.position = new Vector3(center_CutOff, movingPosition.y, movingPosition.z);
        cutOffTransform.localScale = new Vector3(cutOff, movingScale.y, movingScale.z);

        // yes it can be calculated more easily if we use the object scale to calculate the remaining part
        // but i wanted to keep the theme of using the right-most/left-most points :)
        var remaining = Mathf.Abs(rmp_Moving - lmp_Stationary);
        var center_Remaining = lmp_Stationary + remaining / 2f;

        Debug.Log(remaining);

        // var remainingObj = GetPlatformFromPool();
        // var remainingTransform = remainingObj.transform;

        movingTransform.position = new Vector3(center_Remaining, movingPosition.y, movingPosition.z);
        movingTransform.localScale = new Vector3(remaining, movingScale.y, movingScale.z);

        UpdateMovingPlatform();
    }
}