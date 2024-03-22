using Core;
using Events;
using UnityEngine;

namespace Platforms
{
    public class PlatformSplitter : MonoBehaviour
    {
        private Platform m_MovingPlatform;
        private Platform m_StationaryPlatform;

        private float m_PerfectHitTolerance => m_GeneralSettings.PerfectHitTolerance;
        private float m_FailTolerance => m_GeneralSettings.FailTolerance;

        private float lmp_Stationary;
        private float rmp_Stationary;

        private float lmp_Moving;
        private float rmp_Moving;

        private GeneralSettings m_GeneralSettings;

        private void Awake()
        {
            m_GeneralSettings = GeneralSettings.Get();
        }

        private void OnEnable()
        {
            GEM.AddListener<PlatformEvent>(OnSplitPlatform, channel: (int)PlatformEventType.CheckSplit);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<PlatformEvent>(OnSplitPlatform, channel: (int)PlatformEventType.CheckSplit);
        }

        // LMP -> Left-most point, RMP -> Right-most point
        private void OnSplitPlatform(PlatformEvent evt)
        {
            m_StationaryPlatform = evt.Platform1;
            m_MovingPlatform = evt.Platform2;

            m_MovingPlatform.CurrentStateType = Platform.PlatformStateType.Stationary;

            var bounds_Stationary = m_StationaryPlatform.Collider.bounds;

            lmp_Stationary = bounds_Stationary.min.x;
            rmp_Stationary = bounds_Stationary.max.x;

            var bounds_Moving = m_MovingPlatform.Collider.bounds;

            lmp_Moving = bounds_Moving.min.x;
            rmp_Moving = bounds_Moving.max.x;

            var outOfBounds_Right = !rmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, m_PerfectHitTolerance);
            var outOfBounds_Left = !lmp_Moving.IsWithin(lmp_Stationary, rmp_Stationary, m_PerfectHitTolerance);

            if (outOfBounds_Right && outOfBounds_Left)
            {
                Debug.Log("Fail");
                OnFail();
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
                OnPerfectHit();
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
            using var splitEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.OnSplit);

            if (remaining < m_FailTolerance)
            {
                Debug.Log("Fail");
                OnFail();
                return;
            }

            var movingTransform = m_MovingPlatform.transform;
            var movingPosition = m_MovingPlatform.Position;
            var movingScale = movingTransform.localScale;

            var cutOffObj = PlatformExtensions.GetPlatformFromPool();
            var cutOffTransform = cutOffObj.transform;

            cutOffObj.Renderer.material = m_MovingPlatform.Renderer.material;

            cutOffObj.CurrentStateType = Platform.PlatformStateType.CutOff;

            cutOffTransform.position = new Vector3(cutOffCenter, movingPosition.y, movingPosition.z);
            cutOffTransform.localScale = new Vector3(cutOff, movingScale.y, movingScale.z);

            movingTransform.position = new Vector3(remainingCenter, movingPosition.y, movingPosition.z);
            movingTransform.localScale = new Vector3(remaining, movingScale.y, movingScale.z);

            using var updatePlatformsEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.UpdatePlatforms);
        }

        private void SnapPlatform()
        {
            m_MovingPlatform.transform.position = m_StationaryPlatform.Position.WithZ(m_MovingPlatform.Position.z);
            using var updatePlatformsEvt = PlatformEvent.Get().SendGlobal((int)PlatformEventType.UpdatePlatforms);
        }

        private void OnPerfectHit()
        {
            using var perfectHitEvt = PlatformEvent.Get().SendGlobal(channel: (int)PlatformEventType.PerfectHit);
            SnapPlatform();
        }

        private void OnFail()
        {
            m_MovingPlatform.CurrentStateType = Platform.PlatformStateType.CutOff;
            using var failEvt = PlatformEvent.Get().SendGlobal(channel: (int)PlatformEventType.Fall);
        }
    }
}