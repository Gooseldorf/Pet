using UnityEngine;

namespace Pet.Gameplay
{
    public sealed class SpiderLegSurfaceSampler
    {
        private const int MAX_OVERLAPPING_COLLIDERS = 32;
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;
        private const float SCORE_COMPARISON_EPSILON = 0.0001f;

        private readonly Collider[] overlappingColliders = new Collider[MAX_OVERLAPPING_COLLIDERS];
        private readonly LayerMask traversableSurfaceMask;
        private readonly float searchRadius;

        public SpiderLegSurfaceSampler(SpiderConfig config)
        {
            traversableSurfaceMask = config.TraversableSurfaceMask;
            searchRadius = config.LegSearchRadius;
        }

        public float SearchRadius => searchRadius;

        public bool TryFindSupport(
            Vector3 desiredPosition,
            Vector3 rootPosition,
            float maxReach,
            float footOffset,
            out SpiderLegSurfaceContact support)
        {
            support = default;
            int overlappingCount = Physics.OverlapSphereNonAlloc(
                desiredPosition,
                searchRadius,
                overlappingColliders,
                traversableSurfaceMask,
                QueryTriggerInteraction.Ignore);
            float bestScore = float.MaxValue;

            for (int colliderIndex = 0; colliderIndex < overlappingCount; colliderIndex++)
            {
                Collider collider = overlappingColliders[colliderIndex];

                if (collider.attachedRigidbody != null ||
                    !TryCreateSupport(
                        collider,
                        desiredPosition,
                        rootPosition,
                        maxReach,
                        footOffset,
                        out SpiderLegSurfaceContact candidate))
                {
                    continue;
                }

                if (candidate.Score < bestScore - SCORE_COMPARISON_EPSILON ||
                    (Mathf.Abs(candidate.Score - bestScore) <= SCORE_COMPARISON_EPSILON &&
                     (support.Collider == null || candidate.Collider.GetEntityId() < support.Collider.GetEntityId())))
                {
                    support = candidate;
                    bestScore = candidate.Score;
                }
            }

            return support.Collider != null;
        }

        private bool TryCreateSupport(
            Collider collider,
            Vector3 desiredPosition,
            Vector3 rootPosition,
            float maxReach,
            float footOffset,
            out SpiderLegSurfaceContact support)
        {
            support = default;
            Vector3 closestPoint = collider.ClosestPoint(desiredPosition);
            Vector3 toSupport = closestPoint - desiredPosition;
            float desiredDistance = toSupport.magnitude;

            if (desiredDistance > MIN_VECTOR_SQR_MAGNITUDE)
            {
                return TryCreateSupportFromDesiredPosition(
                    collider,
                    desiredPosition,
                    desiredPosition,
                    rootPosition,
                    maxReach,
                    footOffset,
                    toSupport / desiredDistance,
                    desiredDistance + Physics.defaultContactOffset,
                    out support);
            }

            Vector3 rootToDesired = desiredPosition - rootPosition;
            float rootToDesiredDistance = rootToDesired.magnitude;

            if (rootToDesiredDistance <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                return false;
            }

            // ClosestPoint returns the input point from inside a collider. Cast from the hip to recover its visible face.
            return TryCreateSupportFromDesiredPosition(
                collider,
                desiredPosition,
                rootPosition,
                rootPosition,
                maxReach,
                footOffset,
                rootToDesired / rootToDesiredDistance,
                rootToDesiredDistance + Physics.defaultContactOffset,
                out support);
        }

        private bool TryCreateSupportFromDesiredPosition(
            Collider collider,
            Vector3 desiredPosition,
            Vector3 rayOrigin,
            Vector3 rootPosition,
            float maxReach,
            float footOffset,
            Vector3 rayDirection,
            float rayDistance,
            out SpiderLegSurfaceContact support)
        {
            support = default;

            if (!Physics.Raycast(
                    rayOrigin,
                    rayDirection,
                    out RaycastHit hit,
                    rayDistance,
                    traversableSurfaceMask,
                    QueryTriggerInteraction.Ignore) || hit.collider != collider)
            {
                return false;
            }

            Vector3 targetPosition = hit.point + hit.normal * footOffset;

            if (Vector3.Distance(rootPosition, targetPosition) > maxReach)
            {
                return false;
            }

            support = new SpiderLegSurfaceContact(
                collider,
                hit.point,
                hit.normal,
                Vector3.Distance(desiredPosition, hit.point));
            return true;
        }
    }
}
