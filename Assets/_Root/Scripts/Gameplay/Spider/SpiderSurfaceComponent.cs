using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderSurfaceComponent
    {
        private const float PENDING_SURFACE_MAX_NORMAL_ANGLE = 20f;
        private const int DOWN_NORMAL_SAMPLE_COUNT = 5;
        private const float DOWN_NORMAL_SAMPLE_RADIUS_FACTOR = 0.6f;
        private const float DOWN_NORMAL_PRIMARY_WEIGHT = 2f;
        private const float DOWN_NORMAL_DISTANCE_TOLERANCE_FACTOR = 1.25f;
        private const int OVERLAP_BUFFER_SIZE = 8;

        private readonly SpiderConfig config;
        private readonly Collider[] overlapBuffer = new Collider[OVERLAP_BUFFER_SIZE];
        private SpiderSurfaceHit pendingForwardHit;
        private int pendingForwardHitFrames;

        public SpiderSurfaceComponent(SpiderConfig config)
        {
            this.config = config;
        }

        public SpiderSurfaceState Sample(SpiderPlayerController controller)
        {
            Vector3 localUp = ResolveLocalUp(controller);
            Vector3 probeOrigin = ResolveProbeOrigin(controller);

            bool hasForwardHit = TrySphereCast(
                probeOrigin,
                controller.transform.forward,
                config.ForwardProbeRadius,
                config.ForwardProbeDistance,
                controller.BodyCollider,
                out SpiderSurfaceHit forwardHit);
            bool hasDownHit = TrySphereCast(
                probeOrigin,
                -localUp,
                config.DownProbeRadius,
                config.DownProbeDistance,
                controller.BodyCollider,
                out SpiderSurfaceHit downHit);

            if (hasDownHit)
            {
                downHit = RefineDownHit(controller, probeOrigin, localUp, downHit);
            }

            if (ShouldPreferForwardHit(hasForwardHit, forwardHit, hasDownHit, downHit))
            {
                return BuildState(forwardHit);
            }

            if (hasDownHit)
            {
                return BuildState(downHit);
            }

            if (hasForwardHit)
            {
                return BuildState(forwardHit);
            }

            if (TryOverlapHit(controller, probeOrigin, localUp, out SpiderSurfaceHit overlapHit))
            {
                return BuildState(overlapHit);
            }

            return BuildNoSurfaceState(probeOrigin, localUp);
        }

        private Vector3 ResolveLocalUp(SpiderPlayerController controller)
        {
            Vector3 localUp = controller.CurrentReferenceUp;

            if (localUp.sqrMagnitude <= Mathf.Epsilon)
            {
                return controller.transform.up;
            }

            return localUp.normalized;
        }

        private Vector3 ResolveProbeOrigin(SpiderPlayerController controller)
        {
            return controller.ProbeOrigin != null
                ? controller.ProbeOrigin.position
                : controller.transform.position;
        }

        private bool TrySphereCast(
            Vector3 origin,
            Vector3 direction,
            float radius,
            float distance,
            Collider ignoredCollider,
            out SpiderSurfaceHit surfaceHit)
        {
            surfaceHit = default;

            if (direction.sqrMagnitude <= Mathf.Epsilon || radius <= 0f || distance <= 0f)
            {
                return false;
            }

            if (!Physics.SphereCast(
                    origin,
                    radius,
                    direction.normalized,
                    out RaycastHit hit,
                    distance,
                    config.TraversableSurfaceMask,
                    QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            if (hit.collider == ignoredCollider)
            {
                return false;
            }

            surfaceHit = new SpiderSurfaceHit(hit.collider, hit.point, hit.normal.normalized, hit.distance);
            return true;
        }

        private bool TryOverlapHit(SpiderPlayerController controller, Vector3 probeOrigin, Vector3 localUp, out SpiderSurfaceHit surfaceHit)
        {
            surfaceHit = default;

            int overlapCount = Physics.OverlapSphereNonAlloc(
                probeOrigin,
                Mathf.Max(config.DownProbeRadius, config.ForwardProbeRadius),
                overlapBuffer,
                config.TraversableSurfaceMask,
                QueryTriggerInteraction.Ignore);

            if (overlapCount == 0)
            {
                return false;
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

                Vector3 candidatePoint = GetClosestPoint(candidateCollider, probeOrigin);
                float candidateDistance = Vector3.Distance(probeOrigin, candidatePoint);

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
                return false;
            }

            Vector3 contactNormal = closestDistance > 0f
                ? (probeOrigin - closestPoint).normalized
                : localUp;
            surfaceHit = new SpiderSurfaceHit(closestCollider, closestPoint, contactNormal, closestDistance);
            return true;
        }

        private bool ShouldPreferForwardHit(
            bool hasForwardHit,
            SpiderSurfaceHit forwardHit,
            bool hasDownHit,
            SpiderSurfaceHit downHit)
        {
            if (!hasForwardHit)
            {
                ResetPendingForwardHit();
                return false;
            }

            if (!hasDownHit)
            {
                ResetPendingForwardHit();
                return true;
            }

            float surfaceAngle = Vector3.Angle(downHit.Normal, forwardHit.Normal);

            if (surfaceAngle < config.ForwardProbePriorityAngle)
            {
                ResetPendingForwardHit();
                return false;
            }

            if (!IsSamePendingForwardSurface(forwardHit))
            {
                pendingForwardHit = forwardHit;
                pendingForwardHitFrames = 1;
                return false;
            }

            pendingForwardHit = forwardHit;
            pendingForwardHitFrames++;
            return pendingForwardHitFrames >= config.ForwardProbeConfirmFrames;
        }

        private bool IsSamePendingForwardSurface(SpiderSurfaceHit forwardHit)
        {
            if (pendingForwardHitFrames == 0)
            {
                return false;
            }

            if (pendingForwardHit.Collider != forwardHit.Collider)
            {
                return false;
            }

            return Vector3.Angle(pendingForwardHit.Normal, forwardHit.Normal) <= PENDING_SURFACE_MAX_NORMAL_ANGLE;
        }

        private SpiderSurfaceHit RefineDownHit(
            SpiderPlayerController controller,
            Vector3 probeOrigin,
            Vector3 localUp,
            SpiderSurfaceHit downHit)
        {
            Vector3 tangentForward = Vector3.ProjectOnPlane(controller.transform.forward, localUp);

            if (tangentForward.sqrMagnitude <= Mathf.Epsilon)
            {
                tangentForward = Vector3.ProjectOnPlane(controller.transform.right, localUp);
            }

            if (tangentForward.sqrMagnitude <= Mathf.Epsilon)
            {
                tangentForward = Vector3.ProjectOnPlane(Vector3.forward, localUp);
            }

            tangentForward.Normalize();

            Vector3 tangentRight = Vector3.Cross(localUp, tangentForward);

            if (tangentRight.sqrMagnitude <= Mathf.Epsilon)
            {
                tangentRight = Vector3.ProjectOnPlane(Vector3.right, localUp);
            }

            tangentRight.Normalize();

            float sampleRadius = config.DownProbeRadius * DOWN_NORMAL_SAMPLE_RADIUS_FACTOR;

            if (sampleRadius <= Mathf.Epsilon)
            {
                return downHit;
            }

            Vector3 normalSum = downHit.Normal * DOWN_NORMAL_PRIMARY_WEIGHT;
            int hitCount = 1;
            float rayDistance = config.DownProbeDistance + config.DownProbeRadius;
            float maxAcceptedDistance = downHit.Distance + (config.DownProbeRadius * DOWN_NORMAL_DISTANCE_TOLERANCE_FACTOR);

            for (int sampleIndex = 0; sampleIndex < DOWN_NORMAL_SAMPLE_COUNT; sampleIndex++)
            {
                Vector3 sampleOffset = ResolveDownNormalSampleOffset(sampleIndex, tangentForward, tangentRight, sampleRadius);
                Vector3 sampleOrigin = probeOrigin + sampleOffset;

                if (!Physics.Raycast(
                        sampleOrigin,
                        -localUp,
                        out RaycastHit hit,
                        rayDistance,
                        config.TraversableSurfaceMask,
                        QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                if (hit.collider == controller.BodyCollider)
                {
                    continue;
                }

                Vector3 sampleNormal = hit.normal.normalized;

                if (hit.distance > maxAcceptedDistance || Vector3.Dot(localUp, sampleNormal) <= 0f)
                {
                    continue;
                }

                normalSum += sampleNormal;
                hitCount++;
            }

            if (hitCount <= 1)
            {
                return downHit;
            }

            Vector3 refinedNormal = normalSum.normalized;
            return refinedNormal == Vector3.zero
                ? downHit
                : new SpiderSurfaceHit(downHit.Collider, downHit.Point, refinedNormal, downHit.Distance);
        }

        private Vector3 ResolveDownNormalSampleOffset(int sampleIndex, Vector3 tangentForward, Vector3 tangentRight, float sampleRadius)
        {
            switch (sampleIndex)
            {
                case 1:
                    return tangentForward * sampleRadius;
                case 2:
                    return -tangentForward * sampleRadius;
                case 3:
                    return tangentRight * sampleRadius;
                case 4:
                    return -tangentRight * sampleRadius;
                default:
                    return Vector3.zero;
            }
        }

        private void ResetPendingForwardHit()
        {
            pendingForwardHit = default;
            pendingForwardHitFrames = 0;
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

        private SpiderSurfaceState BuildState(SpiderSurfaceHit hit)
        {
            return BuildState(hit, hit.Normal);
        }

        private SpiderSurfaceState BuildState(SpiderSurfaceHit hit, Vector3 surfaceNormal)
        {
            return new SpiderSurfaceState(
                true,
                surfaceNormal != Vector3.zero,
                surfaceNormal,
                hit.Point,
                hit.Distance,
                hit,
                1);
        }

        private SpiderSurfaceState BuildNoSurfaceState(Vector3 probeOrigin, Vector3 localUp)
        {
            return new SpiderSurfaceState(
                false,
                false,
                localUp,
                probeOrigin,
                config.DownProbeDistance,
                default,
                0);
        }
    }
}
