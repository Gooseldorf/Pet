using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pet.Gameplay
{
    public sealed class SpiderLegController : MonoBehaviour
    {
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;

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

            foreach (SpiderLegBinding leg in legs)
            {
                InitializeLeg(leg);
            }

            IsInitialized = true;
        }

        public void Tick(float deltaTime, bool shouldRetract)
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

            bool hasSteppingLegs = false;

            foreach (SpiderLegBinding leg in legs)
            {
                UpdateDesiredPosition(leg);

                if (leg.IsStepping)
                {
                    AdvanceStep(leg, deltaTime);
                    hasSteppingLegs = true;
                    continue;
                }

                if (leg.HasSupport)
                {
                    HoldOnSupport(leg);
                }
            }

            if (hasSteppingLegs)
            {
                return;
            }

            int preferredGroup = 1 - lastStartedGaitGroup;

            if (TryStartGaitGroup(preferredGroup))
            {
                lastStartedGaitGroup = preferredGroup;
                return;
            }

            int alternateGroup = 1 - preferredGroup;

            if (TryStartGaitGroup(alternateGroup))
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

        public bool TryGetSupport(int legIndex, out SpiderLegSurfaceContact support)
        {
            SpiderLegBinding leg = legs[legIndex];
            support = new SpiderLegSurfaceContact(leg.SupportCollider, leg.SupportPoint, leg.SupportNormal, 0f);
            return leg.HasSupport;
        }

        private void InitializeLeg(SpiderLegBinding leg)
        {
            leg.RestPositionLocal = transform.InverseTransformPoint(leg.Target.position);
            leg.DesiredPosition = transform.TransformPoint(leg.RestPositionLocal);
            leg.StepStartNormal = transform.up;
            leg.NextSupportSearchTime = Time.time;

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

        private void UpdateDesiredPosition(SpiderLegBinding leg)
        {
            leg.DesiredPosition = transform.TransformPoint(leg.RestPositionLocal);
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

            BeginStep(leg, support.Point + support.Normal * config.LegFootOffset, support.Normal);
            leg.SupportPoint = support.Point;
            leg.SupportNormal = support.Normal;
            leg.SupportCollider = support.Collider;
            leg.HasLoggedSupportSearchFailure = false;
            LogLegs(
                $"Started step for {leg.Root.name}. desiredDistance=" +
                $"{Vector3.Distance(leg.DesiredPosition, support.Point):F3}, rootDistance=" +
                $"{Vector3.Distance(leg.Root.position, support.Point + support.Normal * config.LegFootOffset):F3}, " +
                $"support={support.Collider.name}.");
            return true;
        }

        private void BeginStep(SpiderLegBinding leg, Vector3 targetPosition, Vector3 targetNormal)
        {
            leg.StepStartPosition = leg.Target.position;
            leg.StepStartNormal = leg.HasSupport ? leg.SupportNormal : transform.up;
            leg.StepStartRotation = leg.Target.rotation;
            leg.StepTargetRotation = CalculateTargetRotation(targetNormal);
            leg.StepProgress = 0f;
            leg.IsStepping = true;

            leg.SupportPoint = targetPosition - targetNormal * config.LegFootOffset;
            leg.SupportNormal = targetNormal;
        }

        private void AdvanceStep(SpiderLegBinding leg, float deltaTime)
        {
            leg.StepProgress = Mathf.Min(1f, leg.StepProgress + deltaTime / config.LegStepDuration);
            float arcHeight = config.LegStepHeight * 4f * leg.StepProgress * (1f - leg.StepProgress);
            Vector3 normal = Vector3.Slerp(leg.StepStartNormal, leg.SupportNormal, leg.StepProgress).normalized;
            Vector3 position = Vector3.Lerp(leg.StepStartPosition, leg.SupportPoint + leg.SupportNormal * config.LegFootOffset, leg.StepProgress);
            Quaternion rotation = Quaternion.Slerp(leg.StepStartRotation, leg.StepTargetRotation, leg.StepProgress);
            leg.Target.SetPositionAndRotation(position + normal * arcHeight, rotation);

            if (leg.StepProgress >= 1f)
            {
                leg.IsStepping = false;
                leg.NextSupportSearchTime = Time.time + config.LegUngroundedResampleInterval;
            }
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

        private void RetractLegs(float deltaTime)
        {
            float interpolation = Mathf.Clamp01(deltaTime / config.LegStepDuration);
            Quaternion targetRotation = CalculateTargetRotation(transform.up);

            foreach (SpiderLegBinding leg in legs)
            {
                UpdateDesiredPosition(leg);
                leg.IsStepping = false;
                leg.SupportCollider = null;
                leg.Target.SetPositionAndRotation(
                    Vector3.Lerp(leg.Target.position, leg.DesiredPosition, interpolation),
                    Quaternion.Slerp(leg.Target.rotation, targetRotation, interpolation));
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
