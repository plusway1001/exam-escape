using UnityEngine;

namespace Isaiah
{
    public class LaserMirror : MonoBehaviour
    {
        [SerializeField] private bool doubleSided = true;

        public bool Reflects(Vector3 hitNormal)
        {
            if (doubleSided) return true;

            return Vector3.Dot(hitNormal, transform.forward) > 0.5f;
        }
    }
}
