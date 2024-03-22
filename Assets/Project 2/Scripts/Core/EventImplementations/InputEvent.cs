using Events;

namespace Core
{
    public enum InputEventType
    {
        ToggleEnabled = 0,
        Tap = 1,
    }

    public class InputEvent : Event<InputEvent>
    {
        public bool Enabled;

        public static InputEvent Get()
        {
            return GetPooledInternal();
        }

        public static InputEvent Get(bool enabled)
        {
            var evt = GetPooledInternal();
            evt.Enabled = enabled;

            return evt;
        }
    }
}