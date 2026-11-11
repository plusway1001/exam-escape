using System;

namespace Caijie
{
    // Allow PlayerInteractor to put a prompt on the HUD
    // basically PlayerInteractor will call Show or HIde, the HUD will subscribe to Changed
    // this
    public static class InteractionPrompt
    {
        // HUD listen to this, send null to hide the prompt
        // this is a list of methods waiting to be called, and each one receives a string.
        // event means other scripts can only add or remove
        public static event Action<string> Changed; 

        public static void Show(string text) => Changed?.Invoke(text);
        public static void Hide() => Changed?.Invoke(null);
    }
}