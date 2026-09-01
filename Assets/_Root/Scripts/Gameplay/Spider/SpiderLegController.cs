using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pet.Gameplay
{
    public sealed class SpiderLegController : MonoBehaviour
    {
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;
        private const float MIN_STEP_DIRECTION_SPEED = 0.05f;

        [Header("Required References")]
        [SerializeField] private SpiderLegBinding[] legs;

        private SpiderConfig config;
        private SpiderLegSurfaceSampler surfaceSampler;
        private int lastStartedGaitGroup = 1;
        private bool isRetracting;

        public bool IsInitialized { get; private set; }
        public int LegCount => legs.Length;
        public float SearchRadius => surfaceSampler.SearchRadius;

        public void Initialize(SpiderConfig config)
        {
            this.config = config;
            surfaceSampler = new SpiderLegSurfaceSampler(config);

            for (int legIndex = 0; legIndex < legs.Length; legIndex++)
            {
                InitializeLeg(legs[legIndex], legIndex);
            }

            IsInitialized = true;
        }

        public void Tick(float deltaTime, bool shouldRetract, Vector3 bodyVelocity)
        {
            if (!IsInitialized)
            {
                return;
            }

            if (shouldRetract)
            {
                if (!isRetracting)
                {
                    LogLegs("Entering airborne leg retraction.");
                    BeginRetraction();
                }

                isRetracting = true;
                RetractLegs(deltaTime);
                return;
            }

            if (isRetracting)
            {
                LogLegs("Leaving airborne leg retraction.");
                isRetracting = false;
            }

            Vector3 stepDirection = CalculateStepDirection(bodyVelocity);
            bool hasSteppingLegs = false;

            foreach (SpiderLegBinding leg in legs)
            {
                UpdateDesiredPosition(leg, stepDirection);

                if (leg.IsStepping)
                {
                    AdvanceStep(leg, deltaTime);
                    hasSteppingLegs |= leg.IsStepping;
                    continue;
                }

                if (leg.HasSupport)
                {
                    HoldOnSupport(leg);
                }
            }

            int preferredGroup = 1 - lastStartedGaitGroup;

            if (CanStartGaitGroup(preferredGroup, hasSteppingLegs) && TryStartGaitGroup(preferredGroup))
            {
                lastStartedGaitGroup = preferredGroup;
                return;
            }

            int alternateGroup = 1 - preferredGroup;

            if (CanStartGaitGroup(alternateGroup, hasSteppingLegs) && TryStartGaitGroup(alternateGroup))
            {
                lastStartedGaitGroup = alternateGroup;
            }
        }

        public Transform GetRoot(int legIndex)
        {
            return legs[legIndex].Root;
        }

        public Transform GetTarget(int legIndex)
        {
            return legs[legIndex].Target;
        }

        public Vector3 GetDesiredPosition(int legIndex)
        {
            return legs[legIndex].DesiredPosition;
        }

        public bool IsLegStepping(int legIndex)
        {
            return legs[legIndex].IsStepping;
        }

        internal bool TryGetSupport(int legIndex, out SpiderLegSurfaceContact support)
        {
            SpiderLegBinding leg = legs[legIndex];
            support = new SpiderLegSurfaceContact(leg.SupportCollider, leg.SupportPoint, leg.SupportNormal, 0f);
            return leg.HasSupport;
        }

        private void InitializeLeg(SpiderLegBinding leg, int legIndex)
        {
            leg.RestPositionLocal = transform.InverseTransformPoint(leg.Target.position);
            leg.DesiredPosition = transform.TransformPoint(leg.RestPositionLocal);
            leg.StepStartNormal = transform.up;
            leg.NextSupportSearchTime = Time.time +
                                        config.LegUngroundedResampleInterval * legIndex / Mathf.Max(1, legs.Length);

            leg.MaxReach = Mathf.Min(CalculateChainReach(leg), config.LegMaxReach);

            if (surfaceSampler.TryFindSupport(
                    leg.DesiredPosition,
                    leg.Root.position,
                    leg.MaxReach,
                    config.LegFootOffset,
                    out SpiderLegSurfaceContact support))
            {
                PlaceOnSupport(leg, support);
                LogLegs(
                    $"Initialized {leg.Root.name}. group={leg.GaitGroup}, maxReach={leg.MaxReach:F3}, " +
                    $"searchRadius={surfaceSampler.SearchRadius:F3}, support={support.Collider.name}.");
                return;
            }

            leg.Target.SetPositionAndRotation(leg.DesiredPosition, CalculateTargetRotation(transform.up));
            leg.SupportCollider = null;
            LogLegs(
                $"Initialized {leg.Root.name}. group={leg.GaitGroup}, maxReach={leg.MaxReach:F3}, " +
                $"searchRadius={surfaceSampler.SearchRadius:F3}, support=none.");
        }

        private bool TryStartGaitGroup(int gaitGroup)
        {
            bool startedStep = false;

            foreach (SpiderLegBinding leg in legs)
            {
                if (leg.GaitGroup != gaitGroup || !NeedsStep(leg))
                {
                    continue;
                }

                startedStep |= TryStartStep(leg);
            }

            return startedStep;
        }

        private bool CanStartGaitGroup(int gaitGroup, bool hasSteppingLegs)
        {
            foreach (SpiderLegBinding leg in legs)
            {
                if (leg.GaitGroup == gaitGroup && leg.IsStepping)
                {
                    return false;
                }
            }

            if (!hasSteppingLegs)
            {
                return true;
            }

            foreach (SpiderLegBinding leg in legs)
            {
                if (leg.IsStepping && leg.StepProgress >= config.LegGaitOverlapProgress)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateDesiredPosition(SpiderLegBinding leg, Vector3 stepDirection)
        {
            Vector3 neutralPosition = transform.TransformPoint(leg.RestPositionLocal);
            leg.DesiredPosition = neutralPosition + stepDirection * config.LegStepForwardDistance;
        }

        private bool NeedsStep(SpiderLegBinding leg)
        {
            if (!leg.HasSupport)
            {
                return Time.time >= leg.NextSupportSearchTime;
            }

            Vector3 supportTargetPosition = leg.SupportPoint + leg.SupportNormal * config.LegFootOffset;
            bool shouldStep = Vector3.Distance(supportTargetPosition, leg.DesiredPosition) >= config.LegStepDistance ||
                              Vector3.Distance(leg.Root.position, supportTargetPosition) > leg.MaxReach;
            return shouldStep && Time.time >= leg.NextSupportSearchTime;
        }

        private bool TryStartStep(SpiderLegBinding leg)
        {
            if (!surfaceSampler.TryFindSupport(
                    leg.DesiredPosition,
                    leg.Root.position,
                    leg.MaxReach,
                    config.LegFootOffset,
                    out SpiderLegSurfaceContact support))
            {
                if (leg.HasSupport && !IsSupportReachable(leg))
                {
                    leg.SupportCollider = null;
                }

                leg.NextSupportSearchTime = Time.time + config.LegUngroundedResampleInterval;

                if (!leg.HasLoggedSupportSearchFailure)
                {
                    LogLegs(
                        $"No reachable support for {leg.Root.name}. desired={leg.DesiredPosition}, " +
                        $"root={leg.Root.position}, maxReach={leg.MaxReach:F3}.");
                    leg.HasLoggedSupportSearchFailure = true;
                }

                return false;
            }

            BeginStep(leg, support);
            leg.HasLoggedSupportSearchFailure = false;
            LogLegs(
                $"Started step for {leg.Root.name}. desiredDistance=" +
                $"{Vector3.Distance(leg.DesiredPosition, support.Point):F3}, rootDistance=" +
                $"{Vector3.Distance(leg.Root.position, support.Point + support.Normal * config.LegFootOffset):F3}, " +
                $"support={support.Collider.name}.");
            return true;
        }

        private void BeginStep(SpiderLegBinding leg, SpiderLegSurfaceContact target)
        {
            leg.StepStartPosition = leg.Target.position;
            leg.StepStartNormal = leg.HasSupport ? leg.SupportNormal : transform.up;
            leg.StepStartRotation = leg.Target.rotation;
            leg.StepTargetPoint = target.Point;
            leg.StepTargetNormal = target.Normal;
            leg.StepTargetCollider = target.Collider;
            leg.StepProgress = 0f;
            leg.IsStepping = true;
        }

        private void AdvanceStep(SpiderLegBinding leg, float deltaTime)
        {
            leg.StepProgress = Mathf.Min(1f, leg.StepProgress + deltaTime / config.LegStepDuration);
            float arcHeight = config.LegStepHeight * 4f * leg.StepProgress * (1f - leg.StepProgress);
            Vector3 normal = Vector3.Slerp(leg.StepStartNormal, leg.StepTargetNormal, leg.StepProgress).normalized;
            Vector3 position = Vector3.Lerp(
                leg.StepStartPosition,
                leg.StepTargetPoint + leg.StepTargetNormal * config.LegFootOffset,
                leg.StepProgress);
            Quaternion rotation = Quaternion.Slerp(
                leg.StepStartRotation,
                CalculateTargetRotation(leg.StepTargetNormal),
                leg.StepProgress);
            leg.Target.SetPositionAndRotation(position + normal * arcHeight, rotation);

            if (leg.StepProgress >= 1f)
            {
                leg.IsStepping = false;
                leg.SupportPoint = leg.StepTargetPoint;
                leg.SupportNormal = leg.StepTargetNormal;
                leg.SupportCollider = leg.StepTargetCollider;
            }
        }

        private Vector3 CalculateStepDirection(Vector3 bodyVelocity)
        {
            Vector3 tangentialVelocity = Vector3.ProjectOnPlane(bodyVelocity, transform.up);

            if (tangentialVelocity.sqrMagnitude < MIN_STEP_DIRECTION_SPEED * MIN_STEP_DIRECTION_SPEED)
            {
                return Vector3.zero;
            }

            return tangentialVelocity.normalized;
        }

        private Quaternion CalculateTargetRotation(Vector3 normal)
        {
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, normal);

            if (forward.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                forward = Vector3.ProjectOnPlane(transform.right, normal);
            }

            return Quaternion.LookRotation(forward.normalized, normal);
        }

        private void PlaceOnSupport(SpiderLegBinding leg, SpiderLegSurfaceContact support)
        {
            leg.SupportPoint = support.Point;
            leg.SupportNormal = support.Normal;
            leg.SupportCollider = support.Collider;
            leg.Target.SetPositionAndRotation(
                support.Point + support.Normal * config.LegFootOffset,
                CalculateTargetRotation(support.Normal));
        }

        private void HoldOnSupport(SpiderLegBinding leg)
        {
            leg.Target.SetPositionAndRotation(
                leg.SupportPoint + leg.SupportNormal * config.LegFootOffset,
                CalculateTargetRotation(leg.SupportNormal));
        }

        private bool IsSupportReachable(SpiderLegBinding leg)
        {
            Vector3 supportTargetPosition = leg.SupportPoint + leg.SupportNormal * config.LegFootOffset;
            return Vector3.Distance(leg.Root.position, supportTargetPosition) <= leg.MaxReach;
        }

        private void BeginRetraction()
        {
            foreach (SpiderLegBinding leg in legs)
            {
                leg.RetractStartPosition = leg.Target.position;
                leg.RetractStartRotation = leg.Target.rotation;
                leg.RetractProgress = 0f;
                leg.IsStepping = false;
                leg.SupportCollider = null;
            }
        }

        private void RetractLegs(float deltaTime)
        {
            Quaternion targetRotation = CalculateTargetRotation(transform.up);

            foreach (SpiderLegBinding leg in legs)
            {
                UpdateDesiredPosition(leg, Vector3.zero);
                leg.RetractProgress = Mathf.Min(1f, leg.RetractProgress + deltaTime / config.LegStepDuration);
                leg.Target.SetPositionAndRotation(
                    Vector3.Lerp(leg.RetractStartPosition, leg.DesiredPosition, leg.RetractProgress),
                    Quaternion.Slerp(leg.RetractStartRotation, targetRotation, leg.RetractProgress));
            }
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void LogLegs(string message)
        {
            Debug.Log($"[SpiderLegs] {message}", this);
        }

        private static float CalculateChainReach(SpiderLegBinding leg)
        {
            float reach = 0f;
            Transform current = leg.Tip;

            while (current != leg.Root)
            {
                Transform parent = current.parent;
                reach += Vector3.Distance(current.position, parent.position);
                current = parent;
            }

            return reach;
        }
    }
}
