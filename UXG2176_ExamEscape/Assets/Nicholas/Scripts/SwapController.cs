using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwapController : MonoBehaviour
{
    [SerializeField] private float range = 6f;
    [SerializeField] private LayerMask blockerMask = ~0;

    private Swappable first;
    private Swappable hovered;

    [SerializeField] private Material rangeSphereMaterial;

    private Transform rangeSphere;

    [SerializeField] private Color inRangeColor = Color.green;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.white;

    private readonly List<Swappable> inRange = new();

    private void Awake()
    {
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(s.GetComponent<Collider>());
        s.GetComponent<Renderer>().sharedMaterial = rangeSphereMaterial;
        s.transform.localScale = Vector3.one * range * 2f;
        s.SetActive(false);
        rangeSphere = s.transform;
    }

    private void Update()
    {
        UpdateHover();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Select();
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ClearSelection();
        }
    }

    private void Select()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, range, blockerMask))
        {
            return;
        }

        Swappable target = hit.collider.GetComponentInParent<Swappable>();

        if (target == null)
        {
            return;
        }

        if (target == first)
        {
            ClearSelection();
        }

        if (first == null)
        {
            first = target;
            ShowRange(first);
            Refresh(first);
        }
        else
        {
            if (!InRange(first, target))
            {
                return;
            }
            Swap(first, target);
            ClearSelection();
        }
    }

    private void Refresh(Swappable s)
    {
        if (s == null) return;

        Color? c = null;
        if (s == first)
        {
            c = selectedColor;
        }
        else if (s == hovered)
        {
            c = hoverColor;
        }
        else if (first != null && inRange.Contains(s))
        {
            c = inRangeColor;
        }

        s.SetColor(c);
    }

    private void Swap(Swappable a, Swappable b)
    {
        Bounds ba = a.GetBounds();
        Bounds bb = b.GetBounds();

        Vector3 aBase = new Vector3(ba.center.x, ba.min.y, ba.center.z);
        Vector3 bBase = new Vector3(bb.center.x, bb.min.y, bb.center.z);

        a.transform.position += bBase - aBase;
        b.transform.position += aBase - bBase;
    }

    private void ClearSelection()
    {
        Swappable old = first;
        first = null;
        HideRange();
        Refresh(old);
    }

    private void UpdateHover()
    {
        Swappable target = null;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, blockerMask, QueryTriggerInteraction.Ignore))
        {
            target = hit.collider.GetComponentInParent<Swappable>();
        }

        if (target == hovered)
        {
            return;
        }

        Swappable prev = hovered;
        hovered = target;
        Refresh(prev);
        Refresh(hovered);
    }

    private void ShowRange(Swappable target)
    {
        rangeSphere.position = target.GetBounds().center;
        rangeSphere.gameObject.SetActive(true);

        inRange.Clear();
        foreach (Swappable s in Swappable.All)
        {
            if (s != target && InRange(target, s))
            {
                inRange.Add(s);
            }
        }

        foreach (Swappable s in inRange)
        {
            Refresh(s);
        }
    }

    private void HideRange()
    {
        rangeSphere.gameObject.SetActive(false);

        var old = new List<Swappable>(inRange);
        inRange.Clear();
        foreach (Swappable s in old) Refresh(s);
    }

    private bool InRange(Swappable a, Swappable b)
    {
        return Vector3.Distance(a.GetBounds().center, b.GetBounds().center) <= range;
    }
}