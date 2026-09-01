using UnityEngine;

namespace Pet.Gameplay
{
    internal static class SpiderColliderMetrics
    {
        public static float CalculateWorldRadius(SphereCollider collider)
        {
            Vector3 scale = collider.transform.lossyScale;
            float largestScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            return collider.radius * largestScale;
        }
    }
}
