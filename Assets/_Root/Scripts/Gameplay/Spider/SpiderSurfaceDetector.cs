using UnityEngine;

namespace Pet.Gameplay
{
    public sealed class SpiderSurfaceDetector
    {
        private const int MAX_OVERLAPPING_COLLIDERS = 32;
        private const int MAX_CONTACTS = 64;
        private const float NORMAL_DEDUPLICATION_DOT = 0.999f;
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;

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
        private readonly Collider[] overlappingColliders = new Collider[MAX_OVERLAPPING_COLLIDERS];
        private readonly SpiderSurfaceContact[] contacts = new SpiderSurfaceContact[MAX_CONTACTS];
        private readonly SpiderSurfaceContact[] selectedContacts = new SpiderSurfaceContact[MAX_CONTACTS];
        private readonly bool[] processedContacts = new bool[MAX_CONTACTS];

        private int contactCount;
        private int selectedContactCount;
        private int attachmentFrames;
        private int missingFrames;
        private Vector3 bodyCenter;
        private float bodyRadius;

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

        public void Sample()
        {
            bodyCenter = bodyCollider.transform.TransformPoint(bodyCollider.center);
            bodyRadius = CalculateWorldRadius();
            HasSample = true;
            contactCount = 0;

            CollectClosestPointContacts();
            CollectDirectionalContacts();

            if (TryBuildSurface(out Vector3 point, out Vector3 normal))
            {
                attachmentFrames++;
                missingFrames = 0;

                if (State.IsAttached || attachmentFrames >= config.AttachConfirmationFrames)
                {
                    State.SetAttached(point, normal, selectedContacts, selectedContactCount);
                }

                return;
            }

            attachmentFrames = 0;
            missingFrames++;

            if (State.IsAttached && missingFrames > config.DetachGraceFrames)
            {
                State.SetAirborne();
            }
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
            int overlappingCount = Physics.OverlapSphereNonAlloc(
                bodyCenter,
                searchRadius,
                overlappingColliders,
                config.TraversableSurfaceMask,
                QueryTriggerInteraction.Ignore);

            for (int colliderIndex = 0; colliderIndex < overlappingCount; colliderIndex++)
            {
                Collider collider = overlappingColliders[colliderIndex];

                // Moving supports are outside the initial traversal slice.
                if (collider.attachedRigidbody != null)
                {
                    continue;
                }

                Vector3 point = collider.ClosestPoint(bodyCenter);
                Vector3 fromPointToBody = bodyCenter - point;
                float pointDistance = fromPointToBody.magnitude;

                if (pointDistance <= MIN_VECTOR_SQR_MAGNITUDE)
                {
                    continue;
                }

                Vector3 rayDirection = -fromPointToBody / pointDistance;

                if (!Physics.Raycast(
                        bodyCenter,
                        rayDirection,
                        out RaycastHit hit,
                        pointDistance + Physics.defaultContactOffset,
                        config.TraversableSurfaceMask,
                        QueryTriggerInteraction.Ignore) || hit.collider != collider)
                {
                    continue;
                }

                AddContact(collider, hit.point, hit.normal, Mathf.Max(0f, hit.distance - bodyRadius));
            }
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

                AddContact(hit.collider, hit.point, hit.normal, Mathf.Max(0f, hit.distance - bodyRadius));
            }
        }

        private void AddContact(Collider collider, Vector3 point, Vector3 normal, float distance)
        {
            if (normal.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE)
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

            if (contactCount < contacts.Length)
            {
                contacts[contactCount++] = new SpiderSurfaceContact(collider, point, normal, distance);
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

                if (score < bestScore)
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
                if (processedContacts[contactIndex] || contacts[contactIndex].Distance >= nearestDistance)
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
                if (Vector3.Dot(selectedContacts[contactIndex].Normal, candidate.Normal) < minimumNormalDot)
                {
                    return false;
                }
            }

            return true;
        }

        private float CalculateWorldRadius()
        {
            Vector3 scale = bodyCollider.transform.lossyScale;
            float largestScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            return bodyCollider.radius * largestScale;
        }
    }
}
