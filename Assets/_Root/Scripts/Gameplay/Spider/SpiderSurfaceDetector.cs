using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pet.Gameplay
{
    internal sealed class SpiderSurfaceDetector
    {
        private const int MAX_OVERLAPPING_COLLIDERS = 32;
        private const int INITIAL_CONTACT_CAPACITY = 64;
        private const float NORMAL_DEDUPLICATION_DOT = 0.999f;
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;
        private const float FACE_ALIGNMENT_DOT = 0.999f;
        private const float SCORE_COMPARISON_EPSILON = 0.0001f;
        private const float ATTACHMENT_CONFIRMATION_NORMAL_DOT = 0.95f;

        private static readonly Vector3[] probeDirections =
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
            new Vector3(1f, 1f, 0f).normalized,
            new Vector3(1f, -1f, 0f).normalized,
            new Vector3(-1f, 1f, 0f).normalized,
            new Vector3(-1f, -1f, 0f).normalized,
            new Vector3(1f, 0f, 1f).normalized,
            new Vector3(1f, 0f, -1f).normalized,
            new Vector3(-1f, 0f, 1f).normalized,
            new Vector3(-1f, 0f, -1f).normalized,
            new Vector3(0f, 1f, 1f).normalized,
            new Vector3(0f, 1f, -1f).normalized,
            new Vector3(0f, -1f, 1f).normalized,
            new Vector3(0f, -1f, -1f).normalized
        };

        private readonly SphereCollider bodyCollider;
        private readonly SpiderConfig config;
        private Collider[] overlappingColliders = new Collider[MAX_OVERLAPPING_COLLIDERS];
        private SpiderSurfaceContact[] contacts = new SpiderSurfaceContact[INITIAL_CONTACT_CAPACITY];
        private SpiderSurfaceContact[] selectedContacts = new SpiderSurfaceContact[INITIAL_CONTACT_CAPACITY];
        private Collider[] jumpOriginColliders = new Collider[INITIAL_CONTACT_CAPACITY];
        private bool[] processedContacts = new bool[INITIAL_CONTACT_CAPACITY];
        private BoxCollider[] cachedBoxColliders = new BoxCollider[MAX_OVERLAPPING_COLLIDERS];
        private SpiderBoxFace[] cachedBoxFaces = new SpiderBoxFace[MAX_OVERLAPPING_COLLIDERS * 6];

        private int contactCount;
        private int selectedContactCount;
        private int attachmentFrames;
        private int jumpOriginColliderCount;
        private int cachedBoxColliderCount;
        private Vector3 bodyCenter;
        private float bodyRadius;
        private Vector3 lastAttachedBodyPosition;
        private Quaternion lastAttachedBodyRotation;
        private Vector3 confirmationNormal;
        private bool ignoresJumpOrigin;
        private bool hasConfirmationCandidate;

        public SpiderSurfaceDetector(SphereCollider bodyCollider, SpiderConfig config)
        {
            this.bodyCollider = bodyCollider;
            this.config = config;
            State = new SpiderSurfaceState();
        }

        public SpiderSurfaceState State { get; }
        internal bool HasSample { get; private set; }
        internal Vector3 BodyCenter => bodyCenter;
        internal float SearchRadius => bodyRadius + config.SurfaceSearchDistance;
        internal int ContactCount => contactCount;
        internal int SelectedContactCount => selectedContactCount;
        public bool HasDetectedSurface { get; private set; }
        public Vector3 LastAttachedBodyPosition => lastAttachedBodyPosition;
        public Quaternion LastAttachedBodyRotation => lastAttachedBodyRotation;

        public void Sample()
        {
            bodyCenter = bodyCollider.transform.TransformPoint(bodyCollider.center);
            bodyRadius = SpiderColliderMetrics.CalculateWorldRadius(bodyCollider);
            HasSample = true;
            contactCount = 0;
            cachedBoxColliderCount = 0;
            UpdateJumpOriginExclusion();

            CollectClosestPointContacts();
            CollectDirectionalContacts();

            if (TryBuildSurface(out Vector3 point, out Vector3 normal))
            {
                HasDetectedSurface = true;

                if (!State.IsAttached)
                {
                    if (hasConfirmationCandidate &&
                        Vector3.Dot(confirmationNormal, normal) >= ATTACHMENT_CONFIRMATION_NORMAL_DOT)
                    {
                        attachmentFrames++;
                    }
                    else
                    {
                        confirmationNormal = normal;
                        hasConfirmationCandidate = true;
                        attachmentFrames = 1;
                    }
                }

                if (State.IsAttached || attachmentFrames >= config.AttachConfirmationFrames)
                {
                    State.SetAttached(point, normal, selectedContacts, selectedContactCount);
                    lastAttachedBodyPosition = bodyCollider.attachedRigidbody.position;
                    lastAttachedBodyRotation = bodyCollider.attachedRigidbody.rotation;
                }

                return;
            }

            HasDetectedSurface = false;
            attachmentFrames = 0;
            hasConfirmationCandidate = false;
            selectedContactCount = 0;

            if (State.IsAttached)
            {
                LogTraversal($"Sample found no support. contacts={contactCount}, position={bodyCenter}, normal={State.Normal}");
                State.SetAirborne();
            }
        }

        public void BeginJump()
        {
            jumpOriginColliderCount = 0;

            if (jumpOriginColliders.Length < State.Colliders.Count)
            {
                System.Array.Resize(ref jumpOriginColliders, State.Colliders.Count);
            }

            foreach (Collider collider in State.Colliders)
            {
                jumpOriginColliders[jumpOriginColliderCount++] = collider;
            }

            ignoresJumpOrigin = jumpOriginColliderCount > 0;
            attachmentFrames = 0;
            hasConfirmationCandidate = false;
            HasDetectedSurface = false;
            State.SetAirborne();
        }

        internal bool TryFindPredictedSupport(
            SpiderSurfaceState currentSurface,
            Vector3 predictedBodyCenter,
            out SpiderSurfaceContact predictedSupport)
        {
            predictedSupport = default;
            float searchRadius = bodyRadius + config.SurfaceSearchDistance;
            int overlappingCount = FindOverlappingColliders(predictedBodyCenter, searchRadius);
            float minimumNormalDot = Mathf.Cos(config.MaxSurfaceBlendAngle * Mathf.Deg2Rad);
            float normalPenaltyDistance = config.SurfaceSearchDistance * 0.25f;
            float bestScore = float.MaxValue;

            for (int colliderIndex = 0; colliderIndex < overlappingCount; colliderIndex++)
            {
                Collider collider = overlappingColliders[colliderIndex];

                if (!TryCreateClosestPointContact(
                        collider,
                        predictedBodyCenter,
                        overlappingCount,
                        out SpiderSurfaceContact contact) ||
                    IsAboveUnsupportedBoxGap(collider, predictedBodyCenter, overlappingCount) ||
                    Vector3.Dot(currentSurface.Normal, contact.Normal) < minimumNormalDot)
                {
                    continue;
                }

                float score = contact.Distance +
                              (1f - Vector3.Dot(currentSurface.Normal, contact.Normal)) * normalPenaltyDistance;

                if (!IsBetterContactScore(score, contact, bestScore, predictedSupport))
                {
                    continue;
                }

                predictedSupport = contact;
                bestScore = score;
            }

            return bestScore < float.MaxValue;
        }

        internal SpiderSurfaceContact GetContact(int index)
        {
            return contacts[index];
        }

        internal SpiderSurfaceContact GetSelectedContact(int index)
        {
            return selectedContacts[index];
        }

        private void CollectClosestPointContacts()
        {
            float searchRadius = bodyRadius + config.SurfaceSearchDistance;
            int overlappingCount = FindOverlappingColliders(bodyCenter, searchRadius);

            for (int colliderIndex = 0; colliderIndex < overlappingCount; colliderIndex++)
            {
                Collider collider = overlappingColliders[colliderIndex];

                // Moving supports are outside the initial traversal slice.
                if (collider.attachedRigidbody != null)
                {
                    continue;
                }

                if (!TryCreateClosestPointContact(collider, bodyCenter, overlappingCount, out SpiderSurfaceContact contact))
                {
                    continue;
                }

                AddContact(contact);
            }
        }

        private bool TryCreateClosestPointContact(
            Collider collider,
            Vector3 sampleCenter,
            int overlappingCount,
            out SpiderSurfaceContact contact)
        {
            if (collider is BoxCollider boxCollider)
            {
                return TryCreateBoxContact(boxCollider, sampleCenter, overlappingCount, out contact);
            }

            return TryCreateContact(collider, sampleCenter, out contact);
        }

        private void CollectDirectionalContacts()
        {
            float probeDistance = bodyRadius + config.SurfaceSearchDistance;

            foreach (Vector3 localDirection in probeDirections)
            {
                Vector3 direction = bodyCollider.transform.TransformDirection(localDirection);

                if (!Physics.Raycast(
                        bodyCenter,
                        direction,
                        out RaycastHit hit,
                        probeDistance,
                        config.TraversableSurfaceMask,
                        QueryTriggerInteraction.Ignore) || hit.collider.attachedRigidbody != null)
                {
                    continue;
                }

                if (UsesExpandedSurfaceNormal(hit.collider))
                {
                    continue;
                }

                AddContact(hit.collider, hit.point, hit.normal, Mathf.Max(0f, hit.distance - bodyRadius));
            }
        }

        private bool TryCreateContact(Collider collider, Vector3 sampleCenter, out SpiderSurfaceContact contact)
        {
            contact = default;

            if (collider.attachedRigidbody != null || IsIgnoredJumpOrigin(collider))
            {
                return false;
            }

            Vector3 closestPoint = collider.ClosestPoint(sampleCenter);
            Vector3 fromPointToCenter = sampleCenter - closestPoint;
            float pointDistance = fromPointToCenter.magnitude;

            if (pointDistance <= MIN_VECTOR_SQR_MAGNITUDE ||
                !Physics.Raycast(
                    sampleCenter,
                    -fromPointToCenter / pointDistance,
                    out RaycastHit hit,
                    pointDistance + Physics.defaultContactOffset,
                    config.TraversableSurfaceMask,
                    QueryTriggerInteraction.Ignore) || hit.collider != collider)
            {
                return false;
            }

            if (UsesExpandedSurfaceNormal(collider))
            {
                contact = new SpiderSurfaceContact(
                    collider,
                    closestPoint,
                    fromPointToCenter / pointDistance,
                    Mathf.Max(0f, pointDistance - bodyRadius));
                return true;
            }

            contact = new SpiderSurfaceContact(
                collider,
                hit.point,
                hit.normal,
                Mathf.Max(0f, hit.distance - bodyRadius));
            return true;
        }

        private bool TryCreateBoxContact(
            BoxCollider collider,
            Vector3 sampleCenter,
            int overlappingCount,
            out SpiderSurfaceContact contact)
        {
            contact = default;

            if (collider.attachedRigidbody != null || IsIgnoredJumpOrigin(collider))
            {
                return false;
            }

            Vector3 closestPoint = collider.ClosestPoint(sampleCenter);
            Vector3 fromPointToCenter = sampleCenter - closestPoint;
            float closestDistance = fromPointToCenter.magnitude;

            if (closestDistance <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                return false;
            }

            Vector3 resolvedPoint = closestPoint;
            float resolvedDistance = closestDistance;
            Collider seamPartner = null;
            bool usesVirtualSeam = false;

            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                if (!TryCreateBoxFace(collider, faceIndex, sampleCenter, out SpiderBoxFace face))
                {
                    continue;
                }

                for (int colliderIndex = 0; colliderIndex < overlappingCount; colliderIndex++)
                {
                    if (overlappingColliders[colliderIndex] is not BoxCollider partner ||
                        partner == collider ||
                        partner.attachedRigidbody != null ||
                        IsIgnoredJumpOrigin(partner))
                    {
                        continue;
                    }

                    for (int partnerFaceIndex = 0; partnerFaceIndex < 6; partnerFaceIndex++)
                    {
                        if (!TryCreateBoxFace(partner, partnerFaceIndex, sampleCenter, out SpiderBoxFace partnerFace) ||
                            !TryGetFaceSeam(face, partnerFace, out float gap, out Vector4 bridgeBounds) ||
                            gap > bodyRadius + Physics.defaultContactOffset)
                        {
                            continue;
                        }

                        Vector3 candidatePoint = FindClosestPointOnFacePair(
                            sampleCenter,
                            face,
                            partnerFace,
                            bridgeBounds,
                            out bool candidateUsesVirtualSeam);
                        float candidateDistance = Vector3.Distance(sampleCenter, candidatePoint);

                        if (candidateDistance >= resolvedDistance - SCORE_COMPARISON_EPSILON)
                        {
                            continue;
                        }

                        resolvedPoint = candidatePoint;
                        resolvedDistance = candidateDistance;
                        seamPartner = partner;
                        usesVirtualSeam = candidateUsesVirtualSeam;
                    }
                }
            }

            if (!HasVisibleBoxSupport(
                    sampleCenter,
                    resolvedPoint,
                    collider,
                    seamPartner,
                    usesVirtualSeam))
            {
                return false;
            }

            Vector3 normal = sampleCenter - resolvedPoint;

            if (normal.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                return false;
            }

            contact = new SpiderSurfaceContact(
                collider,
                resolvedPoint,
                normal.normalized,
                Mathf.Max(0f, resolvedDistance - bodyRadius));
            return true;
        }

        private bool HasVisibleBoxSupport(
            Vector3 sampleCenter,
            Vector3 supportPoint,
            BoxCollider source,
            Collider seamPartner,
            bool usesVirtualSeam)
        {
            Vector3 toSupport = supportPoint - sampleCenter;
            float supportDistance = toSupport.magnitude;

            if (supportDistance <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                return false;
            }

            if (!Physics.Raycast(
                sampleCenter,
                toSupport / supportDistance,
                out RaycastHit hit,
                supportDistance + Physics.defaultContactOffset,
                config.TraversableSurfaceMask,
                QueryTriggerInteraction.Ignore))
            {
                return usesVirtualSeam;
            }

            return hit.collider == source || hit.collider == seamPartner;
        }

        private bool TryCreateBoxFace(
            BoxCollider collider,
            int faceIndex,
            Vector3 sampleCenter,
            out SpiderBoxFace face)
        {
            face = GetBoxFace(collider, faceIndex);

            if (Vector3.Dot(sampleCenter - face.Center, face.Normal) < -Physics.defaultContactOffset)
            {
                face = default;
                return false;
            }

            return true;
        }

        private SpiderBoxFace GetBoxFace(BoxCollider collider, int faceIndex)
        {
            int colliderIndex = FindCachedBoxCollider(collider);

            if (colliderIndex < 0)
            {
                colliderIndex = CacheBoxCollider(collider);
            }

            return cachedBoxFaces[colliderIndex * 6 + faceIndex];
        }

        private int FindCachedBoxCollider(BoxCollider collider)
        {
            for (int colliderIndex = 0; colliderIndex < cachedBoxColliderCount; colliderIndex++)
            {
                if (cachedBoxColliders[colliderIndex] == collider)
                {
                    return colliderIndex;
                }
            }

            return -1;
        }

        private int CacheBoxCollider(BoxCollider collider)
        {
            if (cachedBoxColliderCount == cachedBoxColliders.Length)
            {
                System.Array.Resize(ref cachedBoxColliders, cachedBoxColliders.Length * 2);
                System.Array.Resize(ref cachedBoxFaces, cachedBoxFaces.Length * 2);
            }

            int colliderIndex = cachedBoxColliderCount++;
            cachedBoxColliders[colliderIndex] = collider;

            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                cachedBoxFaces[colliderIndex * 6 + faceIndex] = CreateBoxFace(collider, faceIndex);
            }

            return colliderIndex;
        }

        private static SpiderBoxFace CreateBoxFace(BoxCollider collider, int faceIndex)
        {
            int normalAxis = faceIndex / 2;
            float normalSign = faceIndex % 2 == 0 ? 1f : -1f;
            Vector3 localNormal = GetAxis(normalAxis) * normalSign;
            Vector3 localExtents = collider.size * 0.5f;
            Vector3 localCenter = collider.center + localNormal * GetAxisValue(localExtents, normalAxis);
            Transform transform = collider.transform;
            Vector3 normal = transform.worldToLocalMatrix.transpose.MultiplyVector(localNormal).normalized;
            Vector3 center = transform.TransformPoint(localCenter);
            int axisU = (normalAxis + 1) % 3;
            int axisV = (normalAxis + 2) % 3;
            Vector3 localU = GetAxis(axisU);
            Vector3 localV = GetAxis(axisV);
            Vector3 worldU = transform.TransformVector(localU);
            Vector3 worldV = transform.TransformVector(localV);
            float extentU = GetAxisValue(localExtents, axisU) * worldU.magnitude;
            float extentV = GetAxisValue(localExtents, axisV) * worldV.magnitude;

            return new SpiderBoxFace(
                center,
                normal,
                worldU.normalized,
                worldV.normalized,
                extentU,
                extentV);
        }

        private static Vector3 GetAxis(int axis)
        {
            return axis switch
            {
                0 => Vector3.right,
                1 => Vector3.up,
                _ => Vector3.forward
            };
        }

        private static float GetAxisValue(Vector3 value, int axis)
        {
            return axis switch
            {
                0 => value.x,
                1 => value.y,
                _ => value.z
            };
        }

        private static bool TryGetFaceSeam(
            SpiderBoxFace first,
            SpiderBoxFace second,
            out float gap,
            out Vector4 bridgeBounds)
        {
            gap = 0f;
            bridgeBounds = default;

            if (Vector3.Dot(first.Normal, second.Normal) < FACE_ALIGNMENT_DOT ||
                Mathf.Abs(Vector3.Dot(second.Center - first.Center, first.Normal)) > Physics.defaultContactOffset)
            {
                return false;
            }

            GetFaceIntervals(second, first, out float secondMinU, out float secondMaxU, out float secondMinV, out float secondMaxV);
            float firstMinU = -first.ExtentU;
            float firstMaxU = first.ExtentU;
            float firstMinV = -first.ExtentV;
            float firstMaxV = first.ExtentV;
            float overlapU = Mathf.Min(firstMaxU, secondMaxU) - Mathf.Max(firstMinU, secondMinU);
            float overlapV = Mathf.Min(firstMaxV, secondMaxV) - Mathf.Max(firstMinV, secondMinV);

            if (overlapU >= -Physics.defaultContactOffset && overlapV >= -Physics.defaultContactOffset)
            {
                return true;
            }

            if (overlapU >= -Physics.defaultContactOffset)
            {
                gap = -overlapV;
                bridgeBounds = new Vector4(
                    Mathf.Max(firstMinU, secondMinU),
                    Mathf.Min(firstMaxU, secondMaxU),
                    Mathf.Min(firstMaxV, secondMaxV),
                    Mathf.Max(firstMinV, secondMinV));
                return true;
            }

            if (overlapV >= -Physics.defaultContactOffset)
            {
                gap = -overlapU;
                bridgeBounds = new Vector4(
                    Mathf.Min(firstMaxU, secondMaxU),
                    Mathf.Max(firstMinU, secondMinU),
                    Mathf.Max(firstMinV, secondMinV),
                    Mathf.Min(firstMaxV, secondMaxV));
                return true;
            }

            return false;
        }

        private static void GetFaceIntervals(
            SpiderBoxFace target,
            SpiderBoxFace reference,
            out float minU,
            out float maxU,
            out float minV,
            out float maxV)
        {
            minU = float.MaxValue;
            maxU = float.MinValue;
            minV = float.MaxValue;
            maxV = float.MinValue;

            for (int uSign = -1; uSign <= 1; uSign += 2)
            {
                for (int vSign = -1; vSign <= 1; vSign += 2)
                {
                    Vector3 corner = target.Center +
                                     target.AxisU * (target.ExtentU * uSign) +
                                     target.AxisV * (target.ExtentV * vSign);
                    Vector3 offset = corner - reference.Center;
                    float u = Vector3.Dot(offset, reference.AxisU);
                    float v = Vector3.Dot(offset, reference.AxisV);
                    minU = Mathf.Min(minU, u);
                    maxU = Mathf.Max(maxU, u);
                    minV = Mathf.Min(minV, v);
                    maxV = Mathf.Max(maxV, v);
                }
            }
        }

        private static Vector3 FindClosestPointOnFacePair(
            Vector3 sampleCenter,
            SpiderBoxFace first,
            SpiderBoxFace second,
            Vector4 bridgeBounds,
            out bool usesVirtualSeam)
        {
            Vector3 planePoint = sampleCenter - first.Normal * Vector3.Dot(sampleCenter - first.Center, first.Normal);

            if (IsInsideFace(planePoint, first) || IsInsideFace(planePoint, second))
            {
                usesVirtualSeam = false;
                return planePoint;
            }

            if (IsInsideBridge(planePoint, first, bridgeBounds))
            {
                // The authored seam spans this short empty interval as one support patch.
                usesVirtualSeam = true;
                return planePoint;
            }

            Vector3 firstPoint = ClosestPointOnFace(planePoint, first);
            Vector3 secondPoint = ClosestPointOnFace(planePoint, second);
            usesVirtualSeam = false;
            return (sampleCenter - firstPoint).sqrMagnitude <= (sampleCenter - secondPoint).sqrMagnitude
                ? firstPoint
                : secondPoint;
        }

        private static bool IsInsideFace(Vector3 point, SpiderBoxFace face)
        {
            Vector3 offset = point - face.Center;
            return Mathf.Abs(Vector3.Dot(offset, face.AxisU)) <= face.ExtentU + Physics.defaultContactOffset &&
                   Mathf.Abs(Vector3.Dot(offset, face.AxisV)) <= face.ExtentV + Physics.defaultContactOffset;
        }

        private static bool IsInsideBridge(Vector3 point, SpiderBoxFace face, Vector4 bridgeBounds)
        {
            if (bridgeBounds.x > bridgeBounds.y || bridgeBounds.z > bridgeBounds.w)
            {
                return false;
            }

            Vector3 offset = point - face.Center;
            float u = Vector3.Dot(offset, face.AxisU);
            float v = Vector3.Dot(offset, face.AxisV);
            return u >= bridgeBounds.x - Physics.defaultContactOffset &&
                   u <= bridgeBounds.y + Physics.defaultContactOffset &&
                   v >= bridgeBounds.z - Physics.defaultContactOffset &&
                   v <= bridgeBounds.w + Physics.defaultContactOffset;
        }

        private static Vector3 ClosestPointOnFace(Vector3 point, SpiderBoxFace face)
        {
            Vector3 offset = point - face.Center;
            float u = Mathf.Clamp(Vector3.Dot(offset, face.AxisU), -face.ExtentU, face.ExtentU);
            float v = Mathf.Clamp(Vector3.Dot(offset, face.AxisV), -face.ExtentV, face.ExtentV);
            return face.Center + face.AxisU * u + face.AxisV * v;
        }

        private static bool UsesExpandedSurfaceNormal(Collider collider)
        {
            return collider is BoxCollider ||
                   collider is SphereCollider ||
                   collider is CapsuleCollider ||
                   collider is MeshCollider { convex: true };
        }

        private void AddContact(SpiderSurfaceContact contact)
        {
            AddContact(contact.Collider, contact.Point, contact.Normal, contact.Distance);
        }

        private void AddContact(Collider collider, Vector3 point, Vector3 normal, float distance)
        {
            if (normal.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE || IsIgnoredJumpOrigin(collider))
            {
                return;
            }

            normal.Normalize();

            for (int contactIndex = 0; contactIndex < contactCount; contactIndex++)
            {
                SpiderSurfaceContact existing = contacts[contactIndex];

                if (existing.Collider != collider || Vector3.Dot(existing.Normal, normal) < NORMAL_DEDUPLICATION_DOT)
                {
                    continue;
                }

                if (distance < existing.Distance)
                {
                    contacts[contactIndex] = new SpiderSurfaceContact(collider, point, normal, distance);
                }

                return;
            }

            if (contactCount == contacts.Length)
            {
                System.Array.Resize(ref contacts, contacts.Length * 2);
                System.Array.Resize(ref selectedContacts, selectedContacts.Length * 2);
                System.Array.Resize(ref processedContacts, processedContacts.Length * 2);
            }

            contacts[contactCount++] = new SpiderSurfaceContact(collider, point, normal, distance);
        }

        private void UpdateJumpOriginExclusion()
        {
            if (!ignoresJumpOrigin)
            {
                return;
            }

            float searchRadius = bodyRadius + config.SurfaceSearchDistance;

            Rigidbody bodyRigidbody = bodyCollider.attachedRigidbody;
            bool isLeavingJumpOrigin = false;

            for (int colliderIndex = 0; colliderIndex < jumpOriginColliderCount; colliderIndex++)
            {
                Collider collider = jumpOriginColliders[colliderIndex];

                if (collider == null)
                {
                    continue;
                }

                Vector3 closestPoint = collider.ClosestPoint(bodyCenter);
                Vector3 fromSurface = bodyCenter - closestPoint;

                if (fromSurface.sqrMagnitude > searchRadius * searchRadius)
                {
                    continue;
                }

                if (Vector3.Dot(bodyRigidbody.linearVelocity, fromSurface) < 0f)
                {
                    ignoresJumpOrigin = false;
                    jumpOriginColliderCount = 0;
                    return;
                }

                isLeavingJumpOrigin = true;
            }

            if (isLeavingJumpOrigin)
            {
                return;
            }

            ignoresJumpOrigin = false;
            jumpOriginColliderCount = 0;
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void LogTraversal(string message)
        {
            Debug.Log($"[SpiderTraversal] {message}", bodyCollider);
        }

        private bool IsIgnoredJumpOrigin(Collider collider)
        {
            if (!ignoresJumpOrigin)
            {
                return false;
            }

            for (int colliderIndex = 0; colliderIndex < jumpOriginColliderCount; colliderIndex++)
            {
                if (jumpOriginColliders[colliderIndex] == collider)
                {
                    return true;
                }
            }

            return false;
        }

        private int FindOverlappingColliders(Vector3 center, float radius)
        {
            while (true)
            {
                int count = Physics.OverlapSphereNonAlloc(
                    center,
                    radius,
                    overlappingColliders,
                    config.TraversableSurfaceMask,
                    QueryTriggerInteraction.Ignore);

                if (count < overlappingColliders.Length)
                {
                    return count;
                }

                System.Array.Resize(ref overlappingColliders, overlappingColliders.Length * 2);
            }
        }

        private bool TryBuildSurface(out Vector3 point, out Vector3 normal)
        {
            point = default;
            normal = default;
            selectedContactCount = 0;

            if (contactCount == 0)
            {
                return false;
            }

            int anchorIndex = SelectAnchorContact();
            selectedContacts[selectedContactCount++] = contacts[anchorIndex];
            System.Array.Clear(processedContacts, 0, contactCount);
            processedContacts[anchorIndex] = true;

            for (int selectedIndex = 1; selectedIndex < contactCount; selectedIndex++)
            {
                int nearestIndex = FindNearestUnprocessedContact();

                if (nearestIndex < 0)
                {
                    break;
                }

                processedContacts[nearestIndex] = true;

                if (IsCompatibleWithSelectedContacts(contacts[nearestIndex]))
                {
                    selectedContacts[selectedContactCount++] = contacts[nearestIndex];
                }
            }

            float totalWeight = 0f;
            Vector3 weightedPoint = Vector3.zero;
            Vector3 weightedNormal = Vector3.zero;
            float searchDistance = Mathf.Max(config.SurfaceSearchDistance, Physics.defaultContactOffset);

            for (int contactIndex = 0; contactIndex < selectedContactCount; contactIndex++)
            {
                SpiderSurfaceContact contact = selectedContacts[contactIndex];
                float proximity = 1f - Mathf.Clamp01(contact.Distance / searchDistance);
                float weight = Mathf.Max(0.01f, proximity * proximity);
                totalWeight += weight;
                weightedPoint += contact.Point * weight;
                weightedNormal += contact.Normal * weight;
            }

            if (weightedNormal.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                selectedContactCount = 0;
                return false;
            }

            point = weightedPoint / totalWeight;
            normal = weightedNormal.normalized;
            return true;
        }

        private int SelectAnchorContact()
        {
            int anchorIndex = 0;
            float bestScore = float.MaxValue;
            float normalPenaltyDistance = config.SurfaceSearchDistance * 0.25f;

            for (int contactIndex = 0; contactIndex < contactCount; contactIndex++)
            {
                SpiderSurfaceContact contact = contacts[contactIndex];
                float score = contact.Distance;

                if (State.IsAttached)
                {
                    score += (1f - Vector3.Dot(State.Normal, contact.Normal)) * normalPenaltyDistance;
                }

                if (IsBetterContactScore(score, contact, bestScore, contacts[anchorIndex]))
                {
                    bestScore = score;
                    anchorIndex = contactIndex;
                }
            }

            return anchorIndex;
        }

        private int FindNearestUnprocessedContact()
        {
            int nearestIndex = -1;
            float nearestDistance = float.MaxValue;

            for (int contactIndex = 0; contactIndex < contactCount; contactIndex++)
            {
                if (processedContacts[contactIndex] ||
                    !IsBetterContactScore(
                        contacts[contactIndex].Distance,
                        contacts[contactIndex],
                        nearestDistance,
                        nearestIndex >= 0 ? contacts[nearestIndex] : default))
                {
                    continue;
                }

                nearestDistance = contacts[contactIndex].Distance;
                nearestIndex = contactIndex;
            }

            return nearestIndex;
        }

        private bool IsCompatibleWithSelectedContacts(SpiderSurfaceContact candidate)
        {
            float minimumNormalDot = Mathf.Cos(config.MaxSurfaceBlendAngle * Mathf.Deg2Rad);

            for (int contactIndex = 0; contactIndex < selectedContactCount; contactIndex++)
            {
                SpiderSurfaceContact selectedContact = selectedContacts[contactIndex];

                if (Vector3.Dot(selectedContact.Normal, candidate.Normal) < minimumNormalDot ||
                    IsSeparatedByUnsupportedBoxGap(selectedContact, candidate))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSeparatedByUnsupportedBoxGap(SpiderSurfaceContact first, SpiderSurfaceContact second)
        {
            if (first.Collider is not BoxCollider firstCollider || second.Collider is not BoxCollider secondCollider)
            {
                return false;
            }

            return IsAboveUnsupportedBoxGap(firstCollider, secondCollider, bodyCenter);
        }

        private bool IsAboveUnsupportedBoxGap(Collider collider, Vector3 sampleCenter, int overlappingCount)
        {
            if (collider is not BoxCollider boxCollider)
            {
                return false;
            }

            for (int colliderIndex = 0; colliderIndex < overlappingCount; colliderIndex++)
            {
                if (overlappingColliders[colliderIndex] is BoxCollider partner &&
                    partner != boxCollider &&
                    IsAboveUnsupportedBoxGap(boxCollider, partner, sampleCenter))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAboveUnsupportedBoxGap(
            BoxCollider firstCollider,
            BoxCollider secondCollider,
            Vector3 sampleCenter)
        {
            for (int firstFaceIndex = 0; firstFaceIndex < 6; firstFaceIndex++)
            {
                if (!TryCreateBoxFace(firstCollider, firstFaceIndex, sampleCenter, out SpiderBoxFace firstFace))
                {
                    continue;
                }

                for (int secondFaceIndex = 0; secondFaceIndex < 6; secondFaceIndex++)
                {
                    if (!TryCreateBoxFace(secondCollider, secondFaceIndex, sampleCenter, out SpiderBoxFace secondFace) ||
                        !TryGetFaceSeam(firstFace, secondFace, out float gap, out Vector4 bridgeBounds) ||
                        gap <= bodyRadius + Physics.defaultContactOffset)
                    {
                        continue;
                    }

                    Vector3 planePoint = sampleCenter -
                                         firstFace.Normal * Vector3.Dot(sampleCenter - firstFace.Center, firstFace.Normal);

                    if (IsInsideBridge(planePoint, firstFace, bridgeBounds))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsBetterContactScore(
            float candidateScore,
            SpiderSurfaceContact candidate,
            float currentScore,
            SpiderSurfaceContact current)
        {
            if (candidateScore < currentScore - SCORE_COMPARISON_EPSILON)
            {
                return true;
            }

            if (candidateScore > currentScore + SCORE_COMPARISON_EPSILON || current.Collider == null)
            {
                return false;
            }

            return candidate.Collider.GetEntityId() < current.Collider.GetEntityId();
        }

    }
}
