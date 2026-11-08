using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nicholas
{
    public class Swappable : MonoBehaviour
    {
        public static readonly List<Swappable> All = new();

        private Renderer[] renderers;
        private Material[][] mats;
        private Color[][] originalColors;

        [SerializeField] private string displayName;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;

        public static event Action<Swappable, Swappable> Swapped;
        public static void NotifySwapped(Swappable a, Swappable b) => Swapped?.Invoke(a, b);

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            mats = new Material[renderers.Length][];
            originalColors = new Color[renderers.Length][];

            for (int i = 0; i < renderers.Length; i++)
            {
                mats[i] = renderers[i].materials;
                originalColors[i] = new Color[mats[i].Length];
                for (int j = 0; j < mats[i].Length; j++)
                {
                    originalColors[i][j] = mats[i][j].color;
                }
            }
        }

        private void OnEnable() => All.Add(this);
        private void OnDisable() => All.Remove(this);

        public void SetColor(Color? tint)
        {
            for (int i = 0; i < mats.Length; i++)
            {
                for (int j = 0; j < mats[i].Length; j++)
                {
                    mats[i][j].color = tint ?? originalColors[i][j];
                }
            }
        }

        public Bounds GetBounds()
        {
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                b.Encapsulate(renderers[i].bounds);
            }
            return b;
        }
    }
}