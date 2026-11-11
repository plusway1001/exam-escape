using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns random rubbish inside a box. It ALWAYS spawns one matching item for every entry
/// in the required sequence (so the puzzle is always solvable), plus a random number of extras
/// picked from the whole catalogue (including useless junk).
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Box covering the area to spawn in. Make it Is Trigger and keep its top below the ceiling.")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private Transform container;
    [Tooltip("Used when a TrashItemData has no prefab of its own (cube + Rigidbody + TrashItem).")]
    [SerializeField] private TrashItem fallbackPrefab;
    [SerializeField] private List<TrashItemData> catalogue = new();

    [Header("Amount")]
    [Tooltip("Random number of extra items on top of the required ones (min, max inclusive).")]
    [SerializeField] private Vector2Int extraItemRange = new(4, 9);

    [Header("Placement")]
    [SerializeField] private float clearRadius = 0.25f;
    [SerializeField] private int maxAttempts = 30;
    [Tooltip("Layers items can land on / must not overlap. Exclude the Player layer.")]
    [SerializeField] private LayerMask surfaceMask = ~0;

    [Header("Fallback (used when no clear spot is found)")]
    [Tooltip("Empty objects placed just above the floor. One is picked at random. Leave empty to use the area centre.")]
    [SerializeField] private Transform[] fallbackSpawnPoints;
    [Tooltip("Random sideways offset so several items don't spawn exactly on top of each other.")]
    [SerializeField] private float fallbackJitter = 0.3f;

    private readonly List<TrashItem> spawned = new();

    public void Spawn(IReadOnlyList<MaterialType> requiredSequence)
    {
        Clear();

        if (catalogue.Count == 0)
        {
            Debug.LogError("TrashSpawner: catalogue is empty.", this);
            return;
        }

        // Guaranteed items: one per required step.
        foreach (var material in requiredSequence)
        {
            var options = catalogue.FindAll(d => d.material == material);
            if (options.Count == 0)
            {
                Debug.LogError($"TrashSpawner: no catalogue item with material {material}.", this);
                continue;
            }
            SpawnItem(options[Random.Range(0, options.Count)]);
        }

        // Random extras (may be junk or more of the same).
        int extras = Random.Range(extraItemRange.x, extraItemRange.y + 1);
        for (int i = 0; i < extras; i++)
            SpawnItem(catalogue[Random.Range(0, catalogue.Count)]);
    }

    public void Clear()
    {
        foreach (var item in spawned)
            if (item != null) Destroy(item.gameObject);
        spawned.Clear();
    }

    private void SpawnItem(TrashItemData data)
    {
        TrashItem prefab = data.prefab != null ? data.prefab : fallbackPrefab;
        if (prefab == null)
        {
            Debug.LogError($"TrashSpawner: no prefab for {data.name} and no fallback prefab set.", this);
            return;
        }

        var rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        TrashItem item = Instantiate(prefab, FindSpawnPosition(), rotation);
        ParentUnderContainer(item.transform);
        item.Initialize(data);
        spawned.Add(item);
    }

    /// <summary>
    /// Parenting under a non-uniformly scaled object would stretch the item (spheres become ovals),
    /// so only parent when the container's scale is uniform.
    /// </summary>
    private void ParentUnderContainer(Transform item)
    {
        if (container == null) return;

        Vector3 s = container.lossyScale;
        bool uniform = Mathf.Abs(s.x - s.y) < 0.001f && Mathf.Abs(s.y - s.z) < 0.001f;

        if (uniform)
            item.SetParent(container, true);
        else
            Debug.LogWarning("TrashSpawner: container has a non-uniform scale, spawning items unparented to avoid stretching. Set the container scale to (1,1,1).", this);
    }

    private Vector3 FindSpawnPosition()
    {
        Bounds b = spawnArea.bounds;

        for (int i = 0; i < maxAttempts; i++)
        {
            var origin = new Vector3(
                Random.Range(b.min.x, b.max.x),
                b.max.y,
                Random.Range(b.min.z, b.max.z));

            // Drop a ray to find the floor (or a desk top) below this point.
            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, b.size.y,
                    surfaceMask, QueryTriggerInteraction.Ignore))
                continue;

            Vector3 point = hit.point + Vector3.up * (clearRadius + 0.05f);

            // Skip spots that are jammed against walls or other objects.
            if (Physics.CheckSphere(point, clearRadius, surfaceMask, QueryTriggerInteraction.Ignore))
                continue;

            return point;
        }

        return GetFallbackPosition(b);
    }

    private Vector3 GetFallbackPosition(Bounds area)
    {
        if (fallbackSpawnPoints == null || fallbackSpawnPoints.Length == 0)
        {
            Debug.LogWarning("TrashSpawner: no clear spot found and no fallback spawn points set, using the area centre.", this);
            return area.center;
        }

        Transform point = fallbackSpawnPoints[Random.Range(0, fallbackSpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * fallbackJitter;
        return point.position + new Vector3(offset.x, 0f, offset.y);
    }
}

/*using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns random rubbish inside a box. It ALWAYS spawns one matching item for every entry
/// in the required sequence (so the puzzle is always solvable), plus a random number of extras
/// picked from the whole catalogue (including useless junk).
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Box covering the area to spawn in. Make it Is Trigger and keep its top below the ceiling.")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private Transform container;
    [Tooltip("Used when a TrashItemData has no prefab of its own (cube + Rigidbody + TrashItem).")]
    [SerializeField] private TrashItem fallbackPrefab;
    [SerializeField] private List<TrashItemData> catalogue = new();

    [Header("Amount")]
    [Tooltip("Random number of extra items on top of the required ones (min, max inclusive).")]
    [SerializeField] private Vector2Int extraItemRange = new(4, 9);

    [Header("Placement")]
    [SerializeField] private float clearRadius = 0.25f;
    [SerializeField] private int maxAttempts = 30;
    [Tooltip("Layers items can land on / must not overlap. Exclude the Player layer.")]
    [SerializeField] private LayerMask surfaceMask = ~0;

    private readonly List<TrashItem> spawned = new();

    public void Spawn(IReadOnlyList<MaterialType> requiredSequence)
    {
        Clear();

        if (catalogue.Count == 0)
        {
            Debug.LogError("TrashSpawner: catalogue is empty.", this);
            return;
        }

        // Guaranteed items: one per required step.
        foreach (var material in requiredSequence)
        {
            var options = catalogue.FindAll(d => d.material == material);
            if (options.Count == 0)
            {
                Debug.LogError($"TrashSpawner: no catalogue item with material {material}.", this);
                continue;
            }
            SpawnItem(options[Random.Range(0, options.Count)]);
        }

        // Random extras (may be junk or more of the same).
        int extras = Random.Range(extraItemRange.x, extraItemRange.y + 1);
        for (int i = 0; i < extras; i++)
            SpawnItem(catalogue[Random.Range(0, catalogue.Count)]);
    }

    public void Clear()
    {
        foreach (var item in spawned)
            if (item != null) Destroy(item.gameObject);
        spawned.Clear();
    }

    private void SpawnItem(TrashItemData data)
    {
        TrashItem prefab = data.prefab != null ? data.prefab : fallbackPrefab;
        if (prefab == null)
        {
            Debug.LogError($"TrashSpawner: no prefab for {data.name} and no fallback prefab set.", this);
            return;
        }

        var rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        TrashItem item = Instantiate(prefab, FindSpawnPosition(), rotation, container);
        item.Initialize(data);
        spawned.Add(item);
    }

    private Vector3 FindSpawnPosition()
    {
        Bounds b = spawnArea.bounds;

        for (int i = 0; i < maxAttempts; i++)
        {
            var origin = new Vector3(
                Random.Range(b.min.x, b.max.x),
                b.max.y,
                Random.Range(b.min.z, b.max.z));

            // Drop a ray to find the floor (or a desk top) below this point.
            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, b.size.y,
                    surfaceMask, QueryTriggerInteraction.Ignore))
                continue;

            Vector3 point = hit.point + Vector3.up * (clearRadius + 0.05f);

            // Skip spots that are jammed against walls or other objects.
            if (Physics.CheckSphere(point, clearRadius, surfaceMask, QueryTriggerInteraction.Ignore))
                continue;

            return point;
        }

        Debug.LogWarning("TrashSpawner: could not find a clear spot, using the area centre.", this);
        return b.center;
    }
}*/
