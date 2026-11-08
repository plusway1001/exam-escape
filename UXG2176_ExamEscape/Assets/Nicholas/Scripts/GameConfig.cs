using UnityEngine;
using UnityEngine.InputSystem;

namespace Nicholas
{
    [CreateAssetMenu(menuName = "Nicholas/Config")]
    public class GameConfig : ScriptableObject
    {
        public float aimRange = 8f;
        public float interactRange = 3f;
        public float swapRadius = 5f;

        public LayerMask blockerMask = ~0;
        public LayerMask interactMask = ~0;

        public Key interactKey = Key.E;

        public Color inRangeColor = Color.green;
        public Color hoverColor = Color.yellow;
        public Color selectedColor = Color.white;

        public string swapPrompt = "[Click] Swap";
        public string actionFormat = "<size=70%><color=#{0}>{1}</color></size>";
        public Color swapPromptColor = new Color(0.33f, 1f, 0.33f);
        public Color availablePromptColor = Color.white;
        public Color blockedPromptColor = new Color(1f, 0.4f, 0.4f);
    }
}