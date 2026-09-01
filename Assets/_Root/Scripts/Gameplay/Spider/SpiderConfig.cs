using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "SpiderConfig", menuName = "Configs/Gameplay/Spider/SpiderConfig")]
    public class SpiderConfig : ScriptableObject
    {
        [Header("Prefab")]
        [field: SerializeField] public SpiderPlayerController Prefab { get; private set; }

        [Header("Surface Detection")]
        [SerializeField] private LayerMask traversableSurfaceMask;
        [SerializeField, Min(0.001f)] private float surfaceSearchDistance = 0.3f;
        [SerializeField, Range(0f, 180f)] private float maxSurfaceBlendAngle = 100f;
        [SerializeField, Min(1)] private int attachConfirmationFrames = 2;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float maxMoveSpeed = 5f;
        [SerializeField, Min(0f)] private float moveAcceleration = 25f;

        [Header("Adhesion")]
        [SerializeField, Min(0f)] private float surfaceHoverOffset = 0.1f;
        [SerializeField, Min(0f)] private float adhesionForce = 30f;
        [SerializeField, Min(0f)] private float surfaceStickSpeed = 3f;
        [SerializeField, Min(0f)] private float adhesionDeadZone = 0.05f;

        [Header("Orientation")]
        [SerializeField, Min(0f)] private float surfaceAlignmentSharpness = 12f;
        [SerializeField, Min(0f)] private float movingHeadingInputThreshold = 0.1f;
        [SerializeField, Min(0f)] private float movingHeadingAlignmentSharpness = 10f;
        [SerializeField, Min(0f)] private float headingAlignmentSharpness = 15f;

        [Header("Airborne")]
        [SerializeField, Min(0f)] private float airborneGravity = 18f;
        [SerializeField, Min(0f)] private float jumpSpeed = 5f;
        [SerializeField, Range(0f, 1f)] private float airControlCoefficient = 0.5f;

        [Header("Legs")]
        [SerializeField, Min(0.001f)] private float legSearchRadius = 0.3f;
        [SerializeField, Min(0.001f)] private float legMaxReach = 1.25f;
        [SerializeField, Min(0.001f)] private float legStepDistance = 0.3f;
        [SerializeField, Min(0f)] private float legStepForwardDistance = 0.15f;
        [SerializeField, Min(0f)] private float legStepHeight = 0.2f;
        [SerializeField, Min(0.01f)] private float legStepDuration = 0.18f;
        [SerializeField, Min(0f)] private float legFootOffset = 0.015f;
        [SerializeField, Range(0f, 1f)] private float legGaitOverlapProgress = 0.5f;
        [SerializeField, Min(0.01f)] private float legUngroundedResampleInterval = 0.25f;

        public LayerMask TraversableSurfaceMask => traversableSurfaceMask;
        public float SurfaceSearchDistance => surfaceSearchDistance;
        public float MaxSurfaceBlendAngle => maxSurfaceBlendAngle;
        public int AttachConfirmationFrames => attachConfirmationFrames;
        public float MaxMoveSpeed => maxMoveSpeed;
        public float MoveAcceleration => moveAcceleration;
        public float SurfaceHoverOffset => surfaceHoverOffset;
        public float AdhesionForce => adhesionForce;
        public float SurfaceStickSpeed => surfaceStickSpeed;
        public float AdhesionDeadZone => adhesionDeadZone;
        public float SurfaceAlignmentSharpness => surfaceAlignmentSharpness;
        public float MovingHeadingInputThreshold => movingHeadingInputThreshold;
        public float MovingHeadingAlignmentSharpness => movingHeadingAlignmentSharpness;
        public float HeadingAlignmentSharpness => headingAlignmentSharpness;
        public float AirborneGravity => airborneGravity;
        public float JumpSpeed => jumpSpeed;
        public float AirControlCoefficient => airControlCoefficient;
        public float LegSearchRadius => legSearchRadius;
        public float LegMaxReach => legMaxReach;
        public float LegStepDistance => legStepDistance;
        public float LegStepForwardDistance => legStepForwardDistance;
        public float LegStepHeight => legStepHeight;
        public float LegStepDuration => legStepDuration;
        public float LegFootOffset => legFootOffset;
        public float LegGaitOverlapProgress => legGaitOverlapProgress;
        public float LegUngroundedResampleInterval => legUngroundedResampleInterval;

        private void OnValidate()
        {
            surfaceHoverOffset = Mathf.Min(surfaceHoverOffset, surfaceSearchDistance);
        }
    }
}
