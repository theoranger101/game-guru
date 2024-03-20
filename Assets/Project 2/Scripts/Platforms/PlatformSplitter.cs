using System;
using Events;
using UnityEngine;

namespace Project_2.Scripts.Platforms
{
    public class PlatformSplitter : MonoBehaviour
    {
        private GameObject m_MovingPlatform;
        private GameObject m_StationaryPlatform;

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
        public void OnSplitPlatform(PlatformEvent evt)
        {
            m_StationaryPlatform = evt.Platform1;
            m_MovingPlatform = evt.Platform2;

            var bounds_Stationary = m_StationaryPlatform.GetComponent<BoxCollider>().bounds;

            lmp_Stationary = bounds_Stationary.min.x;
            rmp_Stationary = bounds_Stationary.max.x;

            var bounds_Moving = m_MovingPlatform.GetComponent<BoxCollider>().bounds;

            lmp_Moving = bounds_Moving.min.x;
            rmp_Moving = bounds_Moving.max.x;

            var outOfBoundsWithPerfectHitTolerance_Right =
                !rmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, PerfectHitTolerance);
            var outOfBoundsWithPerfectHitTolerance_Left =
                !lmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, PerfectHitTolerance);

            var outOfBoundsWithFailTolerance_Right =
                !rmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, FailTolerance);
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
            Destroy(m_MovingPlatform.GetComponent<MovingPlatform>());
            Destroy(m_MovingPlatform.GetComponent<Rigidbody>());

            var movingTransform = m_MovingPlatform.transform;
            var movingPosition = movingTransform.position;
            var movingScale = movingTransform.localScale;

            using var getPlatformEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.GetPooledPlatform);
            var cutOffObj = getPlatformEvt.Platform1;
            var cutOffTransform = cutOffObj.transform;

            var cutOffRB = cutOffObj.GetComponent<Rigidbody>();
            cutOffRB.useGravity = true;

            cutOffTransform.position = new Vector3(cutOffCenter, movingPosition.y, movingPosition.z);
            cutOffTransform.localScale = new Vector3(cutOff, movingScale.y, movingScale.z);

            movingTransform.position = new Vector3(remainingCenter, movingPosition.y, movingPosition.z);
            movingTransform.localScale = new Vector3(remaining, movingScale.y, movingScale.z);

            // UpdateMovingPlatform();
            using var updatePlatformsEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.UpdatePlatforms);
        }
    }
}