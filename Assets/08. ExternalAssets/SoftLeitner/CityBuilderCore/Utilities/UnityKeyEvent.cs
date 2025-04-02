using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    /// <summary>
    /// fires unity events when a key is pressed or released
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual">https://citybuilder.softleitner.com/manual</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_unity_key_event.html")]
    public class UnityKeyEvent : MonoBehaviour
    {
        public enum KeyModifier { None, Ctrl, Alt, Shift }

        [Tooltip("the key that fires the events on this behaviour on down and up")]
        public KeyCode Key;
        [Tooltip("key that needs to be down for the events to fire(ctrl, alt, ...)")]
        public KeyModifier Modifier;

        [Tooltip("gets fired when the defined key is pressed down")]
        public UnityEvent KeyDown;
        [Tooltip("gets fired when the defined key is released")]
        public UnityEvent KeyUp;

        private void Update()
        {
            if (Modifier != KeyModifier.None && !getModifier(Modifier))
                return;

            if (Input.GetKeyDown(Key))
                KeyDown?.Invoke();
            if (Input.GetKeyUp(Key))
                KeyUp?.Invoke();
        }

        private bool getModifier(KeyModifier modifier)
        {
            switch (modifier)
            {
                case KeyModifier.Ctrl:
                    return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                case KeyModifier.Alt:
                    return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                case KeyModifier.Shift:
                    return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }

            return false;
        }
    }
}