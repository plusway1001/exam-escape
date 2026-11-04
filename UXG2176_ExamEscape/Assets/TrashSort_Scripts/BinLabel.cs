using TMPro;
using UnityEngine;

/// <summary>
/// Put this on the ROOT of a bin (the object holding the solid body collider).
/// 1) Writes the bin's purpose on a world-space text above it (taken from TrashBin.BinType,
///    so you never type it twice) and turns the text to face the player.
/// 2) Shows a description on the HUD while the player aims at the bin.
/// </summary>
public class BinLabel : MonoBehaviour, IInteractable
{
    [SerializeField] private TrashBin bin;
    [Tooltip("A world-space TextMeshPro text above the bin (3D Object > Text - TextMeshPro).")]
    [SerializeField] private TMP_Text worldText;

    [Header("Text ({0} = bin material)")]
    [SerializeField] private string labelFormat = "{0}\nRECYCLING";
    [SerializeField] private string descriptionFormat = "Only {0} items belong here. Drop or throw them into the opening.";
    [SerializeField] private Color labelColor = Color.white;

    [SerializeField] private bool faceCamera = true;

    private Transform cam;

    private void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;

        worldText.text = string.Format(labelFormat, bin.BinType.ToString().ToUpper());
        worldText.color = labelColor;
    }

    private void LateUpdate()
    {
        if (!faceCamera || cam == null || worldText == null) return;

        // Turn around the vertical axis only, so the text stays upright.
        Vector3 away = worldText.transform.position - cam.position;
        away.y = 0f;
        if (away.sqrMagnitude > 0.001f)
            worldText.transform.rotation = Quaternion.LookRotation(away);
    }

    // ---------- IInteractable ----------
    public string GetPrompt(PlayerInteractor interactor) =>
        $"<b>{bin.BinType} recycling bin</b>\n" + string.Format(descriptionFormat, bin.BinType);

    public bool CanInteract(PlayerInteractor interactor) => false;   // hover only

    public void Interact(PlayerInteractor interactor) { }
}
