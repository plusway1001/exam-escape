using UnityEngine;

namespace Nicholas
{
    public static class InputBlocker
    {
        public static bool InputBlocked { get; private set; }
        public static void SetInputBlocked(bool blocked)
        {
            InputBlocked = blocked;
            Cursor.lockState = blocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = blocked;
        }
    }
}