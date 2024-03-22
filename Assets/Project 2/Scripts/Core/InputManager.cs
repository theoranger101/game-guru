using Events;
using UnityEngine;

namespace Core
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private bool m_InputEnabled = true;

        private void OnEnable()
        {
            GEM.AddListener<InputEvent>(OnToggleInput, channel: (int)InputEventType.ToggleEnabled);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<InputEvent>(OnToggleInput, channel: (int)InputEventType.ToggleEnabled);
        }

        private void OnToggleInput(InputEvent evt)
        {
            m_InputEnabled = evt.Enabled;
        }

        private void Update()
        {
            if (!m_InputEnabled)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                using var evt = InputEvent.Get().SendGlobal((int)InputEventType.Tap);
            }
        }
    }
}