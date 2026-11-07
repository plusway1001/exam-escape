using UnityEngine;

/// <summary>
/// One kind of rubbish (e.g. "Plastic Bottle"). Create assets via
/// Right-click > Create > Escape Room > Trash Item Data.
/// </summary>
[CreateAssetMenu(menuName = "Escape Room/Trash Item Data", fileName = "NewTrashItem")]
public class TrashItemData : ScriptableObject
{
    [Tooltip("Shown in the interaction prompt. Make it hint at the material, e.g. 'Newspaper'.")]
    public string displayName = "Trash";
    public MaterialType material;

    [Tooltip("Optional model prefab. Leave empty to use the spawner's fallback prefab.")]
    public TrashItem prefab;

    [Tooltip("Colours the item so different materials look different without art assets.")]
    public bool applyTint = true;
    public Color tint = Color.white;
}
