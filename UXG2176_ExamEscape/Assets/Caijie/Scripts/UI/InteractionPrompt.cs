using System;
using UnityEngine;

namespace Caijie
{
    // Allow PlayerInteractor to put a prompt on the HUD
    // basically PlayerInteractor will call Show or HIde, the HUD will subscribe to Changed
    // this
    public static class InteractionPrompt
    {
        // HUD listen to this, send null to hide the prompt
        // this is a list of methods waiting to be called, and each one receives a Sprite (the prompt picture).
        // event means other scripts can only add or remove
        public static event Action<Sprite> Changed;

        public static void Show(Sprite image) => Changed?.Invoke(image);
        public static void Hide() => Changed?.Invoke(null);
    }
}