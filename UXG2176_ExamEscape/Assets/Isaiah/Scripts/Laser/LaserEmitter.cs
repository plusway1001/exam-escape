using System.Collections.Generic;
using UnityEngine;

namespace Isaiah
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserEmitter : MonoBehaviour
    {
        [Header("Beam")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private int maxBounces = 10;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private bool startOn = true;

        private const float SurfaceOffset = 0.01f;

        private LineRenderer line;
        private Vector3[] points;
        private int pointCount;

        private readonly HashSet<LaserReceiver> litThisFrame = new HashSet<LaserReceiver>();
        private readonly HashSet<LaserReceiver> litLastFrame = new HashSet<LaserReceiver>();

        public bool IsOn { get; private set; }

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.useWorldSpace = true;

            if (firePoint == null) firePoint = transform;

            points = new Vector3[maxBounces + 2];

            IsOn = startOn;
        }

        private void LateUpdate()
        {
            litThisFrame.Clear();
            pointCount = 0;

            if (IsOn)
                TraceBeam();

            line.positionCount = pointCount;
            line.SetPositions(points);

            UpdateReceivers();
        }

        private void TraceBeam()
        {
            Physics.SyncTransforms();

            Vector3 origin = firePoint.position;
            Vector3 direction = firePoint.forward;
            float remaining = maxDistance;

            AddPoint(origin);

            for (int bounce = 0; bounce <= maxBounces; bounce++)
            {
                if (!Physics.Raycast(origin, direction, out RaycastHit hit, remaining, hitMask, QueryTriggerInteraction.Ignore))
                {
                    AddPoint(origin + direction * remaining);
                    return;
                }

                AddPoint(hit.point);
                remaining -= hit.distance;

                LaserReceiver receiver = hit.collider.GetComponentInParent<LaserReceiver>();
                if (receiver != null)
                {
                    litThisFrame.Add(receiver);
                    return;
                }

                LaserMirror mirror = hit.collider.GetComponentInParent<LaserMirror>();
                if (mirror == null || !mirror.Reflects(hit.normal))
                    return;

                direction = Vector3.Reflect(direction, hit.normal);
                origin = hit.point + direction * SurfaceOffset;
            }
        }

        private void AddPoint(Vector3 p)
        {
            if (pointCount < points.Length)
                points[pointCount++] = p;
        }

        private void UpdateReceivers()
        {
            foreach (LaserReceiver r in litThisFrame)
                if (!litLastFrame.Contains(r))
                    r.AddBeam();

            foreach (LaserReceiver r in litLastFrame)
                if (r != null && !litThisFrame.Contains(r))
                    r.RemoveBeam();

            litLastFrame.Clear();
            litLastFrame.UnionWith(litThisFrame);
        }

        public void SetOn(bool on) => IsOn = on;
        public void Toggle() => IsOn = !IsOn;

        private void OnDisable()
        {
            foreach (LaserReceiver r in litLastFrame)
                if (r != null) r.RemoveBeam();

            litLastFrame.Clear();
            if (line != null) line.positionCount = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Transform t = firePoint != null ? firePoint : transform;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(t.position, t.forward * 2f);
        }
    }
}
