using System;
using UnityEngine;
using UnityEngine.Events;

namespace Isaiah
{
    public class LaserReceiver : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Renderer indicator;
        [SerializeField] private Color offColor = Color.red;
        [SerializeField, ColorUsage(false, true)] private Color onColor = Color.green * 3f;

        [Header("Events")]
        public UnityEvent onPowered;
        public UnityEvent onUnpowered;

        public event Action<LaserReceiver, bool> PoweredChanged;

        private int beamCount;
        public bool IsPowered => beamCount > 0;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private void Start() => RefreshVisual();

        public void AddBeam()
        {
            beamCount++;
            if (beamCount == 1) Changed(true);
        }

        public void RemoveBeam()
        {
            if (beamCount == 0) return;
            beamCount--;
            if (beamCount == 0) Changed(false);
        }

        private void Changed(bool powered)
        {
            RefreshVisual();

            if (powered)
            {
                AudioManager.Instance.PlaySfxAt(AudioManager.Instance.sounds.laserPowered, transform.position);
                onPowered?.Invoke();
            }
            else
            {
                AudioManager.Instance.PlaySfxAt(AudioManager.Instance.sounds.laserUnpowered, transform.position);
                onUnpowered?.Invoke();
            }

            PoweredChanged?.Invoke(this, powered);
        }

        private void RefreshVisual()
        {
            if (indicator == null) return;

            Material mat = indicator.material;
            mat.SetColor(BaseColorId, IsPowered ? onColor : offColor);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor(EmissionColorId, IsPowered ? onColor : Color.black);
        }
    }
}
