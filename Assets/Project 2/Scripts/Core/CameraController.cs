using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Events;
using Platforms;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    // i added this part last minute so i am not proud of it...
    public class CameraController : MonoBehaviour
    {
        public CinemachineVirtualCamera MainCamera;
        public CinemachineVirtualCamera DollyCamera;

        public int NumOfWaypoints = 10;
        public float RotationRadius = 5f;

        private List<Vector3> m_Waypoints = new List<Vector3>();

        [SerializeField]
        private Transform m_CharTransform;

        private GeneralSettings m_GeneralSettings;

        private void OnEnable()
        {
            m_GeneralSettings = GeneralSettings.Get();

            GEM.AddListener<PlatformEvent>(OnCheckSplit, channel: (int)PlatformEventType.CheckSplit);

            GEM.AddListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Success);
            GEM.AddListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Fail);

            GEM.AddListener<GameEvent>(OnDance, channel: (int)GameEventType.Dance);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<PlatformEvent>(OnCheckSplit, channel: (int)PlatformEventType.CheckSplit);

            GEM.RemoveListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Success);
            GEM.RemoveListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Fail);

            GEM.RemoveListener<GameEvent>(OnDance, channel: (int)GameEventType.Dance);
        }

        private void OnCheckSplit(PlatformEvent evt)
        {
            MainCamera.Follow = evt.Platform2.transform;
        }

        private void OnStartMovement(GameEvent evt)
        {
            MainCamera.Follow = m_CharTransform;
        }

        private void OnDance(GameEvent evt)
        {
            OnRotateAroundPlayer();
        }

        private void OnRotateAroundPlayer()
        {
            CalculateWaypoints();

            DollyCamera.LookAt = m_CharTransform;

            CinemachineTrackedDolly trackedDolly = DollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            CinemachineSmoothPath smoothPath = new GameObject("DollyTrack").AddComponent<CinemachineSmoothPath>();

            smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[m_Waypoints.Count];
            for (int i = 0; i < m_Waypoints.Count; i++)
            {
                smoothPath.m_Waypoints[i] = new CinemachineSmoothPath.Waypoint
                {
                    position = m_Waypoints[i]
                };
            }

            smoothPath.m_Looped = true;

            trackedDolly.m_Path = smoothPath;
            trackedDolly.m_PathPosition = 0;

            DollyCamera.gameObject.SetActive(true);

            DOTween.To(() => trackedDolly.m_PathPosition, x => trackedDolly.m_PathPosition = x, NumOfWaypoints,
                    m_GeneralSettings.CharacterDanceSequenceDuration)
                .OnComplete(() => DollyCamera.gameObject.SetActive(false));
        }

        private void CalculateWaypoints()
        {
            m_Waypoints.Clear();
            var charPos = m_CharTransform.position;

            for (var i = 0; i < NumOfWaypoints; i++)
            {
                var angle = i * (360f / NumOfWaypoints);
                var x = charPos.x + RotationRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                var z = charPos.z + RotationRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                m_Waypoints.Add(new Vector3(x, transform.position.y, z));
            }
        }
    }
}