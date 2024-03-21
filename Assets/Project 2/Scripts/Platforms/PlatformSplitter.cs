using System;
using Events;
using Platforms.PlatformStates;
using UnityEngine;

namespace Platforms
{
    public class PlatformSplitter : MonoBehaviour
    {
        private Platform m_MovingPlatform;
        private Platform m_StationaryPlatform;

        [Header("Tolerance Info")]
        public float PerfectHitTolerance = 0.5f;

        public float FailTolerance = 1f;

        private float lmp_Stationary;
        private float rmp_Stationary;

        private float lmp_Moving;
        private float rmp_Moving;

        private void OnEnable()
        {
            GEM.AddListener<PlatformEvent>(OnSplitPlatform, channel: (int)PlatformEventType.Split);
        }

        // LMP -> Left-most point, RMP -> Right-most point
        private void OnSplitPlatform(PlatformEvent evt)
        {
            m_StationaryPlatform = evt.Platform1;
            m_MovingPlatform = evt.Platform2;

            m_MovingPlatform.CurrentStateType = Platform.PlatformStateType.Stationary;
            //
            // Destroy(m_MovingPlatform.GetComponent<MovingPlatform>());
            // Destroy(m_MovingPlatform.GetComponent<Rigidbody>());
            
            var bounds_Stationary = m_StationaryPlatform.GetComponent<BoxCollider>().bounds;

            lmp_Stationary = bounds_Stationary.min.x;
            rmp_Stationary = bounds_Stationary.max.x;

            Debug.Log(lmp_Stationary + " " + rmp_Stationary);
            
            var bounds_Moving = m_MovingPlatform.GetComponent<BoxCollider>().bounds;

            lmp_Moving = bounds_Moving.min.x;
            rmp_Moving = bounds_Moving.max.x;
            
            Debug.Log(lmp_Moving + " " + rmp_Moving);

            var outOfBounds_Right = !rmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, PerfectHitTolerance);
            var outOfBounds_Left = !lmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, PerfectHitTolerance);
            
            Debug.Log(outOfBounds_Right + " " + outOfBounds_Left);

            if (outOfBounds_Right && outOfBounds_Left)
            {
                Debug.Log("Fail");
            }
            else if (outOfBounds_Right)
            {
                Debug.Log("Cutoff from right");
                OnCutOffRight();
            }
            else if (outOfBounds_Left)
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
            var cutOff = Mathf.Abs(Mathf.Abs(rmp_Stationary) - Mathf.Abs(rmp_Moving));
            var cutOffCenter = rmp_Stationary + (cutOff / 2f);

            // yes it can be calculated more easily if we use the object scale to calculate the remaining part
            // but i wanted to keep the theme of using the right-most/left-most points :)
            var remaining = Mathf.Abs(rmp_Stationary - lmp_Moving);
            var remainingCenter = rmp_Stationary - remaining / 2f;
            
            CutOffAndSetRemaining(cutOff, cutOffCenter, remaining, remainingCenter);
        }

        private void OnCutOffLeft()
        {
            var cutOff = Mathf.Abs(Mathf.Abs(lmp_Stationary) - Mathf.Abs(lmp_Moving));
            var cutOffCenter = lmp_Stationary - (cutOff / 2f);

            // yes it can be calculated more easily if we use the object scale to calculate the remaining part
            // but i wanted to keep the theme of using the right-most/left-most points :)
            var remaining = Mathf.Abs(rmp_Moving - lmp_Stationary);
            var remainingCenter = lmp_Stationary + remaining / 2f;

            CutOffAndSetRemaining(cutOff, cutOffCenter, remaining, remainingCenter);
        }

        private void CutOffAndSetRemaining(float cutOff, float cutOffCenter, float remaining, float remainingCenter)
        {
            if (remaining < FailTolerance)
            {
                Debug.Log("Fail");
            }
            
            var movingTransform = m_MovingPlatform.transform;
            var movingPosition = movingTransform.position;
            var movingScale = movingTransform.localScale;

            using var getPlatformEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.GetPooledPlatform);
            var cutOffObj = getPlatformEvt.Platform1;
            var cutOffTransform = cutOffObj.transform;

            cutOffObj.CurrentStateType = Platform.PlatformStateType.CutOff;
            // var cutOffRB = cutOffObj.AddComponent<Rigidbody>();

            cutOffTransform.position = new Vector3(cutOffCenter, movingPosition.y, movingPosition.z);
            cutOffTransform.localScale = new Vector3(cutOff, movingScale.y, movingScale.z);

            movingTransform.position = new Vector3(remainingCenter, movingPosition.y, movingPosition.z);
            movingTransform.localScale = new Vector3(remaining, movingScale.y, movingScale.z);

            using var updatePlatformsEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.UpdatePlatforms);
        }
    }
}