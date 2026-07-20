using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderSurfaceComponent
    {
        private const int PROBE_COUNT = 5;
        private const int OVERLAP_BUFFER_SIZE = 8;

        private readonly SpiderConfig config;
        private readonly SpiderSurfaceHit[] hitsBuffer = new SpiderSurfaceHit[PROBE_COUNT];
        private readonly Collider[] overlapBuffer = new Collider[OVERLAP_BUFFER_SIZE];
        private readonly Vector3[] probeOffsets = new Vector3[PROBE_COUNT];

        public SpiderSurfaceComponent(SpiderConfig config)
        {
            this.config = config;
        }

        public SpiderSurfaceState Sample(SpiderPlayerController controller)
        {
            Vector3 localUp = controller.CurrentReferenceUp;
            Vector3 probeOrigin = controller.ProbeOrigin.position;
            int hitCount = CollectProbeHits(controller, probeOrigin, localUp);
            return BuildState(probeOrigin, localUp, hitCount);
        }

        private int CollectProbeHits(SpiderPlayerController controller, Vector3 probeOrigin, Vector3 localUp)
        {
            probeOffsets[0] = Vector3.zero;
            probeOffsets[1] = controller.transform.forward * config.ProbeOffset;
            probeOffsets[2] = -controller.transform.forward * config.ProbeOffset;
            probeOffsets[3] = controller.transform.right * config.ProbeOffset;
            probeOffsets[4] = -controller.transform.right * config.ProbeOffset;

            int hitCount = 0;
            Vector3 castDirection = -localUp;

            for (int i = 0; i < PROBE_COUNT; i++)
            {
                Vector3 castOrigin = probeOrigin + probeOffsets[i];

                if (Physics.SphereCast(
                        castOrigin,
                        config.ProbeRadius,
                        castDirection,
                        out RaycastHit hit,
                        config.ProbeDistance,
                        config.TraversableSurfaceMask,
                        QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider == controller.BodyCollider)
                    {
                        continue;
                    }

                    hitsBuffer[hitCount] = new SpiderSurfaceHit(hit.collider, hit.point, hit.normal, hit.distance);
                    hitCount++;
                    continue;
                }

                int overlapCount = Physics.OverlapSphereNonAlloc(
                    castOrigin,
                    config.ProbeRadius,
                    overlapBuffer,
                    config.TraversableSurfaceMask,
                    QueryTriggerInteraction.Ignore);

                if (overlapCount == 0)
                {
                    continue;
                }

                Collider closestCollider = null;
                Vector3 closestPoint = default;
                float closestDistance = float.MaxValue;

                for (int overlapIndex = 0; overlapIndex < overlapCount; overlapIndex++)
                {
                    Collider candidateCollider = overlapBuffer[overlapIndex];

                    if (candidateCollider == controller.BodyCollider)
                    {
                        continue;
                    }

                    Vector3 candidatePoint = GetClosestPoint(candidateCollider, castOrigin);
                    float candidateDistance = Vector3.Distance(castOrigin, candidatePoint);

                    if (candidateDistance >= closestDistance)
                    {
                        continue;
                    }

                    closestCollider = candidateCollider;
                    closestPoint = candidatePoint;
                    closestDistance = candidateDistance;
                }

                if (closestCollider == null)
                {
                    continue;
                }

                Vector3 contactNormal = closestDistance > 0f
                    ? (castOrigin - closestPoint).normalized
                    : localUp;

                hitsBuffer[hitCount] = new SpiderSurfaceHit(closestCollider, closestPoint, contactNormal, closestDistance);
                hitCount++;
            }

            return hitCount;
        }

        private Vector3 GetClosestPoint(Collider collider, Vector3 position)
        {
            if (collider is BoxCollider || collider is SphereCollider || collider is CapsuleCollider)
            {
                return collider.ClosestPoint(position);
            }

            if (collider is MeshCollider meshCollider && meshCollider.convex)
            {
                return collider.ClosestPoint(position);
            }

            return collider.bounds.ClosestPoint(position);
        }

        private SpiderSurfaceState BuildState(Vector3 probeOrigin, Vector3 localUp, int hitCount)
        {
            if (hitCount == 0)
            {
                return new SpiderSurfaceState(
                    false,
                    false,
                    localUp,
                    probeOrigin,
                    config.ProbeDistance,
                    default,
                    0);
            }

            SpiderSurfaceHit primaryHit = SelectPrimaryHit(hitCount);
            Vector3 aggregatedNormal = AggregateNormal(hitCount);

            return new SpiderSurfaceState(
                true,
                aggregatedNormal != Vector3.zero,
                aggregatedNormal,
                primaryHit.Point,
                primaryHit.Distance,
                primaryHit,
                hitCount);
        }

        private SpiderSurfaceHit SelectPrimaryHit(int hitCount)
        {
            SpiderSurfaceHit primaryHit = hitsBuffer[0];

            for (int i = 1; i < hitCount; i++)
            {
                if (hitsBuffer[i].Distance < primaryHit.Distance)
                {
                    primaryHit = hitsBuffer[i];
                }
            }

            return primaryHit;
        }

        private Vector3 AggregateNormal(int hitCount)
        {
            Vector3 normalSum = Vector3.zero;

            for (int i = 0; i < hitCount; i++)
            {
                SpiderSurfaceHit hit = hitsBuffer[i];
                float weight = 1f - Mathf.Clamp01(hit.Distance / Mathf.Max(config.ProbeDistance, 0.0001f));
                normalSum += hit.Normal * (weight + 0.01f);
            }

            return normalSum.normalized;
        }
    }
}
