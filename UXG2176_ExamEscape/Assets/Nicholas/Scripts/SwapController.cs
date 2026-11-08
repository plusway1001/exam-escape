using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nicholas
{
    [RequireComponent(typeof(PlayerSelect))]
    public class SwapController : MonoBehaviour
    {
        [SerializeField] private Material rangeSphereMaterial;

        private PlayerSelect select;
        private GameConfig Cfg => select.Config;

        private Swappable first, hovered;
        private readonly List<Swappable> inRange = new();
        private Transform rangeSphere;

        private void Awake()
        {
            select = GetComponent<PlayerSelect>();

            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(s.GetComponent<Collider>());
            s.GetComponent<Renderer>().sharedMaterial = rangeSphereMaterial;
            s.SetActive(false);
            rangeSphere = s.transform;
        }

        private void Update()
        {
            UpdateHover();
            if (!InputBlocker.InputBlocked && Mouse.current.leftButton.wasPressedThisFrame)
            {
                TrySelect();
            }
        }

        private void UpdateHover()
        {
            Swappable target = select.Swappable;
            if (target == hovered)
            {
                return;
            }

            Swappable prev = hovered;
            hovered = target;
            Refresh(prev);
            Refresh(hovered);
        }

        private void TrySelect()
        {
            Swappable target = select.Swappable;
            if (target == null)
            {
                return;
            }

            if (target == first)
            {
                ClearSelection();
                return;
            }

            if (first == null)
            {
                first = target;
                ShowRange(first);
                Refresh(first);
            }
            else if (InRange(first, target))
            {
                Swap(first, target);
                ClearSelection();
            }
        }

        private void Refresh(Swappable s)
        {
            if (s == null)
            {
                return;
            }

            Color? c = null;
            if (s == first)
            {
                c = Cfg.selectedColor;
            }
            else if (s == hovered)
            {
                c = Cfg.hoverColor;
            }
            else if (first != null && inRange.Contains(s))
            {
                c = Cfg.inRangeColor;
            }

            s.SetColor(c);
        }

        private bool InRange(Swappable a, Swappable b) => Vector3.Distance(a.GetBounds().center, b.GetBounds().center) <= Cfg.swapRadius;

        private void ShowRange(Swappable target)
        {
            rangeSphere.position = target.GetBounds().center;
            rangeSphere.localScale = Vector3.one * Cfg.swapRadius * 2f;
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

        private void ClearSelection()
        {
            Swappable old = first;
            first = null;

            rangeSphere.gameObject.SetActive(false);
            var previous = new List<Swappable>(inRange);
            inRange.Clear();
            foreach (Swappable s in previous) Refresh(s);
            Refresh(old);
        }

        private void Swap(Swappable a, Swappable b)
        {
            Bounds ba = a.GetBounds(), bb = b.GetBounds();
            Vector3 aBase = new Vector3(ba.center.x, ba.min.y, ba.center.z);
            Vector3 bBase = new Vector3(bb.center.x, bb.min.y, bb.center.z);
            a.transform.position += bBase - aBase;
            b.transform.position += aBase - bBase;

            Swappable.NotifySwapped(a, b);
        }
    }
}