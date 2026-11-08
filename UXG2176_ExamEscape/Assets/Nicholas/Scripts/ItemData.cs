using UnityEngine;

namespace Nicholas
{
    [CreateAssetMenu(menuName = "Nicholas/Item")]
    public class ItemData : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
    }
}
